using Application.Models;

namespace Application.Services;

public class BrandWSService : GenericWSService<Marque>
{
    public BrandWSService(HttpClient httpClient) : base(httpClient){}
}