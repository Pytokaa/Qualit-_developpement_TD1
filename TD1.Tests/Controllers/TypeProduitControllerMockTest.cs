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
}