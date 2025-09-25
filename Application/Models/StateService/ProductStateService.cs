namespace Application.Models.StateService;

public class ProductStateService : IStateService<Product>
{
    public Product CurrentEntity { get; set; } = new Product();
}