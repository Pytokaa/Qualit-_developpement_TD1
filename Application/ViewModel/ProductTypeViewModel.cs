using Application.Models;
using Application.Models.StateService;
using Application.Services;

namespace Application.ViewModel;

public class ProductTypeViewModel : BaseEntityViewModel<TypeProduit>
{
    public ProductTypeViewModel(IService<TypeProduit> service, IStateService<TypeProduit> typeToAdd): base(service, typeToAdd){}
    
}