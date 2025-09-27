using System.Net.Http.Json;
using Application.Models;

namespace Application.Services;

public class ProductWSService : GenericWSService<Product>
{
    public ProductWSService(HttpClient httpClient) : base(httpClient){}

    public async Task<List<Product>?> GetProductByBrandName(string brandName)
    {
        return await _httpClient.GetFromJsonAsync<List<Product>>($"Product/brand/{brandName}");
    }
    public async Task<List<Product>?> GetProductByType(string productTypeName)
    {
        return await _httpClient.GetFromJsonAsync<List<Product>>($"Product/productType/{productTypeName}");
    }

    public async Task<List<Product>?> GetProductListByName(string name)
    {
        return await _httpClient.GetFromJsonAsync<List<Product>?>($"Product/productListByName/{name}");
    }
}