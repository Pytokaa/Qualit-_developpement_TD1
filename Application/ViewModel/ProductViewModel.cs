using Application.Components.Pages;
using Application.Models;
using Application.Services;

namespace Application.ViewModel;

public class ProductViewModel
{
    private readonly ProductWSService  _webService;
    public string? SearchName { get; set; }
    public string? SearchBrand { get; set; }
    public string? SearchType { get; set; }

    public ProductViewModel(ProductWSService service)
    {
        _webService = service;
    }
    public  Product? productGet { get; set; }
    
    public IEnumerable<Product> products { get; set; } = new List<Product>();
    
    public bool IsLoading { get; set; }
    
    public string ErrorMessage { get; set; }

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
    
}