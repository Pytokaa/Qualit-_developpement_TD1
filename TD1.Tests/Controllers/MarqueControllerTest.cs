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
using TD1.Mapper;

namespace TD1.Tests.Controllers;


[TestClass]
[TestSubject(typeof(MarqueController))]
[TestCategory("integration")]
public class MarqueControllerTest
{
    private ProduitDbContext _context;
    private MarqueController _brandController;
    
    //marques de base pour les tests
    private Marque _defaultBrand1;
    private Marque _defaultBrand2;

    [TestInitialize]
    public void SetUp()
    {
        //pour chaque test => creer un nouveau context
        var builder = new DbContextOptionsBuilder<ProduitDbContext>()
            .UseNpgsql("Server=localhost;Port=5432;Database=produit_db;Username=postgres;Password=postgres");
        
        _context = new ProduitDbContext(builder.Options);
        CleanupDatabase();
        InitializeDefaultBrands();
        
        var manager = new MarqueManager(_context);
   
        _brandController = new MarqueController(manager);
    }

    private void InitializeDefaultBrands()
    {
        _defaultBrand1 = new Marque
        {
            NomMarque = "marque1"
        };
        _defaultBrand2 = new Marque
        {
            NomMarque = "marque2"
        };
    }

    [TestMethod]
    public void ShouldGetBrand()
    {
        //Given : 
        _context.Marques.Add(_defaultBrand1);
        _context.SaveChanges();
        
        //When : appelle une marque avec getbyid
        ActionResult<Marque> action = _brandController.GetById((_defaultBrand1.IdMarque)).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(Marque));

        Marque returnBrand = action.Value;
        Assert.AreEqual(_defaultBrand1.IdMarque, returnBrand.IdMarque);
    }

    [TestMethod]
    public void ShouldDeleteBrand()
    {
        //Given : 
        _context.Marques.Add(_defaultBrand1);
        _context.SaveChanges();
        //When  :
        IActionResult action = _brandController.Delete(_defaultBrand1.IdMarque).GetAwaiter().GetResult();
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        Assert.IsNull(_context.Marques.Find(_defaultBrand1.IdMarque));
    }

    [TestMethod]
    public void ShouldNotDeleteBrandBecauseItDoesNotExist()
    {
        //Given : 
        int nonExistentId = 9999;
        //When : 
        IActionResult action = _brandController.Delete(nonExistentId).GetAwaiter().GetResult();
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllBrands()
    {
        //Given : 
        _context.Marques.AddRange(new []{_defaultBrand1, _defaultBrand2});
        _context.SaveChanges();
        //When : 
        var brands = _brandController.GetAll().GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(brands);
        Assert.IsInstanceOfType(brands.Value, typeof(IEnumerable<Marque>));

        var brandList = (brands.Value as IEnumerable<Marque>)?.ToList();
        Assert.IsTrue(brandList?.Count >= 2, "Au moins 2 marques doivent etre dans la abse de données");
    }

    [TestMethod]
    public void GetBrandShouldReturnNotFound()
    {
        //Given : 
        int nonExistentId = 0;
        //When : Recuperation de la marque inexistante via l'API
        ActionResult<Marque> action = _brandController.GetById(nonExistentId).GetAwaiter().GetResult();
        //Then : 
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");
        Assert.IsNull(action.Value, "La marque n'est pas null");
    }

    [TestMethod]
    public void ShouldCreateBrand()
    {
        //Given : 
        Marque brandToInsert = _defaultBrand1;
        
        //When : 
        ActionResult<Marque> action = _brandController.Add(brandToInsert).GetAwaiter().GetResult();
        
        //Then : 
        Marque brandInDb = _context.Marques.Find(brandToInsert.IdMarque);
        
        Assert.IsNotNull(brandInDb);
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
    }

    [TestMethod]
    public void ShouldUpdateBrand()
    {
        //Given : 
        _context.Marques.Add(_defaultBrand1);
        _context.SaveChanges();
        //changement d'une donnée
        _defaultBrand1.NomMarque = "modifiedBrand";
        //When : 
        IActionResult action = _brandController.Put(_defaultBrand1.IdMarque, _defaultBrand1).GetAwaiter().GetResult();
        
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        Marque editedBrandInDb = _context.Marques.Find(_defaultBrand1.IdMarque);
        
        Assert.IsNotNull(editedBrandInDb);
        Assert.AreEqual("modifiedBrand", editedBrandInDb.NomMarque);
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseIdInUrlIsDifferent()
    {
        //Given : 
        _context.Marques.Add(_defaultBrand1);
        _context.SaveChanges();
        //changement de certaines propriétés
        _defaultBrand1.NomMarque = "modifiedBrand";
        
        //When : 
        IActionResult action = _brandController.Put(0, _defaultBrand1).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseItDoesNotExist()
    {
        
        //Given : 
        int nonExistentId = 0;
        //When : 
        IActionResult action = _brandController.Put(nonExistentId, _defaultBrand1).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetBrandByName()
    {
        //Given : 
        _context.Marques.Add(_defaultBrand1);
        _context.SaveChanges();
        //When : 
        ActionResult<Marque> action = _brandController.GetByName(_defaultBrand1.NomMarque).GetAwaiter().GetResult();
        //Then : 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(Marque));
        Marque returnBrandInDb = action.Value;
        Assert.AreEqual(_defaultBrand1.NomMarque, returnBrandInDb.NomMarque);
    }

    [TestMethod]
    public void ShouldNotGetByNameBecauseBrandDoesNotExist()
    {
        //Given : 
        string nonExistentBrandName = "";
        //When : 
        ActionResult<Marque> action = _brandController.GetByName(_defaultBrand1.NomMarque).GetAwaiter().GetResult();
        //Then : 
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        Assert.IsNull(action.Value);
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
                var allBrands = _context.Marques.ToList();
                _context.Marques.RemoveRange(allBrands);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du nettoyage des données : {ex.Message}");
            }
        }
    }
}