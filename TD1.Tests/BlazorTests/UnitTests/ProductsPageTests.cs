using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bunit;
using Application.Components.Pages;
using Application.ViewModel;
using Application.Models;
using Application.Services;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Models.StateService;
using Application.Shared;

namespace BlazorUnitTests
{
    [TestClass]
    public class ProductsPageTests
    {
        private Bunit.TestContext ctx;
        private Mock<IService<Product>> mockService;
        private Mock<ProductWSService> mockWebService;
        private Mock<IStateService<Product>> mockStateService;
        private Mock<NotificationService> mockNotificationService;
        private Mock<IService<Marque>> mockBrandService;
        private Mock<IService<TypeProduit>> mockProductTypeService;
        private Mock<IStateService<Marque>> mockBrandStateService;
        private Mock<IStateService<TypeProduit>> mockProductTypeStateService;

        [TestInitialize]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            mockService = new Mock<IService<Product>>();
            mockWebService = new Mock<ProductWSService>(MockBehavior.Loose, new object[] { null });
            mockStateService = new Mock<IStateService<Product>>();
            mockNotificationService = new Mock<NotificationService>(MockBehavior.Loose);
            mockBrandService = new Mock<IService<Marque>>();
            mockProductTypeService = new Mock<IService<TypeProduit>>();
            mockBrandStateService = new Mock<IStateService<Marque>>();
            mockProductTypeStateService = new Mock<IStateService<TypeProduit>>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            ctx?.Dispose();
        }

        [TestMethod]
        public void ProductViewModel_RendersNoProducts_WhenListIsEmpty()
        {
            // Arrange
            var vm = CreateProductViewModel();

            // Act & Assert
            Assert.AreEqual(0, vm.Items.Count);
        }

        [TestMethod]
        public void ProductViewModel_ContainsProducts_WhenItemsAdded()
        {
            // Arrange
            var vm = CreateProductViewModel();
            var products = new List<Product>
            {
                new Product 
                { 
                    IdProduit = 1, 
                    NomProduit = "Test Product", 
                    NomMarque = "Brand1", 
                    NomTypeProduit = "Type1", 
                    StockReel = 5, 
                    StockMin = 2
                }
            };

            // Act
            typeof(CrudViewModel<Product>)
                .GetProperty("Items")
                .SetValue(vm, products);

            // Assert
            Assert.AreEqual(1, vm.Items.Count);
            Assert.AreEqual("Test Product", vm.Items[0].NomProduit);
        }

        [TestMethod]
        public async Task OnSearchInputAsync_UpdatesSearchNameAndCallsFilter()
        {
            // Arrange
            var expectedProducts = new List<Product> 
            { 
                new Product { IdProduit = 1, NomProduit = "Filtered Product" } 
            };

            mockWebService
                .Setup(s => s.GetProductByFilter(It.IsAny<string>(), null, null))
                .ReturnsAsync(expectedProducts);

            var vm = CreateProductViewModel();

            // Act
            await vm.OnSearchInputAsync("Filtered");
            await Task.Delay(600);

            // Assert
            Assert.AreEqual("Filtered", vm.SearchName);
            Assert.AreEqual(1, vm.Items.Count);
            Assert.AreEqual("Filtered Product", vm.Items[0].NomProduit);
            mockWebService.Verify(s => s.GetProductByFilter("Filtered", null, null), Times.Once);
        }

        [TestMethod]
        public async Task ApplyFilterAsync_FiltersProductsByName()
        {
            // Arrange
            var filteredProducts = new List<Product>
            {
                new Product { IdProduit = 1, NomProduit = "Product A" }
            };

            mockWebService
                .Setup(s => s.GetProductByFilter("Product A", null, null))
                .ReturnsAsync(filteredProducts);

            var vm = CreateProductViewModel();
            vm.SearchName = "Product A";

            // Act
            await vm.ApplyFilterAsync();

            // Assert
            Assert.AreEqual(1, vm.Items.Count);
            Assert.AreEqual("Product A", vm.Items[0].NomProduit);
        }

        [TestMethod]
        public async Task ApplyFilterAsync_FiltersProductsByBrandAndType()
        {
            // Arrange
            var filteredProducts = new List<Product>
            {
                new Product 
                { 
                    IdProduit = 1, 
                    NomProduit = "Product X", 
                    NomMarque = "Brand1",
                    NomTypeProduit = "Type1"
                }
            };

            mockWebService
                .Setup(s => s.GetProductByFilter(null, "Brand1", "Type1"))
                .ReturnsAsync(filteredProducts);

            var vm = CreateProductViewModel();
            vm.SearchBrand = "Brand1";
            vm.SearchType = "Type1";

            // Act
            await vm.ApplyFilterAsync();

            // Assert
            Assert.AreEqual(1, vm.Items.Count);
            Assert.AreEqual("Brand1", vm.Items[0].NomMarque);
            Assert.AreEqual("Type1", vm.Items[0].NomTypeProduit);
        }
        

