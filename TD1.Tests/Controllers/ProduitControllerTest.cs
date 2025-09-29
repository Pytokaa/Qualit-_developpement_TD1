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
using AutoMapper;
using TD1.DTO;
using TD1.Mapper;
using TD1.Tests.Helpers;

namespace TD1.Tests.Controllers;

[TestClass]
[TestSubject(typeof(ProductController))]
[TestCategory("integration")]
public class ProductControllerTest
{
    private AppDbContext _context;
    private ProductController _productController;
    private IMapper _mapper;
    
    // Produits de base pour les tests
    private Product _defaultProduct1;
    private Product _defaultProduct2;
    private Product _defaultProduct3;

    [TestInitialize]
    public void Setup()
    {
        _context = DbContextHelper.CreateInMemoryContext();
        
        
        CleanupDatabase();
        
        InitializeDefaultProducts();
        
        var manager = new ProductManager(_context);
        
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GenericProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = config.CreateMapper();
        
        _productController = new ProductController(manager, mapper);
    }

    //creation de produits par défauts pour effectuer les tests
    private void InitializeDefaultProducts()
    {
        TypeProduit _defaultProductType = new TypeProduit
        {
            IdTypeProduit = 1,
            NomTypeProduit = "assise"
        };
        Marque _defaultMarque = new Marque
        {
            IdMarque = 1,
            NomMarque = "Ikea"
        };
        
        _defaultProduct1 = new Product
        {
            NomProduit = "Chaise par défaut",
            Description = "Une chaise de test",
            NomPhoto = "chaise-test.jpg",
            UriPhoto = "https://example.com/chaise-test.jpg",
            MarqueNavigation = _defaultMarque,
            TypeProduitNavigation = _defaultProductType
        };

        _defaultProduct2 = new Product
        {
            NomProduit = "Table par défaut",
            Description = "Une table de test",
            NomPhoto = "table-test.jpg",
            UriPhoto = "https://example.com/table-test.jpg"
        };
        _defaultProduct3 = new Product
        {
            NomProduit = "Tabouret par defaut",
            Description = "Un tabouret de test",
            NomPhoto = "table-test.jpg",
            UriPhoto = "https://example.com/table-test.jpg",
            MarqueNavigation = _defaultMarque,
            TypeProduitNavigation = _defaultProductType
        };
    }

    [TestMethod]
    public void ShouldGetProduct()
    {
        // Given :
        _context.Produits.Add(_defaultProduct1);
        _context.SaveChanges();
        
        // When : appelle du produit avec getbyid
        ActionResult<ProduitDetailDTO> action = _productController.GetById(_defaultProduct1.IdProduit).GetAwaiter().GetResult();
        
        // Then :
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(ProduitDetailDTO));
        
