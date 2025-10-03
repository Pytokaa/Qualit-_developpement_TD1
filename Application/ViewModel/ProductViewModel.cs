using System.Xml;
using Application.Components.Pages;
using Application.Models;
using Application.Models.StateService;
using Application.Services;
using Application.Shared;
using Microsoft.AspNetCore.Components;

namespace Application.ViewModel;

public class ProductViewModel : CrudViewModel<Product>
{
    private readonly ProductWSService _webService;
    public IStateService<Product> _productToAdd;
    
    public string? SearchName { get; set; }
    private readonly NotificationService _notificationService;
    public string? SearchBrand { get; set; }
    public string? SearchType { get; set; }
    private CancellationTokenSource searchCts;

    public ProductViewModel(IService<Product> service, IStateService<Product> productToAdd, ProductWSService webService, NotificationService notificationService)
        : base(service)
    {
        _webService = webService;
        _productToAdd = productToAdd;
        _notificationService = notificationService;
    }
    
    public Product? CurrentEditingProduct { get; set; }

    public override async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();
        try
        {
            await base.LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task ApplyFilterAsync()
    {
        IsLoading = true;
        NotifyStateChanged();

        try
        {
            var data = await _webService.GetProductByFilter(SearchName, SearchBrand, SearchType);
            Items = data?.ToList() ?? new List<Product>();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }
    public async Task SetCurrentEditingProduct(int id)
    {
        CurrentEditingProduct = await _webService.GetByIdAsync(id);
    }

    public async Task<(bool Success, string? ErrorMessage)> AddProductAsync()
    {
        if (_productToAdd.CurrentEntity != null)
        {
            var result = await AddAsync(_productToAdd.CurrentEntity);
            _notificationService.Show(
                result.Success ? "Product Added" : "Error while creating the product", 
                result.Success
            );
            
            if (result.Success)
            {
                _productToAdd.CurrentEntity = new Product();
            }
            return result;
        }
        return (false, "No product to add");
    }
    public async Task UpdateCurrentEditingProduct()
    {
        if (CurrentEditingProduct != null)
        {
            var result = await UpdateAsync(CurrentEditingProduct);
            _notificationService.Show(
                result.Success ? "Product Updated" : "Failed to update product", 
                result.Success
            );
        }
    }

    public async Task DeleteCurrentEditingProduct()
    {
        if (CurrentEditingProduct != null)
        {
            var result = await DeleteAsync(CurrentEditingProduct.IdProduit);
            
            _notificationService.Show(
                result.Success ? "Product deleted" : "Error while deleting the product", 
                result.Success
            );
        }
    }
    public async Task OnSearchInputAsync(string value)
    {
        SearchName = value;
        searchCts?.Cancel();
        searchCts = new CancellationTokenSource();
        var token = searchCts.Token;

        try
        {
            await Task.Delay(500, token);
            await ApplyFilterAsync();
        }
        catch (TaskCanceledException) { }

        NotifyStateChanged(); 
    }
   
}