        [TestMethod]
        public async Task AddProductAsync_ReturnsError_WhenNoProductToAdd()
        {
            // Arrange
            mockStateService.Setup(s => s.CurrentEntity).Returns((Product)null);
            var vm = CreateProductViewModel();

            // Act
            var result = await vm.AddProductAsync();

            // Assert
            Assert.IsFalse(result.Success);
            Assert.AreEqual("No product to add", result.ErrorMessage);
        }

        [TestMethod]
        public async Task UpdateCurrentEditingProduct_UpdatesProductSuccessfully()
        {
            // Arrange
            var product = new Product 
            { 
                IdProduit = 1, 
                NomProduit = "Updated Product" 
            };

            mockWebService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);
            mockService.Setup(s => s.UpdateAsync(It.IsAny<Product>()))
                .Callback<Product>(p => { })
                .Returns(Task.CompletedTask);
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Product> { product });

            var vm = CreateProductViewModel();
            await vm.SetCurrentEditingProduct(1);

            // Act
            await vm.UpdateCurrentEditingProduct();

            // Assert
            mockService.Verify(s => s.UpdateAsync(product), Times.Once);
            mockNotificationService.Verify(n => n.Show("Product Updated", true), Times.Once);
        }

        [TestMethod]
        public async Task DeleteCurrentEditingProduct_DeletesProductSuccessfully()
        {
            // Arrange
            var product = new Product 
            { 
                IdProduit = 1, 
                NomProduit = "Product to Delete" 
            };

            mockWebService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);
            mockService.Setup(s => s.DeleteAsync(1))
                .Callback<int>(id => { })
                .Returns(Task.CompletedTask);
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Product>());

            var vm = CreateProductViewModel();
            await vm.SetCurrentEditingProduct(1);

            // Act
            await vm.DeleteCurrentEditingProduct();

            // Assert
            mockService.Verify(s => s.DeleteAsync(1), Times.Once);
            mockNotificationService.Verify(n => n.Show("Product deleted", true), Times.Once);
        }

        [TestMethod]
        public async Task LoadAsync_HandlesExceptionGracefully()
        {
            // Arrange
            mockService
                .Setup(s => s.GetAllAsync())
                .ThrowsAsync(new Exception("Database error"));

            var vm = CreateProductViewModel();

            // Act
            await vm.LoadAsync();

            // Assert
            Assert.IsFalse(vm.IsLoading);
            Assert.IsNotNull(vm.ErrorMessage);
            Assert.AreEqual("Database error", vm.ErrorMessage);
        }

        [TestMethod]
        public async Task OnSearchInputAsync_CancelsPreviousSearch()
        {
            // Arrange
            var callCount = 0;
            mockWebService
                .Setup(s => s.GetProductByFilter(It.IsAny<string>(), null, null))
                .Callback(() => callCount++)
                .ReturnsAsync(new List<Product>());

            var vm = CreateProductViewModel();

            // Act
            var task1 = vm.OnSearchInputAsync("A");
            var task2 = vm.OnSearchInputAsync("AB");
            var task3 = vm.OnSearchInputAsync("ABC");
            await Task.Delay(600);

            // Assert
            Assert.AreEqual(1, callCount);
            mockWebService.Verify(s => s.GetProductByFilter("ABC", null, null), Times.Once);
        }

        [TestMethod]
        public async Task ApplyFilterAsync_SetsIsLoadingCorrectly()
        {
            // Arrange
            var tcs = new TaskCompletionSource<List<Product>>();
            mockWebService
                .Setup(s => s.GetProductByFilter(null, null, null))
                .Returns(tcs.Task);

            var vm = CreateProductViewModel();

            // Act
            var filterTask = vm.ApplyFilterAsync();
            Assert.IsTrue(vm.IsLoading);

            tcs.SetResult(new List<Product>());
            await filterTask;

            // Assert
            Assert.IsFalse(vm.IsLoading);
        }

        [TestMethod]
        public async Task SetCurrentEditingProduct_LoadsProductCorrectly()
        {
            // Arrange
            var product = new Product 
            { 
                IdProduit = 5, 
                NomProduit = "Editing Product" 
            };

            mockWebService.Setup(s => s.GetByIdAsync(5)).ReturnsAsync(product);
            var vm = CreateProductViewModel();

            // Act
            await vm.SetCurrentEditingProduct(5);

            // Assert
            Assert.IsNotNull(vm.CurrentEditingProduct);
            Assert.AreEqual(5, vm.CurrentEditingProduct.IdProduit);
            Assert.AreEqual("Editing Product", vm.CurrentEditingProduct.NomProduit);
        }

        private ProductViewModel CreateProductViewModel()
        {
            return new ProductViewModel(
                service: mockService.Object,
                productToAdd: mockStateService.Object,
                webService: mockWebService.Object,
                notificationService: mockNotificationService.Object
            );
        }
    }

    [TestClass]
    public class BaseEntityViewModelTests
    {
        private Mock<IService<Marque>> mockService;
        private Mock<IStateService<Marque>> mockStateService;
        private Mock<NotificationService> mockNotificationService;

        [TestInitialize]
        public void Setup()
        {
            mockService = new Mock<IService<Marque>>();
            mockStateService = new Mock<IStateService<Marque>>();
            mockNotificationService = new Mock<NotificationService>(MockBehavior.Loose);
        }

        [TestMethod]
        public async Task LoadAsync_SetsCurrentEntityToFirst()
        {
            // Arrange
            var marques = new List<Marque>
            {
                new Marque { IdMarque = 1, NomMarque = "Marque1" },
                new Marque { IdMarque = 2, NomMarque = "Marque2" }
            };

            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(marques);
            var vm = CreateBaseEntityViewModel();

            // Act
            await vm.LoadAsync();

            // Assert
            Assert.IsNotNull(vm.CurrentEntity);
            Assert.AreEqual(1, vm.CurrentEntity.IdMarque);
            Assert.AreEqual("Marque1", vm.CurrentEntity.NomMarque);
        }

        [TestMethod]
        public void OnSelectorChange_UpdatesCurrentEntity()
        {
            // Arrange
            var vm = CreateBaseEntityViewModel();
            var newMarque = new Marque { IdMarque = 3, NomMarque = "Marque3" };

            // Act
            vm.OnSelectorChange(newMarque);

            // Assert
            Assert.IsNotNull(vm.CurrentEntity);
            Assert.AreEqual(3, vm.CurrentEntity.IdMarque);
            Assert.AreEqual("Marque3", vm.CurrentEntity.NomMarque);
        }

      

        [TestMethod]
        public async Task SubmitUpdateAsync_UpdatesEntitySuccessfully()
        {
            // Arrange
            var marque = new Marque { IdMarque = 1, NomMarque = "Updated Marque" };
            mockService.Setup(s => s.UpdateAsync(It.IsAny<Marque>()))
                .Callback<Marque>(m => { })
                .Returns(Task.CompletedTask);
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Marque> { marque });

            var vm = CreateBaseEntityViewModel();
            vm.CurrentEntity = marque;

            // Act
            await vm.SubmitUpdateAsync();

            // Assert
            mockService.Verify(s => s.UpdateAsync(marque), Times.Once);
            mockNotificationService.Verify(n => n.Show("Marque Updated", true), Times.Once);
        }

        [TestMethod]
        public async Task SubmitDeleteAsync_DeletesEntitySuccessfully()
        {
            // Arrange
            var marque = new Marque { IdMarque = 1, NomMarque = "Marque to Delete" };
            mockService.Setup(s => s.DeleteAsync(1))
                .Callback<int>(id => { })
                .Returns(Task.CompletedTask);
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Marque>());

            var vm = CreateBaseEntityViewModel();
            vm.CurrentEntity = marque;

            // Act
            await vm.SubmitDeleteAsync();

            // Assert
            mockService.Verify(s => s.DeleteAsync(1), Times.Once);
            mockNotificationService.Verify(n => n.Show("Marque deleted", true), Times.Once);
        }

        [TestMethod]
        public async Task SubmitAddAsync_DoesNothing_WhenCurrentEntityIsNull()
        {
            // Arrange
            mockStateService.Setup(s => s.CurrentEntity).Returns((Marque)null);
            var vm = CreateBaseEntityViewModel();

            // Act
            await vm.SubmitAddAsync();

            // Assert
            mockService.Verify(s => s.AddAsync(It.IsAny<Marque>()), Times.Never);
        }

        [TestMethod]
        public async Task SubmitUpdateAsync_DoesNothing_WhenCurrentEntityIsNull()
        {
            // Arrange
            var vm = CreateBaseEntityViewModel();
            vm.CurrentEntity = null;

            // Act
            await vm.SubmitUpdateAsync();

            // Assert
            mockService.Verify(s => s.UpdateAsync(It.IsAny<Marque>()), Times.Never);
        }

        [TestMethod]
        public async Task SubmitDeleteAsync_DoesNothing_WhenCurrentEntityIsNull()
        {
            // Arrange
            var vm = CreateBaseEntityViewModel();
            vm.CurrentEntity = null;

            // Act
            await vm.SubmitDeleteAsync();

            // Assert
            mockService.Verify(s => s.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public async Task LoadAsync_SetsCurrentEntityToNull_WhenNoItems()
        {
            // Arrange
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Marque>());
            var vm = CreateBaseEntityViewModel();

            // Act
            await vm.LoadAsync();

            // Assert
            Assert.IsNull(vm.CurrentEntity);
        }

        [TestMethod]
        public void BaseEntityViewModel_ItemToAddProperty_IsAccessible()
        {
            // Arrange & Act
            var vm = CreateBaseEntityViewModel();

            // Assert
            Assert.IsNotNull(vm._ItemToAdd);
            Assert.AreSame(mockStateService.Object, vm._ItemToAdd);
        }


        
        
        
        
        
        
        
        
        
        

        private BaseEntityViewModel<Marque> CreateBaseEntityViewModel()
        {
            return new BaseEntityViewModel<Marque>(
                service: mockService.Object,
                itemToAdd: mockStateService.Object,
                notificationService: mockNotificationService.Object
            );
        }
    }
}