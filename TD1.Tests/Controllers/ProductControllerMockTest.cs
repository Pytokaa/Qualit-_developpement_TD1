using System.Collections.Generic;
using TD1.Controllers;
using TD1.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TD1.Repository;


namespace TD1.Tests.Controllers;



[TestClass]
[TestSubject(typeof(ProduitController))]
[TestCategory("mock")]
public class ProductControllerMockTest
{
    private readonly ProduitController _produitController;
    private readonly Mock<IDataRepository<Produit>> _productManager;

    public ProductControllerMockTest()
    {
        var manager = new Mock<IDataRepository<Produit>>();
        _produitController = new ProduitController(manager.Object);
        _productManager = new Mock<IDataRepository<Produit>>();
    }

    [TestMethod]
    public void ShoufdGetProduit()
    {
        Produit productInDb = new Produit
        {
            IdProduit = 30,
            NomProduit = "Chaise par défaut",
            Description = "Une chaise de test",
            NomPhoto = "chaise-test.jpg",
            UriPhoto = "https://example.com/chaise-test.jpg"
        };
        
        
    }

    [TestMethod]
    public void ShouldDeleteProduct()
    {
        Produit productInDb = new Produit
        {
            IdProduit = 20,
            NomProduit = "Chaise par défaut",
            Description = "Une chaise de test",
            NomPhoto = "chaise-test.jpg",
            UriPhoto = "https://example.com/chaise-test.jpg"
        };

        //_productManager.Setup(manager => manager.GetByIdAsync(productInDb.IdProduit)).ReturnsAsync();

        _productManager.Setup(manager => manager.DeleteAsync(productInDb));

        IActionResult action = _produitController.DeleteProduit(productInDb.IdProduit).GetAwaiter().GetResult();
        
    }

    [TestMethod]
    public void ShouldGetAllProduct()
    {
        Produit productInDb = new Produit
        {
            IdProduit = 20,
            NomProduit = "Chaise par défaut",
            Description = "Une chaise de test",
            NomPhoto = "chaise-test.jpg",
            UriPhoto = "https://example.com/chaise-test.jpg"
        };
        Produit productInDb2 = new Produit
        {
            IdProduit = 21,
            NomProduit = "Chaise par défaut",
            Description = "Une chaise de test",
            NomPhoto = "chaise-test.jpg",
            UriPhoto = "https://example.com/chaise-test.jpg"
        };

        //_productManager.Setup(manager => manager.GetAllAsync()).ReturnsAsync(new ActionResult<IEnumerable<Produit>>());
        
    }


    [TestMethod]
    public void ShouldCreateProduct()
    {
        Produit productInDb = new Produit
        {
            IdProduit = 21,
            NomProduit = "Chaise par défaut",
            Description = "Une chaise de test",
            NomPhoto = "chaise-test.jpg",
            UriPhoto = "https://example.com/chaise-test.jpg"
        };   
        //_productManager.
    }
    
    
}