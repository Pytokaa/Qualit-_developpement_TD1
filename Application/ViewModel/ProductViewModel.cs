using System.Xml;
using Application.Components.Pages;
using Application.Models;
using Application.Models.StateService;
using Application.Services;
using Microsoft.AspNetCore.Components;

namespace Application.ViewModel;

public class ProductViewModel
{
    private readonly ProductWSService  _webService;
    public IStateService<Product> _productToAdd;
    public string? SearchName { get; set; }
    public string? SearchBrand { get; set; }
    public string? SearchType { get; set; }
    private CancellationTokenSource? searchCts;
    public event Action? OnChange;
    


    public ProductViewModel(ProductWSService service, IStateService<Product> productToAdd)
    {
        _webService = service;
        _productToAdd = productToAdd;
    }
    public  Product? productGet { get; set; }
    
    public IEnumerable<Product> products { get; set; } = new List<Product>();
    
    public bool IsLoading { get; set; }
    
    public string ErrorMessage { get; set; }
    
    private void NotifyStateChanged() => OnChange?.Invoke();

    public async Task LoadProductAsync()
    {
        var data = await _webService.GetAllAsync();
        if (data != null)
        {
            products = new List<Product>(data);
        }
    }

    public async Task FilterProductsAsync()
    {
        var data = await _webService.GetProductByFilter(SearchName, SearchBrand, SearchType);
        if (data != null)
        {
            products = new List<Product>(data);
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
            await ApplyFiltersAsync();
        }
        catch (TaskCanceledException) { }

        NotifyStateChanged(); 
    }

    public async Task ApplyFiltersAsync()
    {
        List<Product>? data =  null;
        IsLoading = true;
        try
        {
            data = await _webService.GetProductByFilter(SearchName, SearchBrand, SearchType);
        }
        catch (TaskCanceledException)
        {
        }
        finally
        {
            products = data ?? new List<Product>();
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task<(bool Succes, string? ErrorMessage)> AddProductAsync()
    {
        try
        {
            var response = await _webService.AddAsync(_productToAdd.CurrentEntity);
            if (response != null)
            {
                _productToAdd.CurrentEntity = new Product();
                return (true, null);
            }
            else
            {
                return (false, "An error occured");
            }
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }
    public Product _currentEditingProduct { get; set; }

    public async Task SetCurrentEditingProduct(int id)
    {
        _currentEditingProduct = await _webService.GetByIdAsync(id);
    }

    public async Task UpdateCurrentEditingProduct()
    {
        if (_currentEditingProduct != null)
        {
            _webService.UpdateAsync(_currentEditingProduct);
        }
    }

    public async Task DeleteCurrentEditingProduct()
    {
        if (_currentEditingProduct != null)
        {
            _webService.DeleteAsync(_currentEditingProduct.IdProduit);
        }
    }

}