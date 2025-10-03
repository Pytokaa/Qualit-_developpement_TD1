    using System.Net.Http.Json;
    using Application.Models;

    namespace Application.Services;

    public class ProductWSService : GenericWSService<Product>
    {
        public ProductWSService(HttpClient httpClient) : base(httpClient){}

        public virtual async Task<List<Product>?> GetProductByFilter(string? name = null, string? brandName = null, string? productTypeName = null)
        {
            var queryParams = new List<string>();
        
            if (!string.IsNullOrEmpty(name))
                queryParams.Add($"name={Uri.EscapeDataString(name)}");
        
            if (!string.IsNullOrEmpty(brandName))
                queryParams.Add($"brandName={Uri.EscapeDataString(brandName)}");
        
            if (!string.IsNullOrEmpty(productTypeName))
                queryParams.Add($"productTypeName={Uri.EscapeDataString(productTypeName)}");
        
            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        
            return await _httpClient.GetFromJsonAsync<List<Product>>($"Product/productByFilter{queryString}");
        }
    }