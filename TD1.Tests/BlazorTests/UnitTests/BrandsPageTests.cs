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
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorUnitTests
{
    [TestClass]
    public class BrandsPageTests
    {
        private Bunit.TestContext ctx;
        private Mock<IService<Marque>> mockService;
        private Mock<IStateService<Marque>> mockStateService;
        private Mock<NotificationService> mockNotificationService;

        [TestInitialize]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            mockService = new Mock<IService<Marque>>();
            mockStateService = new Mock<IStateService<Marque>>();
            mockNotificationService = new Mock<NotificationService>(MockBehavior.Loose);
        }

        [TestCleanup]
        public void Cleanup() => ctx?.Dispose();

        [TestMethod]
        public void BrandsPage_RendersCorrectly()
        {
            var brands = new List<Marque>
            {
                new Marque { IdMarque = 1, NomMarque = "Nike" },
                new Marque { IdMarque = 2, NomMarque = "Adidas" }
            };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            ctx.Services.AddSingleton(vm);
            var cut = ctx.RenderComponent<Brands>();
            Assert.IsTrue(cut.Markup.Contains("Brands"));
        }

        [TestMethod]
        public void BrandsPage_DisplaysLoadingState()
        {
            var tcs = new TaskCompletionSource<List<Marque>>();
            mockService.Setup(s => s.GetAllAsync()).Returns(tcs.Task);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            vm.IsLoading = true;
            ctx.Services.AddSingleton(vm);
            var cut = ctx.RenderComponent<Brands>();
            Assert.IsTrue(cut.Markup.Contains("Loading..."));
        }

        [TestMethod]
        public async Task BrandsPage_DisplaysBrandList()
        {
            var brands = new List<Marque>
            {
                new Marque { IdMarque = 1, NomMarque = "Nike" },
                new Marque { IdMarque = 2, NomMarque = "Adidas" },
                new Marque { IdMarque = 3, NomMarque = "Puma" }
            };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            ctx.Services.AddSingleton(vm);
            var cut = ctx.RenderComponent<Brands>();
            var select = cut.Find("select");
            Assert.IsNotNull(select);
            var options = cut.FindAll("option");
            Assert.AreEqual(3, options.Count);
            Assert.IsTrue(cut.Markup.Contains("Nike"));
            Assert.IsTrue(cut.Markup.Contains("Adidas"));
            Assert.IsTrue(cut.Markup.Contains("Puma"));
        }

        [TestMethod]
        public async Task BrandsPage_SelectsFirstBrandByDefault()
        {
            var brands = new List<Marque>
            {
                new Marque { IdMarque = 1, NomMarque = "Nike" },
                new Marque { IdMarque = 2, NomMarque = "Adidas" }
            };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            ctx.Services.AddSingleton(vm);
            var cut = ctx.RenderComponent<Brands>();
            Assert.IsNotNull(vm.CurrentEntity);
            Assert.AreEqual("Nike", vm.CurrentEntity.NomMarque);
        }

        [TestMethod]
        public async Task BrandsPage_UpdateForm_DisplaysCurrentEntity()
        {
            var brands = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "Nike" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            ctx.Services.AddSingleton(vm);
            var cut = ctx.RenderComponent<Brands>();
            Assert.IsTrue(cut.Markup.Contains("Update"));
            var inputs = cut.FindAll("input[type='text']");
            Assert.IsTrue(inputs.Count > 0);
        }

        [TestMethod]
        public async Task BrandsPage_AddForm_IsDisplayed()
        {
            var brands = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "Nike" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            ctx.Services.AddSingleton(vm);
            var cut = ctx.RenderComponent<Brands>();
            Assert.IsTrue(cut.Markup.Contains("Add a"));
            var forms = cut.FindAll("form");
            Assert.IsTrue(forms.Count >= 2);
        }

        [TestMethod]
        public async Task BrandsPage_DeleteButton_IsDisplayed()
        {
            var brands = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "Nike" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            ctx.Services.AddSingleton(vm);
            var cut = ctx.RenderComponent<Brands>();
            Assert.IsTrue(cut.Markup.Contains("Delete"));
            var deleteDiv = cut.Find("#deleteItemdiv");
            Assert.IsNotNull(deleteDiv);
        }

        [TestMethod]
        public async Task BrandsPage_HandlesEmptyBrandList()
        {
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Marque>());
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            ctx.Services.AddSingleton(vm);
            var cut = ctx.RenderComponent<Brands>();
            Assert.IsNull(vm.CurrentEntity);
            var select = cut.Find("select");
            var options = cut.FindAll("option");
            Assert.AreEqual(0, options.Count);
        }

        [TestMethod]
        public async Task BrandsPage_TitleIsCorrect()
        {
            var brands = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "Nike" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            ctx.Services.AddSingleton(vm);
            var cut = ctx.RenderComponent<Brands>();
            var h3 = cut.Find("h3");
            Assert.IsNotNull(h3);
            Assert.AreEqual("Brands", h3.TextContent);
        }

        private BaseEntityViewModel<Marque> CreateBrandViewModel()
        {
            return new BaseEntityViewModel<Marque>(
                service: mockService.Object,
                itemToAdd: mockStateService.Object,
                notificationService: mockNotificationService.Object
            );
        }
    }

    [TestClass]
    public class TableTemplateTests
    {
        private Bunit.TestContext ctx;
        private Mock<IService<Marque>> mockService;
        private Mock<IStateService<Marque>> mockStateService;
        private Mock<NotificationService> mockNotificationService;

        [TestInitialize]
        public void Setup()
        {
            ctx = new Bunit.TestContext();
            mockService = new Mock<IService<Marque>>();
            mockStateService = new Mock<IStateService<Marque>>();
            mockNotificationService = new Mock<NotificationService>(MockBehavior.Loose);
        }

        [TestCleanup]
        public void Cleanup() => ctx?.Dispose();

        [TestMethod]
        public async Task TableTemplate_RendersWithCustomTitle()
        {
            var brands = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "TestBrand" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            var cut = ctx.RenderComponent<Application.Components.Layout.TableTemplate<Marque>>(parameters => parameters
                .Add(p => p.Title, "Custom Title")
                .Add(p => p._ViewModel, vm)
            );
            Assert.IsTrue(cut.Markup.Contains("Custom Title"));
        }

        [TestMethod]
        public async Task TableTemplate_CallsLoadAsyncOnInitialization()
        {
            var brands = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "TestBrand" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            var cut = ctx.RenderComponent<Application.Components.Layout.TableTemplate<Marque>>(parameters => parameters
                .Add(p => p.Title, "Test")
                .Add(p => p._ViewModel, vm)
            );
            await Task.Delay(100);
            mockService.Verify(s => s.GetAllAsync(), Times.AtLeastOnce);
        }
        
        [TestMethod]
        public async Task TableTemplate_IgnoresPropertiesWithIgnoreAttribute()
        {
            var brands = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "TestBrand" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            var cut = ctx.RenderComponent<Application.Components.Layout.TableTemplate<Marque>>(parameters => parameters
                .Add(p => p.Title, "Brands")
                .Add(p => p._ViewModel, vm)
            );
            var labels = cut.FindAll("label");
            var hasIdLabel = labels.Any(l => l.TextContent.Contains("IdMarque"));
            Assert.IsFalse(hasIdLabel);
        }

        [TestMethod]
        public async Task TableTemplate_DisplaysUpdateButton()
        {
            var brands = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "TestBrand" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            var cut = ctx.RenderComponent<Application.Components.Layout.TableTemplate<Marque>>(parameters => parameters
                .Add(p => p.Title, "Brands")
                .Add(p => p._ViewModel, vm)
            );
            var updateButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Update"));
            Assert.IsNotNull(updateButton);
        }

        [TestMethod]
        public async Task TableTemplate_DisplaysAddButton()
        {
            var brands = new List<Marque> { new Marque { IdMarque = 1, NomMarque = "TestBrand" } };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(brands);
            mockStateService.Setup(s => s.CurrentEntity).Returns(new Marque());
            var vm = CreateBrandViewModel();
            await vm.LoadAsync();
            var cut = ctx.RenderComponent<Application.Components.Layout.TableTemplate<Marque>>(parameters => parameters
                .Add(p => p.Title, "Brands")
                .Add(p => p._ViewModel, vm)
            );
            var addButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Add"));
            Assert.IsNotNull(addButton);
        }

        private BaseEntityViewModel<Marque> CreateBrandViewModel()
        {
            return new BaseEntityViewModel<Marque>(
                service: mockService.Object,
                itemToAdd: mockStateService.Object,
                notificationService: mockNotificationService.Object
            );
        }
    }
}