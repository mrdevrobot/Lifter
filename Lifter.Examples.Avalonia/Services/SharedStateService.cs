namespace Lifter.Examples.Avalonia.Services;

public class SharedStateService
{
    public string CurrentMessage { get; private set; } = "Waiting for the first update from the background service...";

    public event Action? OnChange;

    public void UpdateMessage(string message)
    {
        CurrentMessage = message;
        OnChange?.Invoke();
    }
}
