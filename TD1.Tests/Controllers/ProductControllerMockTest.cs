using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using TD1.Controllers;
using TD1.Models;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TD1.Mapper;
using TD1.Repository;
using TD1.DTO;


namespace TD1.Tests.Controllers;


[TestClass]
[TestSubject(typeof(ProductController))]
[TestCategory("mock")]
public class ProductControllerMockTest
{
    private readonly ProductController _productController;
    private readonly Mock<IDataRepository<Product>>  _produitManager;
    private readonly IMapper _mapper;
    private Product _defaultProduct1, _defaultProduct2;
    
    public ProductControllerMockTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GenericProfile>();
        });
        IMapper mapper = config.CreateMapper();
        _mapper = mapper;
        _produitManager = new Mock<IDataRepository<Product>>();
        _productController = new ProductController(_produitManager.Object, mapper);
    }

    
    [TestInitialize]
    public void Setup()
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
        _defaultProduct1 = new Product()
        {
            IdProduit = 30,
            NomProduit = "Chaise",
            Description = "Une superbe chaise",
            NomPhoto = "Une superbe chaise bleu",
            UriPhoto = "https://ikea.fr/chaise.jpg",
            MarqueNavigation = _defaultMarque,
            TypeProduitNavigation = _defaultProductType
        };
        _defaultProduct2 = new Product()
        {
            NomProduit = "Armoir",
            Description = "Une superbe armoire",
            NomPhoto = "Une superbe armoire jaune",
            UriPhoto = "https://ikea.fr/armoire-jaune.jpg"
        };

    }
    
    [TestMethod]
    public void ShouldGetProduct()
    {
        // Given : Un produit en enregistré
        _produitManager
            .Setup(manager =>  manager.GetByIdAsync(_defaultProduct1.IdProduit))
            .ReturnsAsync(_defaultProduct1);
        
        // When : On appelle la méthode GET de l'API pour récupérer le produit
        ActionResult<ProduitDetailDTO> action = _productController.GetById(_defaultProduct1.IdProduit).GetAwaiter().GetResult();
        
        // Then : On récupère le produit et le code de retour est 200
        _produitManager.Verify(manager => manager.GetByIdAsync(_defaultProduct1.IdProduit), Times.Once);
        
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Value, typeof(ProduitDetailDTO));
        
        ProduitDetailDTO returnProduct = action.Value;
        ProduitDetailDTO productDTO = _mapper.Map<ProduitDetailDTO>(_defaultProduct1);
        Assert.AreEqual(productDTO, returnProduct);
    }

    [TestMethod]
    public void ShouldDeleteProduct()
    {
        // Given : Un produit enregistré
        _produitManager
            .Setup(manager => manager.GetByIdAsync(_defaultProduct1.IdProduit))
            .ReturnsAsync(_defaultProduct1);

        _produitManager
            .Setup(manager => manager.DeleteAsync(_defaultProduct1));

        // When : On souhaite supprimer un produit depuis l'API
        IActionResult action = _productController.DeleteProduit(_defaultProduct1.IdProduit).GetAwaiter().GetResult();
        
        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        _produitManager.Verify(manager => manager.GetByIdAsync(_defaultProduct1.IdProduit), Times.Once);
        _produitManager.Verify(manager => manager.DeleteAsync(_defaultProduct1), Times.Once);
    }
    
    [TestMethod]
    public void ShouldNotDeleteProductBecauseProductDoesNotExist()
    {
        // Given : Un produit enregistré
        _produitManager
            .Setup(manager => manager.GetByIdAsync(_defaultProduct1.IdProduit))
            .ReturnsAsync((Product)null);
        
        // When : On souhaite supprimer un produit depuis l'API
        IActionResult action = _productController.DeleteProduit(_defaultProduct1.IdProduit).GetAwaiter().GetResult();
        
        // Then : Le produit a bien été supprimé et le code HTTP est NO_CONTENT (204)
        _produitManager.Verify(manager => manager.GetByIdAsync(_defaultProduct1.IdProduit), Times.Once);

        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
    }

    [TestMethod]
    public void ShouldGetAllProducts()
    {
        // Given : Des produits enregistrées
        _produitManager
            .Setup(manager => manager.GetAllAsync())
            .ReturnsAsync(new ActionResult<IEnumerable<Product>>( new[] { _defaultProduct1, _defaultProduct2 }));
        
        // When : On souhaite récupérer tous les produits
        var products = _productController.GetAll().GetAwaiter().GetResult();

        // Then : Tous les produits sont récupérés
        Assert.IsNotNull(products);
        Assert.IsInstanceOfType(products.Value, typeof(IEnumerable<ProduitDTO>));
       // Assert.IsTrue(productInDb.SequenceEqual(products.Value));
        
        _produitManager.Verify(manager => manager.GetAllAsync(), Times.Once);
    }

    [TestMethod]
    public void ShouldGetAllProductsByFilter()
    {
        //Given
        _produitManager
            .Setup(manager => manager.FilterAsync("ch", "Ikea", null))
            .ReturnsAsync(new ActionResult<IEnumerable<Product>>( new[] { _defaultProduct1}));
        //When
        var products = _productController.GetAllProductByFilter("ch", "Ikea", null).GetAwaiter().GetResult();
        //Then
        Assert.IsNotNull(products);
        Assert.IsInstanceOfType(products, typeof(ActionResult<IEnumerable<ProduitDTO>>));
        Assert.IsTrue(products.Value.Count() == 1, "Le nombre de produit doit etre 1");
        Assert.IsTrue(products.Value.All(p => p.NomMarque == "Ikea"));
        Assert.IsTrue(products.Value.All(p => p.NomProduit.Contains("ch", StringComparison.OrdinalIgnoreCase)));
        _produitManager.Verify(manager => manager.FilterAsync("ch", "Ikea", null), Times.Once);
    }
    
    [TestMethod]
    public void GetProductShouldReturnNotFound()
    {
        //Given : Pas de produit trouvé par le manager
        _produitManager
            .Setup(manager => manager.GetByIdAsync(30))
            .ReturnsAsync(new ActionResult<Product>((Product)null));
        
        // When : On appelle la méthode get de mon api pour récupérer le produit
        ActionResult<ProduitDetailDTO> action = _productController.GetById(30).GetAwaiter().GetResult();
        
        // Then : On ne renvoie rien et on renvoie NOT_FOUND (404)
        Assert.IsInstanceOfType(action.Result, typeof(NotFoundResult), "Ne renvoie pas 404");
        Assert.IsNull(action.Value, "Le produit n'est pas null");
        
        _produitManager.Verify(manager => manager.GetByIdAsync(30), Times.Once);
    }
    
    [TestMethod]
    public void ShouldCreateProduct()
    {
        // Given : Un produit a enregistré
        Product productToInsert = _defaultProduct1;
        ProduitDetailDTO productToInsertDTO = _mapper.Map<ProduitDetailDTO>(productToInsert);

        _produitManager
            .Setup(manager => manager.AddAsync(productToInsert));
        
        // When : On appel la méthode POST de l'API pour enregistrer le produit
        var action = _productController.AddProduit(productToInsertDTO).GetAwaiter().GetResult();
        
        // Then : Le produit est bien enregistré et le code renvoyé et CREATED (201)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action.Result, typeof(CreatedAtActionResult));
        
        _produitManager.Verify(manager => manager.AddAsync(productToInsert), Times.Once);
    }

    [TestMethod]
    public void ShouldUpdateProduct()
    {
        // Given : Un produit à mettre à jour
        Product productToEdit = _defaultProduct1;
        
        // Une fois enregistré, on modifie certaines propriétés 
        Product updatedProduct = _defaultProduct1;
        updatedProduct.NomProduit = "Table";
        ProduitDetailDTO updatedProduitDTO = _mapper.Map<ProduitDetailDTO>(updatedProduct);

        _produitManager
            .Setup(manager => manager.GetByIdAsync(productToEdit.IdProduit))
            .ReturnsAsync(productToEdit);

        _produitManager
            .Setup(manager => manager.UpdateAsync(productToEdit, updatedProduct));
        
        // When : On appelle la méthode PUT du controller pour mettre à jour le produit
        IActionResult action = _productController.PutProduit(productToEdit.IdProduit,updatedProduitDTO).GetAwaiter().GetResult();
        
        // Then : On vérifie que le produit a bien été modifié et que le code renvoyé et NO_CONTENT (204)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NoContentResult));
        
        _produitManager.Verify(manager => manager.GetByIdAsync(productToEdit.IdProduit), Times.Once);
        _produitManager.Verify(manager => manager.UpdateAsync(productToEdit, It.IsAny<Product>()), Times.Once);
    }
    
    [TestMethod]
    public void ShouldNotUpdateProductBecauseIdInUrlIsDifferent()
    {
        // Given : Un produit à mettre à jour
        Product productToEdit = _defaultProduct1;
        ProduitDetailDTO productToEditDto = _mapper.Map<ProduitDetailDTO>(productToEdit);
        
        
        

        // When : On appelle la méthode PUT du controller pour mettre à jour le produit,
        // mais en précisant un ID différent de celui du produit enregistré
        IActionResult action = _productController.PutProduit(1, productToEditDto).GetAwaiter().GetResult();
        
        // Then : On vérifie que l'API renvoie un code BAD_REQUEST (400)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(BadRequestResult));
        
        _produitManager.Verify(manager => manager.GetByIdAsync((int)productToEdit.GetId()), Times.Never);
        _produitManager.Verify(manager => manager.UpdateAsync(productToEdit, It.IsAny<Product>()), Times.Never);
    }
    
    [TestMethod]
    public void ShouldNotUpdateProductBecauseProductDoesNotExist()
    {
        // Given : Un produit à mettre à jour qui n'est pas enregistré
        Product productToEdit = _defaultProduct1;
        ProduitDetailDTO productToEditDto = _mapper.Map<ProduitDetailDTO>(productToEdit);
        _produitManager
            .Setup(manager => manager.GetByIdAsync((int)productToEditDto.IdProduit))
            .ReturnsAsync((Product)null);
        
        // When : On appelle la méthode PUT du controller pour mettre à jour un produit qui n'est pas enregistré
        IActionResult action = _productController.PutProduit((int)productToEditDto.IdProduit, productToEditDto).GetAwaiter().GetResult();
        
        // Then : On vérifie que l'API renvoie un code NOT_FOUND (404)
        Assert.IsNotNull(action);
        Assert.IsInstanceOfType(action, typeof(NotFoundResult));
        
        _produitManager.Verify(manager => manager.GetByIdAsync((int)productToEditDto.IdProduit), Times.Once);
        _produitManager.Verify(manager => manager.UpdateAsync(productToEdit, It.IsAny<Product>()), Times.Never);
    }
}
