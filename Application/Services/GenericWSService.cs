using System.Net.Http.Json;
using Application.Models;

namespace Application.Services;

public abstract class GenericWSService<T> : IService<T> where T : class,  IEntity
{
    protected readonly HttpClient  _httpClient;

    public GenericWSService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public virtual async Task<List<T>?> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<T>>($"{typeof(T).Name}");
    }
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<T?>($"{typeof(T).Name}/id/{id}");
    }

    public virtual async Task<T?> AddAsync(T entity)
    {
        var response = await _httpClient.PostAsJsonAsync($"{typeof(T).Name}", entity);
        if (!response.IsSuccessStatusCode)
            return null; 

        var createdProduct = await response.Content.ReadFromJsonAsync<T>();
        return createdProduct;
        
    }

    public virtual async Task UpdateAsync( T updatedEntity)
    {
        await _httpClient.PutAsJsonAsync<T>($"{typeof(T).Name}/id/{updatedEntity.GetId()}", updatedEntity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        await _httpClient.DeleteAsync($"{typeof(T).Name}/id/{id}");
    }

    public virtual  async Task<T?> GetByNameAsync(string name)
    {
        return await _httpClient.GetFromJsonAsync<T>($"{typeof(T).Name}/name/{name}");
    }
}