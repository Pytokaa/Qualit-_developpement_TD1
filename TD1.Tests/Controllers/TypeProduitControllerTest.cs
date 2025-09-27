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
using TD1.Tests.Helpers;

namespace TD1.Tests.Controllers;

[TestClass]
[TestSubject(typeof(TypeProduitController))]
[TestCategory("integration")]
public class TypeProduitControllerTest
{
    private AppDbContext _context;
    private TypeProduitController _typeProductController;
    
    //type produits de base pour les tests
    private TypeProduit _defaultTypeProduit1;
    private TypeProduit _defaultTypeProduit2;

    [TestInitialize]
    public void SetUp()
    {
        
        _context = DbContextHelper.CreateInMemoryContext();
        
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
    public void ShouldNotDeleteProductTypeBecauseItDoesNotExist()
    {
        //Given : 
        int nonExistentId = 9999;
        //When : 
        IActionResult action = _typeProductController.DeleteProductType(nonExistentId).GetAwaiter().GetResult();
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
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

    [TestMethod]
    public void ShouldCreateProductType()
    {
        //Given : 
        TypeProduit productType = _defaultTypeProduit1;
        //When : 
        ActionResult<TypeProduit> action = _typeProductController.AddProductType(productType).GetAwaiter().GetResult();
        //Then : 
        TypeProduit productTypeInDb = _context.TypeProduits.Find(productType.IdTypeProduit);
        Assert.IsNotNull(productTypeInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
    }
    

    [TestMethod]
    public void ShouldUpdateProductType()
    {
        //Given : 
        _context.TypeProduits.Add(_defaultTypeProduit1);
        _context.SaveChanges();
        //changement de données
        _defaultTypeProduit1.NomTypeProduit = "modifiedNameProductType";
        
        //When : 
        IActionResult action = _typeProductController.PutProductType(_defaultTypeProduit1.IdTypeProduit, _defaultTypeProduit1).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));

        TypeProduit editedProductInDb = _context.TypeProduits.Find(_defaultTypeProduit1.IdTypeProduit);
        
        Assert.IsNotNull(editedProductInDb);
        Assert.AreEqual("modifiedNameProductType", editedProductInDb.NomTypeProduit);
    }

    [TestMethod]
    public void ShouldNotUpdateProductTypeBecauseIdInUrlIsDifferent()
    {
        //Given : 
        _context.TypeProduits.Add(_defaultTypeProduit1);
        _context.SaveChanges();
        //changement de données 
        _defaultTypeProduit1.NomTypeProduit = "modifiedNameProductType";
        
        //When : 
        IActionResult action = _typeProductController.PutProductType(0, _defaultTypeProduit1).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }

    [TestMethod]
    public void ShouldNotUpdateProductTypeBecauseProductTypeDoesNotExist()
    {
        //Given 
        int  nonExistentId = 0;
        //When : 
        IActionResult action = _typeProductController.PutProductType(nonExistentId, _defaultTypeProduit1).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetProductTypeByString()
    {
        //Given : 
        _context.TypeProduits.Add(_defaultTypeProduit1);
        _context.SaveChanges();
        //When : 
        ActionResult<TypeProduit> action = _typeProductController.GetByName(_defaultTypeProduit1.NomTypeProduit).GetAwaiter().GetResult();
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(TypeProduit));
        TypeProduit returnProductTypeInDb = action.Value;
        Assert.AreEqual(_defaultTypeProduit1.NomTypeProduit, returnProductTypeInDb.NomTypeProduit);
    }

    [TestMethod]
    public void ShouldNotGetProductTypeByStringBecauseProductTypeDoesNotExist()
    {
        //Given : 
        string nonExistentString = "";
        //When :
        ActionResult<TypeProduit> action = _typeProductController.GetByName(nonExistentString).GetAwaiter().GetResult();
        //Then : 
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        Assert.IsNull(action.Value);
        
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