        ProduitDetailDTO returnProduct = action.Value;
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
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2, _defaultProduct3 });
        _context.SaveChanges();
        
        // When : Récupération des produits via l'API
        var products = _productController.GetAll().GetAwaiter().GetResult();

        // Then : Tests sur le type de retour et des données renvoyées
        Assert.IsNotNull(products);
        Assert.IsInstanceOfType(products, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        
        var productList = (products.Value as IEnumerable<ProduitDTO>)?.ToList();
        Assert.IsTrue(productList?.Count >= 3, "Au moins 3 produits devraient être retournés");
    }
    
    [TestMethod]
    public void GetProductShouldReturnNotFound()
    {
        // Given : 
        int nonExistentId = 0;
        
        // When : Recuperation du produit inexistant via l'API
        ActionResult<ProduitDetailDTO> action = _productController.GetById(nonExistentId).GetAwaiter().GetResult();
        
        // Then : Test sur le type de retour (NotFound)
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");
        Assert.IsNull(action.Value, "Le produit n'est pas null");
    }
    
    [TestMethod]
    public void ShouldCreateProduct()
    {
        // Given : Un nouveau produit à créer
        ProduitDetailDTO productToInsert = _mapper.Map<ProduitDetailDTO>(_defaultProduct1);
        
        
        // When : ajout de ce produit via l'API
        var action = _productController.AddProduit(productToInsert).GetAwaiter().GetResult();
        
        // Then : tests sur le type de action et sur le produit inséré en base de données 
        var createdResult = action.Result as CreatedAtActionResult;
        Assert.IsNotNull(createdResult);

        var returnedDto = createdResult.Value as ProduitDetailDTO;
        Assert.IsNotNull(returnedDto);
        
        //recuperation de l'id a l'aide du retour et non de la class en elle meme à cause des DTO (donc non présent sur les autres class)
        int generatedId = (int)returnedDto.IdProduit;


        var productInDb = _context.Produits.Find(generatedId);
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
        int assignedId = _defaultProduct1.IdProduit;
        ProduitDetailDTO updatedProduct = _mapper.Map<ProduitDetailDTO>(_defaultProduct1);
        // changement de certaines propriétés
        updatedProduct.NomProduit = "ModifiedName";

        // When : modification de ces propriétés via l'API
        IActionResult action = _productController.PutProduit(assignedId, updatedProduct).GetAwaiter().GetResult();
        
        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        Product editedProductInDb = _context.Produits.Find(_defaultProduct1.IdProduit);
        
        Assert.IsNotNull(editedProductInDb);
        Assert.AreEqual("ModifiedName", editedProductInDb.NomProduit);
    }
    
    [TestMethod]
    public void ShouldNotUpdateProductBecauseIdInUrlIsDifferent()
    {
        // Given : ajout d'un produit dans la base de données
        
        _context.Produits.Add(_defaultProduct1);
        _context.SaveChanges();
        ProduitDetailDTO productToUpdate = _mapper.Map<ProduitDetailDTO>(_defaultProduct1);
        int productToUpdateId = _defaultProduct1.IdProduit;
        // changement de certaines propriétés
        _defaultProduct1.NomProduit = "Produit modifié";
        _defaultProduct1.Description = "Description modifiée";

        // When : Utiliser un ID différent dans l'URL
        IActionResult action = _productController.PutProduit(0, productToUpdate).GetAwaiter().GetResult();
        
        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }
    
    [TestMethod]
    public void ShouldNotUpdateProductBecauseProductDoesNotExist()
    {
        // Given : Un produit qui n'existe pas en base
        ProduitDetailDTO productToUpdate = _mapper.Map<ProduitDetailDTO>(_defaultProduct1);
        int nonExistentId = 0;
        
        // When : tentative de modification d'un produit qui n'existe pas
        IActionResult action = _productController.PutProduit(nonExistentId, productToUpdate).GetAwaiter().GetResult();
        
        // Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetProductByName()
    {
        //Given : 
        _context.Produits.Add(_defaultProduct1);
        _context.SaveChanges();
        //When : 
        ActionResult<ProduitDetailDTO> action = _productController.GetByName(_defaultProduct1.NomProduit).GetAwaiter().GetResult();
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(ProduitDetailDTO));
        ProduitDetailDTO returnProductInDb = action.Value;
        Assert.AreEqual(_defaultProduct1.NomProduit, returnProductInDb.NomProduit);
    }

    [TestMethod]
    public void ShouldNotGetProductByNameBecauseItDoesNotExist()
    {
        //Given :
        string nonExistentString = "";
        //When : 
        ActionResult<ProduitDetailDTO> action = _productController.GetByName(nonExistentString).GetAwaiter().GetResult();
        //Then : 
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        Assert.IsNull(action.Value);
    }

    [TestMethod]
    public void ShouldGetAllProductsWhenNoFiltersProvided()
    {
        //Given
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2, _defaultProduct3 });
        _context.SaveChanges();
        
        //When
        var result = _productController.GetAllProductByFilter(null, null, null).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        Assert.IsTrue(result.Value.Count() == 3, "Le nombre de produits retournés doit être 3");
    }

    [TestMethod]
    public void ShouldGetProductsByBrandFilter()
    {
        //Given
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2, _defaultProduct3 });
        _context.SaveChanges();
        //When
        var result = _productController.GetAllProductByFilter(null, "Ikea", null).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        Assert.IsTrue(result.Value.Count() == 2, "Le nombre de produits  retournés doit être 2");
        Assert.IsTrue(result.Value.All(p => p.NomMarque == "Ikea"));
    }

    [TestMethod]
    public void ShouldGetProductsByProductTypeFilter()
    {
        //Given
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2, _defaultProduct3 });
        _context.SaveChanges();
        //When
        var result = _productController.GetAllProductByFilter(null, null, "assise").GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        Assert.IsTrue(result.Value.Count() == 2, "Le nombre de produits  retournés doit être 2");
        Assert.IsTrue(result.Value.All(p => p.NomTypeProduit == "assise"));
    }
    [TestMethod]
    public void ShouldGetProductsByNameFilter()
    {
        //Given
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2, _defaultProduct3 });
        _context.SaveChanges();
        //When
        var result = _productController.GetAllProductByFilter("ta", null, null).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        Assert.IsTrue(result.Value.Count() == 2, "Le nombre de produits  retournés doit être 2");
        Assert.IsTrue(result.Value.All(p => p.NomProduit.Contains("ta", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void ShouldGetProductsByBrandAndNameFilter()
    {
        //Given
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2, _defaultProduct3 });
        _context.SaveChanges();
        //When
        var result = _productController.GetAllProductByFilter("ta", "Ikea", null).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        Assert.IsTrue(result.Value.Count() == 1, "le nombre de produit doit être 1");
        Assert.IsTrue(result.Value.All(p => p.NomMarque == "Ikea"));
        Assert.IsTrue(result.Value.All(p => p.NomProduit.Contains("ta", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void ShouldGetAllProductByBrandAndTypeFilter()
    {
        //Given
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2, _defaultProduct3 });
        _context.SaveChanges();
        //When
        var result = _productController.GetAllProductByFilter(null, "Ikea","assise").GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        Assert.IsTrue(result.Value.Count() == 2, "le nombre de produit doit être 2");
        Assert.IsTrue(result.Value.All(p => p.NomMarque == "Ikea"));
        Assert.IsTrue(result.Value.All(p => p.NomTypeProduit == "assise"));
    }

    [TestMethod]
    public void ShouldGetAllProductByNameAndTypeFilter()
    {
        //Given
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2, _defaultProduct3 });
        _context.SaveChanges();
        //When
        var result = _productController.GetAllProductByFilter("ta", null,"assise").GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        Assert.IsTrue(result.Value.Count() == 1, "le nombre de produit doit être 1");
        Assert.IsTrue(result.Value.All(p => p.NomTypeProduit == "assise"));
        Assert.IsTrue(result.Value.All(p => p.NomProduit.Contains("ta", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void ShouldNotGetProductsByFilterBecauseItDoesNotExist()
    {
        //Given
        _context.Produits.AddRange(new[] { _defaultProduct1, _defaultProduct2, _defaultProduct3 });
        _context.SaveChanges();
        //When
        var result = _productController.GetAllProductByFilter("product that doesn't exist", null, null).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        Assert.IsTrue(result.Value.Count() == 0, "le nombre de produit doit être 0");
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