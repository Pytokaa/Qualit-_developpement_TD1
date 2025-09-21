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
        await _httpClient.PostAsJsonAsync<Product>("Produit", produit);
    }

    public async Task DeleteAsync(int id)
    {
        await _httpClient.DeleteAsync($"Produit/{id}");
    }

    public async Task<List<Product>?> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Product>>("Produit");
        
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Product?>($"Produit/{id}");
    }

    public async Task<Product?> GetByNameAsync(string name)
    {
        var response = await _httpClient.PostAsJsonAsync("Produit/", name);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Product>();
    }

    public async Task UpdateAsync(Product updatedEntity)
    {
        await _httpClient.PutAsJsonAsync<Product>($"Produit/{updatedEntity.IdProduit}", updatedEntity);
    }
}