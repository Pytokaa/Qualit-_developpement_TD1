namespace Application.Models.StateService;

public class ProductTypeStateService : IStateService<TypeProduit>
{
    public TypeProduit CurrentEntity { get; set; } = new TypeProduit();
}