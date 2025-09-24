using Application.Models;

namespace Application.Services;

public class ProductWSService : GenericWSService<Product>
{
    public ProductWSService(HttpClient httpClient) : base(httpClient){}
}