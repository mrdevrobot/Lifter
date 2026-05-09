using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Lifter.Core.Dialog;

namespace Lifter.Avalonia;

/// <summary>
/// Default Avalonia implementation of <see cref="IDialogService"/>.
/// Shows native Avalonia <see cref="Window"/> dialogs, always parented to the current main window.
/// Override <see cref="ShowDialogInternalAsync"/> or any of the public methods to customise behaviour.
/// </summary>
public class AvaloniaDialogService : IDialogService
{
    /// <inheritdoc/>
    public Task<DialogResult> ShowMessageAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new DialogOptions { ButtonSet = DialogButtonSet.Ok };
        return ShowDialogInternalAsync(title, message, null, options, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<DialogResult> ShowConfirmAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new DialogOptions { ButtonSet = DialogButtonSet.OkCancel };
        return ShowDialogInternalAsync(title, message, null, options, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<DialogResult> ShowAsync(
        string title,
        object? content = null,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new DialogOptions { ButtonSet = DialogButtonSet.OkCancel };
        return ShowDialogInternalAsync(title, null, content, options, cancellationToken);
    }

    /// <summary>
    /// Core implementation that builds and shows the dialog window.
    /// Override this method to completely replace the dialog rendering logic.
    /// </summary>
    protected virtual Task<DialogResult> ShowDialogInternalAsync(
        string title,
        string? message,
        object? content,
        DialogOptions options,
        CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<DialogResult>(TaskCreationOptions.RunContinuationsAsynchronously);

        Dispatcher.UIThread.Post(async () =>
        {
            try
            {
                var owner = GetOwnerWindow();
                if (owner is null)
                {
                    tcs.TrySetResult(DialogResult.None);
                    return;
                }

                var result = await ShowDialogWindowAsync(title, message, content, options, owner, cancellationToken);
                tcs.TrySetResult(result);
            }
            catch (OperationCanceledException)
            {
                tcs.TrySetResult(DialogResult.None);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });

        return tcs.Task;
    }

    /// <summary>
    /// Returns the window that will own the dialog. Defaults to the desktop main window.
    /// Override to provide a different parent window.
    /// </summary>
    protected virtual Window? GetOwnerWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;

        return null;
    }

    /// <summary>
    /// Builds and shows the dialog <see cref="Window"/> against the given <paramref name="owner"/>.
    /// Override to fully customise the dialog window layout.
    /// </summary>
    protected virtual async Task<DialogResult> ShowDialogWindowAsync(
        string title,
        string? message,
        object? content,
        DialogOptions options,
        Window owner,
        CancellationToken cancellationToken)
    {
        DialogResult result = DialogResult.None;

        var window = BuildDialogWindow(title, message, content, options, r => result = r);

        using var reg = cancellationToken.Register(() =>
        {
            if (window.IsVisible) window.Close();
        });

        await window.ShowDialog(owner);
        return result;
    }

    /// <summary>
    /// Constructs the dialog <see cref="Window"/> with title, body and action buttons.
    /// Override to change the visual structure.
    /// </summary>
    protected virtual Window BuildDialogWindow(
        string title,
        string? message,
        object? content,
        DialogOptions options,
        Action<DialogResult> onResult)
    {
        var window = new Window
        {
            Title = title,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            MinWidth = 320,
            CanResize = false,
        };

        Control body = content is Control ctrl
            ? ctrl
            : new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 440,
            };

        var buttons = BuildButtonPanel(options, result =>
        {
            onResult(result);
            window.Close();
        });

        window.Content = new StackPanel
        {
            Margin = new Thickness(24),
            Spacing = 20,
            Children = { body, buttons },
        };

        return window;
    }

    /// <summary>
    /// Builds the row of action buttons based on <see cref="DialogOptions.ButtonSet"/>.
    /// Override to customise button styling or layout.
    /// </summary>
    protected virtual StackPanel BuildButtonPanel(DialogOptions options, Action<DialogResult> onResult)
    {
        var panel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Spacing = 8,
        };

        void Add(string text, DialogResult result)
        {
            var btn = new Button { Content = text };
            btn.Click += (_, _) => onResult(result);
            panel.Children.Add(btn);
        }

        switch (options.ButtonSet)
        {
            case DialogButtonSet.Ok:
                Add(options.OkText, DialogResult.Ok);
                break;
            case DialogButtonSet.OkCancel:
                Add(options.OkText, DialogResult.Ok);
                Add(options.CancelText, DialogResult.Cancel);
                break;
            case DialogButtonSet.YesNo:
                Add(options.YesText, DialogResult.Yes);
                Add(options.NoText, DialogResult.No);
                break;
            case DialogButtonSet.YesNoCancel:
                Add(options.YesText, DialogResult.Yes);
                Add(options.NoText, DialogResult.No);
                Add(options.CancelText, DialogResult.Cancel);
                break;
        }

        return panel;
    }
}
