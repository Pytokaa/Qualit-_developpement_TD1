using Application.Models;
using Application.Models.StateService;
using Application.Services;
using Application.Shared;

namespace Application.ViewModel;

public class BrandViewModel : BaseEntityViewModel<Marque>
{
    public BrandViewModel(IService<Marque> service, IStateService<Marque> brandToAdd, NotificationService notificationService) : base(service,  brandToAdd,  notificationService){}
}