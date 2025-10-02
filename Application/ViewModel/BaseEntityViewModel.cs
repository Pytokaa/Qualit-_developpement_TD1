using Application.Models;
using Application.Models.StateService;
using Application.Services;

namespace Application.ViewModel;

public class BaseEntityViewModel<T> : CrudViewModel<T> where T: class, IEntity, new()
{
   public IStateService<T> _ItemToAdd { get; set; }
   
   public T? CurrentEntity { get; set; }

   public BaseEntityViewModel(IService<T> service, IStateService<T> itemToAdd) : base(service)
   {
      _ItemToAdd = itemToAdd;
   }

   public override async Task LoadAsync()
   {
      await base.LoadAsync();
      CurrentEntity = Items.FirstOrDefault();
   }

   public void OnSelectorChange(T selectedEntity)
   {
      CurrentEntity = selectedEntity;
      NotifyStateChanged();
   }

   public async Task SubmitAddAsync()
   {
      if (_ItemToAdd.CurrentEntity != null)
      {
         await AddAsync(_ItemToAdd.CurrentEntity);
         _ItemToAdd.CurrentEntity = new T();
      }
   }
   public async Task SubmitUpdateAsync()
   {
      if (CurrentEntity != null)
      {
         await UpdateAsync(CurrentEntity);
      }
   }
   public async Task SubmitDeleteAsync()
   {
      if (CurrentEntity != null)
      {
         await DeleteAsync((int)CurrentEntity.GetId());
      }
   }
}