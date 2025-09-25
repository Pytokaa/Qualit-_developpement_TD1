using System.Net.Http.Json;
using Application.Models;

namespace Application.Services;

public abstract class GenericWSService<T> : IService<T> where T : class,  IEntity
{
    private readonly HttpClient  _httpClient;

    public GenericWSService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public async Task<List<T>?> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<T>>($"{typeof(T).Name}/GetAll");
    }
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<T?>($"{typeof(T).Name}/GetById/id/{id}");
    }

    public async Task AddAsync(T entity)
    {
        await _httpClient.PostAsJsonAsync($"{typeof(T).Name}/Add", entity);
    }

    public async Task UpdateAsync( T updatedEntity)
    {
        await _httpClient.PutAsJsonAsync<T>($"{typeof(T).Name}/Put/id/{updatedEntity.GetId()}", updatedEntity);
    }

    public async Task DeleteAsync(int id)
    {
        await _httpClient.DeleteAsync($"{typeof(T).Name}/Delete/id/{id}");
    }

    public async Task<T?> GetByNameAsync(string name)
    {
        return await _httpClient.GetFromJsonAsync<T>($"{typeof(T).Name}/GetByName/name/{name}");
    }
}