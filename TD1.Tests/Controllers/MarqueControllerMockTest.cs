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
[TestSubject(typeof(MarqueController))]
[TestCategory("mock")]
public class MarqueControllerMockTest
{
    private readonly MarqueController _brandController;
    private readonly Mock<IDataRepository<Marque>> _brandManager;
    private Marque _defaultBrand1, _defaultBrand2;

    public MarqueControllerMockTest()
    {
        _brandManager = new Mock<IDataRepository<Marque>>();
        _brandController = new MarqueController(_brandManager.Object);
    }
    [TestInitialize]
    public void Setup()
    {
        _defaultBrand1 = new Marque
        {
            IdMarque = 20,
            NomMarque = "marque1"
        };

        _defaultBrand2 = new Marque
        {
            IdMarque = 21,
            NomMarque = "marque2"
        };
    }

    [TestMethod]
    public void ShouldGetBrand()
    {
        //Given
        
        _brandManager.Setup(manager => manager.GetByIdAsync(_defaultBrand1.IdMarque)).ReturnsAsync(_defaultBrand1);
        
        //When
        ActionResult<Marque> action = _brandController.GetById(_defaultBrand1.IdMarque).GetAwaiter().GetResult();
        
        //Then
        _brandManager.Verify(manager => manager.GetByIdAsync(_defaultBrand1.IdMarque), Times.Once);
        
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(Marque));
        Assert.AreEqual(_defaultBrand1, action.Value);
        _brandManager.Verify(manager => manager.GetByIdAsync(_defaultBrand1.IdMarque), Times.Once);
    }

    [TestMethod]
    public void GetShouldReturnNotFound()
    {
        //given
        _brandManager.Setup(manager => manager.GetByIdAsync(30)).ReturnsAsync(new ActionResult<Marque>((Marque)null));
        
        //When
        ActionResult<Marque> action = _brandController.GetById(30).GetAwaiter().GetResult();
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");
        Assert.IsNull(action.Value);
        
        _brandManager.Verify(manager => manager.GetByIdAsync(30),  Times.Once);
    }

    [TestMethod]
    public void ShouldGetAllBrand()
    {
        //Given
        IEnumerable<Marque> brandsInDb = [_defaultBrand1,  _defaultBrand2];

        _brandManager
            .Setup(manager => manager.GetAllAsync())
            .ReturnsAsync(new ActionResult<IEnumerable<Marque>>(brandsInDb));
        //When
        
        var action = _brandController.GetAll().GetAwaiter().GetResult();
        
        //Then
        
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(IEnumerable<Marque>));
        Assert.AreEqual(brandsInDb, action.Value);
        _brandManager.Verify(manager => manager.GetAllAsync(), Times.Once);
    }

    [TestMethod]
    public void ShouldCreateProduct()
    {
        _brandManager
            .Setup(manager => manager.AddAsync(_defaultBrand1));
        
        //When
        
        ActionResult<Marque> action = _brandController.Add(_defaultBrand1).GetAwaiter().GetResult();
        
        //Then
        
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        _brandManager.Verify(manager => manager.AddAsync(_defaultBrand1), Times.Once);
    }

    [TestMethod]
    public void ShouldUpdateProduct()
    {
        //Given
        Marque brandToUpdate= _defaultBrand1;
        Marque  brandUpdated = _defaultBrand1;
        brandUpdated.NomMarque = "updated";
        
        _brandManager
            .Setup(manager => manager.GetByIdAsync(brandToUpdate.IdMarque))
            .ReturnsAsync(brandToUpdate);

        _brandManager
            .Setup(manager => manager.UpdateAsync(brandToUpdate, brandUpdated));

        //When
        IActionResult action = _brandController.Put(brandToUpdate.IdMarque, brandUpdated).GetAwaiter().GetResult();
        
        //Then

        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        _brandManager.Verify(manager => manager.GetByIdAsync(brandToUpdate.IdMarque), Times.Once);
        _brandManager.Verify((manager => manager.UpdateAsync(brandToUpdate, It.IsAny<Marque>())), Times.Once);
    }

    [TestMethod]
    public void ShouldNotUpdateBrandBecauseBrandDoesNotExist()
    {
        //Given
        
        _brandManager
            .Setup(manager => manager.GetByIdAsync(_defaultBrand1.IdMarque))
            .ReturnsAsync(new ActionResult<Marque>((Marque)null));
        
        //When
        
        IActionResult action =  _brandController.Put(_defaultBrand1.IdMarque, _defaultBrand1).GetAwaiter().GetResult();
        
        //Then
        
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        
        _brandManager.Verify(manager => manager.GetByIdAsync(_defaultBrand1.IdMarque), Times.Once);
    }

    [TestMethod]
    public void ShouldDeleteBrand()
    {
        //Given
        _brandManager
            .Setup(manager => manager.GetByIdAsync(_defaultBrand1.IdMarque))
            .ReturnsAsync(_defaultBrand1);
        
        //When
        var action = _brandController.Delete(_defaultBrand1.IdMarque).GetAwaiter().GetResult();
        
        //Then
        
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        _brandManager.Verify(manager => manager.DeleteAsync(_defaultBrand1), Times.Once);
    }

    [TestMethod]
    public void ShouldNotDeleteBrandBecauseItDoesNotExist()
    {
        //Given
        _brandManager
            .Setup(manager => manager.GetByIdAsync(30))
            .ReturnsAsync(new ActionResult<Marque>((Marque)null));
        
        //When
        var action = _brandController.Delete(30).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        _brandManager.Verify(manager => manager.GetByIdAsync(30), Times.Once);
    }

    [TestMethod]
    public void ShouldGetBrandByName()
    {
        //Given
        _brandManager
            .Setup(manager => manager.GetByStringAsync(_defaultBrand1.NomMarque))
            .ReturnsAsync(_defaultBrand1);
        //When
        var action = _brandController.GetByName(_defaultBrand1.NomMarque).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(Marque));
        Assert.AreEqual(action.Value, _defaultBrand1);
        _brandManager.Verify(manager => manager.GetByStringAsync(_defaultBrand1.NomMarque), Times.Once);
    }

    [TestMethod]
    public void ShouldNotGetBrandByNameBecauseItDoesntExist()
    {
        //Given
        _brandManager
            .Setup(manager => manager.GetByStringAsync(_defaultBrand1.NomMarque))
            .ReturnsAsync(new ActionResult<Marque>((Marque)null));
        //When
        var action = _brandController.GetByName(_defaultBrand1.NomMarque).GetAwaiter().GetResult();
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        _brandManager.Verify(manager => manager.GetByStringAsync(_defaultBrand1.NomMarque), Times.Once);
    }
}