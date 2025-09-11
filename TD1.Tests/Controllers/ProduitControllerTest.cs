using System;
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
public class ProduitControllerTest
{
    private ProduitDbContext _context;
    private ProduitController _produitController;
    private ProduitManager _manager;
    [TestInitialize]
    public void Init()
    {
        var builder = new DbContextOptionsBuilder<ProduitDbContext>().UseNpgsql("Server=localhost;Port=5432;Database=produit_db;Username=postgres;Password=postgres");
        _context = new ProduitDbContext(builder.Options);
        _manager = new ProduitManager(_context);
        _produitController = new ProduitController(_manager);
    }

    [TestMethod]
    public void ShouldGetProduit()
    {
        
        
        //given : un produit en base de donnnées
        Produit produitInDb = new Produit()
        {
            NomProduit = "chaise",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };

        _context.Produits.Add(produitInDb);
        _context.SaveChanges();

        //when : j'appelle la méthode get de mon api pour récuperer le produit
        
        ActionResult<Produit> action = _produitController.GetById(produitInDb.IdProduit).GetAwaiter().GetResult();
        
        
        //then : On recupère le produit et le code de retour est 200
        Assert.IsNotNull(action);
        //Assert.IsInstanceOfType(action.Result, typeof(OkObjectResult));
        Assert.IsInstanceOfType(action.Value, typeof(Produit));
        
        //Ici on regarde si le nom du produit est celui dans la base de données
        Produit returnProduit = action.Value;
        Assert.AreEqual(produitInDb.NomProduit, returnProduit.NomProduit);
    }
    [TestMethod]
    public void ShouldGetProduitNotFoundResult()
    {
        
        
        //given : un produit en base de donnnées
        Produit produitInDb = new Produit()
        {
            NomProduit = "chaise",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };

        _context.Produits.Add(produitInDb);
        _context.SaveChanges();

        //when : j'appelle la méthode get de mon api pour récuperer le produit
        
        ActionResult<Produit> action = _produitController.GetById(9999).GetAwaiter().GetResult();
        
        //then : On recupère le produit et le code de retour est 200
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        
    }

    [TestMethod]
    public void ShouldCreateProduit()
    {
        //given : un produit en base de donnnées
        Produit produitInDb = new Produit()
        {
            NomProduit = "CreatedProduit",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };

        _context.Produits.Add(produitInDb);
        _context.SaveChanges();

        //when : j'appelle la méthode get de mon api pour récuperer le produit
        
        ActionResult<Produit> action = _produitController.GetById(produitInDb.IdProduit).GetAwaiter().GetResult();
        
        //then 
        
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(Produit));
        Assert.AreEqual(produitInDb.NomProduit, action.Value.NomProduit);
        
    }

    [TestMethod]
    public void ShouldUpdateProduit()
    {
        
        //given : un produit en base de donnnées
        Produit produitToUpdateInDb = new Produit()
        {
            NomProduit = "ProduitToUpdate",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };
        Produit produitUpdatedInDb = new Produit()
        {
            NomProduit = "ProduitUpdated",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };

        _context.Produits.Add(produitToUpdateInDb);
        _context.SaveChanges();
        
        //when 
        
        IActionResult action = _produitController.PutProduit(produitToUpdateInDb.IdProduit,  produitUpdatedInDb).GetAwaiter().GetResult();
        
        //for action
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        

        ActionResult<Produit> actionResult = _produitController.GetById(produitToUpdateInDb.IdProduit).GetAwaiter().GetResult();
        
        //then 
        
        
        //for actionResult
        
        //Assert.IsInstanceOfType(actionResult.Result, typeof(Produit));
        //Assert.AreEqual(actionResult.Value.NomProduit, produitUpdatedInDb.NomProduit);
            
    }
    
    [TestMethod]
    public void ShouldNotUpdateProduit()
    {
        //given : un produit en base de donnnées
        Produit produitToUpdateInDb = new Produit()
        {
            NomProduit = "ProduitToUpdate",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };
        Produit produitUpdatedInDb = new Produit()
        {
            NomProduit = "ProduitUpdated",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };

        _context.Produits.Add(produitToUpdateInDb);
        _context.SaveChanges();
        
        //when 
        
        IActionResult action = _produitController.PutProduit(0,  produitUpdatedInDb).GetAwaiter().GetResult();
        
        //then 
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        
        
    }
    
    [TestMethod]
    public void ShouldDeleteProduit()
    {
        //given : un produit en base de donnnées
        Produit produitInDb = new Produit()
        {
            NomProduit = "ProduitToDelete",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };

        _context.Produits.Add(produitInDb);
        _context.SaveChanges();

        //when : j'appelle la méthode get de mon api pour récuperer le produit
        
        IActionResult action = _produitController.DeleteProduit(produitInDb.IdProduit).GetAwaiter().GetResult();
        ActionResult<Produit> actionDelete = _produitController.GetById(produitInDb.IdProduit).GetAwaiter().GetResult();
        
        //then
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsInstanceOfType(actionDelete.Result, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldNotDeleteProduit()
    {
        //given : un produit en base de donnnées
        Produit produitInDb = new Produit()
        {
            NomProduit = "ProduitToDelete",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };

        _context.Produits.Add(produitInDb);
        _context.SaveChanges();
        
        //when 
        
        var action = _produitController.DeleteProduit(0).GetAwaiter().GetResult();
        
        //then
        
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }
    
    
    
}