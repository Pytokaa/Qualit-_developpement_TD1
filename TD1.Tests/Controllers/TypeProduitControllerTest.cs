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
[TestSubject(typeof(TypeProduitController))]
[TestCategory("integration")]
public class TypeProduitControllerTest
{
    private ProduitDbContext _context;
    private TypeProduitController _typeProductController;
    
    //type produits de base pour les tests
    private TypeProduit _defaultTypeProduit1;
    private TypeProduit _defaultTypeProduit2;

    [TestInitialize]
    public void SetUp()
    {
        // Créer un nouveau contexte pour chaque test
        var builder = new DbContextOptionsBuilder<ProduitDbContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=produit_db;Username=postgres;Password=postgres");
        
        _context = new ProduitDbContext(builder.Options);
        
        CleanupDatabase();
        
        InitializeDefaultProducts();
        
        var manager = new TypeProduitManager(_context);
        _typeProductController = new TypeProduitController(manager);
    }
    //Creation de type produits par défauts afin d'effectuer des tests
    public void InitializeDefaultProducts()
    {
        _defaultTypeProduit1 = new TypeProduit
        {
            NomTypeProduit = "nomTypeProduit1",
        };
        _defaultTypeProduit2 = new TypeProduit
        {
            NomTypeProduit = "nomTypeProduit2",
        };
    }

    [TestMethod]
    public void ShouldGetProductType()
    {
        //Given : 
        _context.TypeProduits.Add(_defaultTypeProduit1);
        _context.SaveChanges();
        
        //When : 
        ActionResult<TypeProduit> action = _typeProductController.GetById(_defaultTypeProduit1.IdTypeProduit).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(TypeProduit));

        TypeProduit returnProductType = action.Value;
        Assert.AreEqual(_defaultTypeProduit1.NomTypeProduit, returnProductType.NomTypeProduit);
    }

    [TestMethod]
    public void ShouldDeleteProductType()
    {
        //Given : 
        _context.TypeProduits.Add(_defaultTypeProduit1);
        _context.SaveChanges();
        //When : 
        IActionResult action = _typeProductController.DeleteProductType(_defaultTypeProduit1.IdTypeProduit).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.TypeProduits.Find(_defaultTypeProduit1.IdTypeProduit));
    }

    [TestMethod]
    public void ShouldGetAllProductTypes()
    {
        //Given : 
        _context.TypeProduits.AddRange(new []{_defaultTypeProduit1, _defaultTypeProduit2});
        _context.SaveChanges();
        //When : 
        var productTypes = _typeProductController.GetAll().GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(productTypes);
        Assert.IsInstanceOfType(productTypes.Value, typeof(IEnumerable<TypeProduit>));
        
        var productTypesList = (productTypes.Value as IEnumerable<TypeProduit>).ToList();
        Assert.IsTrue(productTypesList?.Count >= 2, "Au moins 2 types de produits doivent être présents");
    }

    [TestMethod]
    public void GetProductTypesShouldReturnNotFound()
    {
        //Given : 
        int nonExistentId = 0;
        
        //When : 
        ActionResult<TypeProduit> action = _typeProductController.GetById(nonExistentId).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        Assert.IsNull(action.Value, "Le type produit n'est paas null");
    }
    

    [TestCleanup]
    public void Cleanup()
    {
        CleanupDatabase();
        _context.Dispose();
    }

    private void CleanupDatabase()
    {
        if (_context != null)
        {
            try
            {
                var allProductTypes = _context.TypeProduits.ToList();
                _context.TypeProduits.RemoveRange(allProductTypes);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Errur lors du nettoyage : {ex.Message}");
            }
        }
    }
}