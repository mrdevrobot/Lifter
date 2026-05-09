using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Lifter.Avalonia;
using Lifter.Core.Dialog;
using Lifter.Examples.Avalonia.Services;

namespace Lifter.Examples.Avalonia.Views;

public class MainView : UserControl, IHostedView
{
    private readonly TextBlock _messageText;
    private readonly TextBlock _dialogResultText;
    private readonly SharedStateService _sharedState;
    private readonly IDialogService _dialogService;

    public MainView(SharedStateService sharedState, IDialogService dialogService)
    {
        _sharedState = sharedState;
        _dialogService = dialogService;

        _messageText = new TextBlock
        {
            Text = _sharedState.CurrentMessage,
            FontSize = 16,
            HorizontalAlignment = HorizontalAlignment.Center,
            TextWrapping = TextWrapping.Wrap,
        };

        _dialogResultText = new TextBlock
        {
            Text = "No dialog opened yet.",
            FontSize = 14,
            HorizontalAlignment = HorizontalAlignment.Center,
            Opacity = 0.7,
            TextWrapping = TextWrapping.Wrap,
        };

        Content = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Spacing = 24,
            Margin = new Thickness(40),
            Children =
            {
                new TextBlock
                {
                    Text = "Lifter Avalonia Example",
                    FontSize = 28,
                    FontWeight = FontWeight.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
                new TextBlock
                {
                    Text = "A background IHostedService updates the message below every second.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Opacity = 0.6,
                    TextWrapping = TextWrapping.Wrap,
                },
                _messageText,
                BuildDialogSection(),
                _dialogResultText,
            }
        };

        _sharedState.OnChange += OnStateChanged;
    }

    private Control BuildDialogSection()
    {
        Button MakeButton(string text, Func<Task> onClick)
        {
            var btn = new Button
            {
                Content = text,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            btn.Click += async (_, _) => await onClick();
            return btn;
        }

        return new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock
                {
                    Text = "IDialogService demo",
                    FontSize = 16,
                    FontWeight = FontWeight.SemiBold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                },
                MakeButton("Show message dialog", ShowMessageAsync),
                MakeButton("Show confirmation (OK/Cancel)", ShowConfirmAsync),
                MakeButton("Show Yes/No dialog", ShowYesNoAsync),
                MakeButton("Show custom content dialog", ShowCustomAsync),
            }
        };
    }

    private async Task ShowMessageAsync()
    {
        var result = await _dialogService.ShowMessageAsync(
            "Hello from Lifter",
            "This is a message dialog shown via IDialogService.");
        SetResult(result);
    }

    private async Task ShowConfirmAsync()
    {
        var result = await _dialogService.ShowConfirmAsync(
            "Confirm action",
            "Do you want to proceed?",
            new DialogOptions { ButtonSet = DialogButtonSet.OkCancel });
        SetResult(result);
    }

    private async Task ShowYesNoAsync()
    {
        var result = await _dialogService.ShowConfirmAsync(
            "Save changes?",
            "You have unsaved changes. Do you want to save them before closing?",
            new DialogOptions { ButtonSet = DialogButtonSet.YesNoCancel });
        SetResult(result);
    }

    private async Task ShowCustomAsync()
    {
        var customContent = new StackPanel
        {
            Spacing = 8,
            Children =
            {
                new TextBlock { Text = "This is arbitrary Avalonia content inside a dialog.", TextWrapping = TextWrapping.Wrap },
                new TextBox { PlaceholderText = "Type something here…", Width = 300 },
            }
        };

        var result = await _dialogService.ShowAsync(
            "Custom content dialog",
            customContent,
            new DialogOptions { ButtonSet = DialogButtonSet.OkCancel });
        SetResult(result);
    }

    private void SetResult(DialogResult result)
    {
        Dispatcher.UIThread.Post(() =>
        {
            _dialogResultText.Text = $"Last dialog result: {result.Button} (Confirmed: {result.Confirmed})";
        });
    }

    private void OnStateChanged()
    {
        Dispatcher.UIThread.Post(() =>
        {
            _messageText.Text = _sharedState.CurrentMessage;
        });
    }
}
