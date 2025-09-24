using Application.Models;

namespace Application.Services;

public class ProductTypeWSService : GenericWSService<TypeProduit>
{
    public ProductTypeWSService(HttpClient httpClient) : base(httpClient){}
}