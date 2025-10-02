namespace Application.Shared;

public class NotificationService
{
    public event Action<string, bool>? OnNotification; 
    
    public void Show(string message, bool isSuccess = true)
    {
        OnNotification?.Invoke(message, isSuccess);
    }
}