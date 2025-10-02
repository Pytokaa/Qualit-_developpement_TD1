using Application.Models;
using Application.Models.StateService;
using Application.Services;
using Application.Shared;

namespace Application.ViewModel;

public class BaseEntityViewModel<T> : CrudViewModel<T> where T: class, IEntity, new()
{
   public IStateService<T> _ItemToAdd { get; set; }
   private readonly NotificationService _notificationService;
   
   public T? CurrentEntity { get; set; }

   public BaseEntityViewModel(IService<T> service, IStateService<T> itemToAdd, NotificationService notificationService) : base(service)
   {
      _ItemToAdd = itemToAdd;
      _notificationService = notificationService;
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
         var result = await AddAsync(_ItemToAdd.CurrentEntity);
         _notificationService.Show(
            result.Success ? $"{typeof(T).Name} Added" : $"Error while creating the {typeof(T).Name}", 
            result.Success
         );
         _ItemToAdd.CurrentEntity = new T();
      }
   }
   public async Task SubmitUpdateAsync()
   {
      if (CurrentEntity != null)
      {
         var result = await UpdateAsync(CurrentEntity);
         _notificationService.Show(
            result.Success ? $"{typeof(T).Name} Updated" : $"Failed to update {typeof(T).Name}", 
            result.Success
         );
      }
   }
   public async Task SubmitDeleteAsync()
   {
      if (CurrentEntity != null)
      {
         var result = await DeleteAsync((int)CurrentEntity.GetId());
         _notificationService.Show(
            result.Success ? $"{typeof(T).Name} deleted" : $"Error while deleting the {typeof(T).Name}", 
            result.Success
         );
      }
   }
}