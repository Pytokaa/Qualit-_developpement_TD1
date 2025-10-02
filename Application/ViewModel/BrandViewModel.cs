using Application.Models;
using Application.Models.StateService;
using Application.Services;

namespace Application.ViewModel;

public class BrandViewModel : BaseEntityViewModel<Marque>
{
    public BrandViewModel(IService<Marque> service, IStateService<Marque> brandToAdd) : base(service,  brandToAdd){}
}