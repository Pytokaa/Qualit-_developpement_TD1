namespace Application.Models.StateService;

public interface IStateService<T>
{
    public T CurrentEntity { get; set; } 
}