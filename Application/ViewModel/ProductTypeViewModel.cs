using Application.Models;
using Application.Models.StateService;
using Application.Services;
using Application.Shared;

namespace Application.ViewModel;

public class ProductTypeViewModel : BaseEntityViewModel<TypeProduit>
{
    public ProductTypeViewModel(IService<TypeProduit> service, IStateService<TypeProduit> typeToAdd, NotificationService notificationService): base(service, typeToAdd, notificationService){}
    
}