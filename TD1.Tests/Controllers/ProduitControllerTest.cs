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
public class ProduitControllerTest
{
    private ProduitDbContext _context;
    private ProduitController _produitController;
    private ProduitManager _manager;

    public  ProduitControllerTest()
    {
        var builder = new DbContextOptionsBuilder<ProduitDbContext>().UseNpgsql("Server=localhost;Port=5432;Database=produit_db;Username=postgres;Password=postgres");
        _context = new ProduitDbContext(builder.Options);
        _manager = new ProduitManager(_context);
        _produitController = new ProduitController(_manager);
    }


    [TestMethod]
    public void ShouldGetAllProduits()
    {
        //given
        IEnumerable<Produit> produits = [
            new Produit()
            {
                NomProduit = "chaise",
                Description = "en bien",
                NomPhoto = "Photo de chaise",
                UriPhoto = "text"
            },
            new Produit()
            {
                NomProduit = "table",
                Description = "en bien",
                NomPhoto = "Photo de table",
                UriPhoto = "text"
            }
        ];
        _context.Produits.AddRange(produits);
        _context.SaveChanges();
        
        //when

        var products = _produitController.GetAll().GetAwaiter().GetResult();
        
        //then
        
        Assert.IsNotNull(products);
        Assert.IsInstanceOfType(products, typeof(ActionResult<IEnumerable<Produit>>));





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
        Produit produitToInsert = new Produit()
        {
            NomProduit = "CreatedProduit",
            Description = "en bien",
            NomPhoto = "Photo de chaise",
            UriPhoto = "text"
        };
        

        //when : j'appelle la méthode get de mon api pour récuperer le produit
        
       ActionResult<Produit> action = _produitController.AddProduit(produitToInsert).GetAwaiter().GetResult();
       
       var produitInDb = _context.Produits.Find(produitToInsert.IdProduit);
       
       
       //then 
       Assert.IsNotNull(produitInDb);
       Assert.IsNotNull(action);
       Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        
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
        

        _context.Produits.Add(produitToUpdateInDb);
        _context.SaveChanges();
        
        //when 
        produitToUpdateInDb.NomProduit = "UpdatedProduit";
        IActionResult action = _produitController.PutProduit(produitToUpdateInDb.IdProduit,  produitToUpdateInDb).GetAwaiter().GetResult();
        
        
        var produitInDb =  _context.Produits.Find(produitToUpdateInDb.IdProduit);
        
        //then
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNotNull(produitInDb);
        Assert.IsInstanceOfType(produitInDb, typeof(Produit));
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
       
        //then
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Produits.Find(produitInDb.IdProduit));
    }

    [TestMethod]
    public void ShouldNotDeleteProductBecauseItDoesntExist()
    {
        //given : aucun produit en base de données
        //when 
        
        var action = _produitController.DeleteProduit(0).GetAwaiter().GetResult();
        
        //then
        
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestCleanup]
    public void Cleanup() //obligé de faire ainsi a votre demande sinon le cleanup interfere avec le reste des tests 
    {
        var builder = new DbContextOptionsBuilder<ProduitDbContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=produit_db;Username=postgres;Password=postgres");

        using var cleanupContext = new ProduitDbContext(builder.Options);
        cleanupContext.Produits.RemoveRange(cleanupContext.Produits.ToList());
        cleanupContext.SaveChanges();
    }
    
    
}