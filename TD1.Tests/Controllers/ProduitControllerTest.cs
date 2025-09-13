using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TD1.Models.EntityFramework;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TD1.Controllers;
using TD1.Models;
using TD1.Repository;

namespace TD1.Tests.Controllers;

[TestClass]
[TestSubject(typeof(ProduitController))]
[TestCategory("integration")]
public class ProductControllerTest
{
    private ProduitDbContext _context;
    private ProduitController _productController;
    
    // Produits de base pour les tests
    private Produit _defaultProduct1;
    private Produit _defaultProduct2;

    [TestInitialize]
    public void Setup()
    {
        // Créer un nouveau contexte pour chaque test
        var builder = new DbContextOptionsBuilder<ProduitDbContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=produit_db;Username=postgres;Password=postgres");
        
        _context = new ProduitDbContext(builder.Options);
        
        CleanupDatabase();
        
        InitializeDefaultProducts();
        
        var manager = new ProduitManager(_context);
        _productController = new ProduitController(manager);
    }

    //creation de produits par défauts pour effectuer les tests
    private void InitializeDefaultProducts()
    {
        _defaultProduct1 = new Produit
        {
            NomProduit = "Chaise par défaut",
            Description = "Une chaise de test",
            NomPhoto = "chaise-test.jpg",
            UriPhoto = "https://example.com/chaise-test.jpg"
        };

        _defaultProduct2 = new Produit
        {
            NomProduit = "Table par défaut",
            Description = "Une table de test",
            NomPhoto = "table-test.jpg",
            UriPhoto = "https://example.com/table-test.jpg"
        };
    }

    [TestMethod]
    public void ShouldGetProduct()
    {
        // Given :
        _context.Produits.Add(_defaultProduct1);
        _context.SaveChanges();
        
        // When : appelle du produit avec getbyid
        ActionResult<Produit> action = _productController.GetById(_defaultProduct1.IdProduit).GetAwaiter().GetResult();
        
        // Then :
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(Produit));
        
        Produit returnProduct = action.Value;
        Assert.AreEqual(_defaultProduct1.NomProduit, returnProduct.NomProduit);
    }

    [TestMethod]
    public void ShouldDeleteProduct()
    {
        // Given :
        _context.Produits.Add(_defaultProduct1);
        _context.SaveChanges();
        
        // When : Suppression depuis l'API du produit
        IActionResult action = _productController.DeleteProduit(_defaultProduct1.IdProduit).GetAwaiter().GetResult();
        
        // Then : Test sur le type de retour (NoContentResult)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Produits.Find(_defaultProduct1.IdProduit));
    }
    
    [TestMethod]
    public void ShouldNotDeleteProductBecauseProductDoesNotExist()
    {
        // Given : 
        int nonExistentId = 99999;
        
        // When : Suppression depuis l'API d'un produit qui n'existe pas
        IActionResult action = _productController.DeleteProduit(nonExistentId).GetAwaiter().GetResult();
        
        // Then : Test sur le type de action.result (NotFound)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllProducts()
    {
        // Given : Ajout de deux produits dans la base de données
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2 });
        _context.SaveChanges();
        
        // When : Récupération des produits via l'API
        var products = _productController.GetAll().GetAwaiter().GetResult();

        // Then : Tests sur le type de retour et des données renvoyées
        Assert.IsNotNull(products);
        Assert.IsInstanceOfType(products.Value, typeof(IEnumerable<Produit>));
        
        var productList = (products.Value as IEnumerable<Produit>)?.ToList();
        Assert.IsTrue(productList?.Count >= 2, "Au moins 2 produits devraient être retournés");
    }
    
    [TestMethod]
    public void GetProductShouldReturnNotFound()
    {
        // Given : 
        int nonExistentId = 0;
        
        // When : Recuperation du produit inexistant via l'API
        ActionResult<Produit> action = _productController.GetById(nonExistentId).GetAwaiter().GetResult();
        
        // Then : Test sur le type de retour (NotFound)
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");
        Assert.IsNull(action.Value, "Le produit n'est pas null");
    }
    
    [TestMethod]
    public void ShouldCreateProduct()
    {
        // Given : Un nouveau produit à créer
        Produit productToInsert = _defaultProduct1;
        
        // When : ajout de ce produit via l'API
        ActionResult<Produit> action = _productController.AddProduit(productToInsert).GetAwaiter().GetResult();
        
        // Then : tests sur le type de action et sur le produit inséré en base de données 
        Produit productInDb = _context.Produits.Find(productToInsert.IdProduit);
        
        Assert.IsNotNull(productInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
    }

    [TestMethod]
    public void ShouldUpdateProduct()
    {
        // Given : ajout d'un produit dans la base de données
        _context.Produits.Add(_defaultProduct1);
        _context.SaveChanges();
        
        // changement de certaines propriétés
        _defaultProduct1.NomProduit = "Produit modifié";
        _defaultProduct1.Description = "Description modifiée";

        // When : modification de ces propriétés via l'API
        IActionResult action = _productController.PutProduit(_defaultProduct1.IdProduit, _defaultProduct1).GetAwaiter().GetResult();
        
        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        Produit editedProductInDb = _context.Produits.Find(_defaultProduct1.IdProduit);
        
        Assert.IsNotNull(editedProductInDb);
        Assert.AreEqual("Produit modifié", editedProductInDb.NomProduit);
        Assert.AreEqual("Description modifiée", editedProductInDb.Description);
    }
    
    [TestMethod]
    public void ShouldNotUpdateProductBecauseIdInUrlIsDifferent()
    {
        // Given : ajout d'un produit dans la base de données
        _context.Produits.Add(_defaultProduct1);
        _context.SaveChanges();
        
        // changement de certaines propriétés
        _defaultProduct1.NomProduit = "Produit modifié";
        _defaultProduct1.Description = "Description modifiée";

        // When : Utiliser un ID différent dans l'URL
        IActionResult action = _productController.PutProduit(0, _defaultProduct1).GetAwaiter().GetResult();
        
        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }
    
    [TestMethod]
    public void ShouldNotUpdateProductBecauseProductDoesNotExist()
    {
        // Given : Un produit qui n'existe pas en base
        int nonExistentId = 0;
        
        // When : tentative de modification d'un produit qui n'existe pas
        IActionResult action = _productController.PutProduit(nonExistentId, _defaultProduct1).GetAwaiter().GetResult();
        
        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestCleanup]
    public void Cleanup()
    {
        CleanupDatabase();
        _context?.Dispose();
    }

    private void CleanupDatabase()
    {
        if (_context != null)
        {
            try
            {
                var allProducts = _context.Produits.ToList();
                _context.Produits.RemoveRange(allProducts);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du nettoyage : {ex.Message}");
            }
        }
    }
}