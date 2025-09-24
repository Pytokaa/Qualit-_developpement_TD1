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
[TestSubject(typeof(TypeProduitController))]
[TestCategory("mock")]
public class TypeProduitControllerMockTest
{
    private readonly TypeProduitController _productTypeController;
    private readonly Mock<IDataRepository<TypeProduit>> _productTypeManager;
    private TypeProduit _defaultProductType1,  _defaultProductType2;

    public TypeProduitControllerMockTest()
    {
        _productTypeManager = new Mock<IDataRepository<TypeProduit>>();
        _productTypeController = new TypeProduitController(_productTypeManager.Object);
    }

    [TestInitialize]
    public void SetUp()
    {
        _defaultProductType1 = new TypeProduit()
        {
            IdTypeProduit = 20,
            NomTypeProduit = "productType1"
        };
        _defaultProductType2 = new TypeProduit()
        {

            IdTypeProduit = 21,
            NomTypeProduit = "productType2"
        };
    }

    [TestMethod]
    public void ShouldGetProductType()
    {
        //Given
        _productTypeManager
            .Setup(manager => manager.GetByIdAsync(_defaultProductType1.IdTypeProduit))
            .ReturnsAsync(_defaultProductType1);
        //When
        var action = _productTypeController.GetById(_defaultProductType1.IdTypeProduit).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(TypeProduit));
        Assert.AreEqual(_defaultProductType1, action.Value);
        _productTypeManager.Verify(manager => manager.GetByIdAsync(_defaultProductType1.IdTypeProduit), Times.Once);
    }

    [TestMethod]
    public void ShouldNotGetProductTypeBecauseItDoesNotExist()
    {
        //Given
        _productTypeManager
            .Setup(manager => manager.GetByIdAsync(30))
            .ReturnsAsync(new ActionResult<TypeProduit>((TypeProduit)null));
        //When
        var action = _productTypeController.GetById(30).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        _productTypeManager.Verify(manager => manager.GetByIdAsync(30), Times.Once);
    }

    [TestMethod]
    public void ShouldGetProductTypeByName()
    {
        //Given
        _productTypeManager
            .Setup(manager => manager.GetByStringAsync(_defaultProductType1.NomTypeProduit))
            .ReturnsAsync(_defaultProductType1);
        //When
        var action = _productTypeController.GetByName(_defaultProductType1.NomTypeProduit).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(TypeProduit));
        Assert.AreEqual(_defaultProductType1, action.Value);
        _productTypeManager.Verify(manager => manager.GetByStringAsync(_defaultProductType1.NomTypeProduit), Times.Once);
    }

    [TestMethod]
    public void ShouldNotGetProductTypeByName()
    {
        //Given
        _productTypeManager
            .Setup(manager => manager.GetByStringAsync(_defaultProductType1.NomTypeProduit))
            .ReturnsAsync(new  ActionResult<TypeProduit>((TypeProduit)null));
        //When
        var action = _productTypeController.GetByName(_defaultProductType1.NomTypeProduit).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult));
        _productTypeManager.Verify(manager => manager.GetByStringAsync(_defaultProductType1.NomTypeProduit),  Times.Once);
    }

    [TestMethod]
    public void ShouldUpdateProductType()
    {
        //Given
        TypeProduit productTypeToUpdate = _defaultProductType1;
        TypeProduit productTypeUpdated = _defaultProductType1;
        productTypeUpdated.NomTypeProduit = "productTypeModified";
        _productTypeManager
            .Setup(manager => manager.GetByIdAsync(productTypeToUpdate.IdTypeProduit))
            .ReturnsAsync(productTypeToUpdate);
        productTypeToUpdate.NomTypeProduit = "productType1Modified";

        _productTypeManager
            .Setup(manager => manager.UpdateAsync(productTypeToUpdate, productTypeToUpdate));
        
        //When

        IActionResult action =
            _productTypeController.Put(productTypeToUpdate.IdTypeProduit, productTypeUpdated).GetAwaiter().GetResult();
        
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        _productTypeManager.Verify(manager => manager.GetByIdAsync(productTypeToUpdate.IdTypeProduit), Times.Once);
        _productTypeManager.Verify(manager => manager.UpdateAsync(productTypeToUpdate,It.IsAny<TypeProduit>()), Times.Once);
    }

    [TestMethod]
    public void ShouldNotUpdateProductTypeBecauseItDoesNotExist()
    {
        //Given
        _productTypeManager
            .Setup(manager => manager.GetByIdAsync(_defaultProductType1.IdTypeProduit))
            .ReturnsAsync(new ActionResult<TypeProduit>((TypeProduit)null));
        //When
        IActionResult action =  _productTypeController.Put(_defaultProductType1.IdTypeProduit, _defaultProductType1).GetAwaiter().GetResult();
        //Then 
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        
        _productTypeManager.Verify(manager => manager.GetByIdAsync(_defaultProductType1.IdTypeProduit), Times.Once);
        _productTypeManager.Verify(manager => manager.UpdateAsync(It.IsAny<TypeProduit>(), _defaultProductType1), Times.Never);
    }

    [TestMethod]
    public void ShouldDeleteProductType()
    {
        //Given
        _productTypeManager
            .Setup(manager => manager.GetByIdAsync(_defaultProductType1.IdTypeProduit))
            .ReturnsAsync(_defaultProductType1);

        _productTypeManager
            .Setup(manager => manager.DeleteAsync(_defaultProductType1));
        
        //When
        IActionResult action = _productTypeController.Delete(_defaultProductType1.IdTypeProduit).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        _productTypeManager.Verify(manager => manager.GetByIdAsync(_defaultProductType1.IdTypeProduit), Times.Once);
        _productTypeManager.Verify(manager => manager.DeleteAsync(_defaultProductType1), Times.Once);
    }

    [TestMethod]
    public void ShouldNotDeleteProductTypeBecauseItDoesNotExist()
    {
        //Given
        _productTypeManager
            .Setup(manager => manager.GetByIdAsync(_defaultProductType1.IdTypeProduit))
            .ReturnsAsync(new ActionResult<TypeProduit>((TypeProduit)null));
        //When
        IActionResult action =  _productTypeController.Delete(_defaultProductType1.IdTypeProduit).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        
        _productTypeManager.Verify(manager => manager.GetByIdAsync(_defaultProductType1.IdTypeProduit), Times.Once);
    }
}