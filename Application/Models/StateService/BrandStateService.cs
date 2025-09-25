namespace Application.Models.StateService;

public class BrandStateService : IStateService<Marque>
{
    public Marque CurrentEntity { get; set; } = new Marque();
}