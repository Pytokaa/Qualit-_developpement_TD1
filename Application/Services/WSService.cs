using System.Net.Http.Json;
using Application.Models;
namespace Application.Services;

public class WSService : IService<Product>
{
    private readonly HttpClient  _httpClient;

    public WSService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AddAsync(Product produit)
    {
        await _httpClient.PostAsJsonAsync<Product>("Product", produit);
    }

    public async Task DeleteAsync(int id)
    {
        await _httpClient.DeleteAsync($"Product/{id}");
    }

    public async Task<List<Product>?> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Product>>("Product");
        
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Product?>($"Product/{id}");
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        var response = await _httpClient.PostAsJsonAsync("Product/", name);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task UpdateAsync(Product updatedEntity)
    {
        await _httpClient.PutAsJsonAsync<Product>($"Product/{updatedEntity.IdProduit}", updatedEntity);
    }
}