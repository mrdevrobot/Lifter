using Lifter.Core.Dialog;

namespace Lifter.Maui;

/// <summary>
/// Default .NET MAUI implementation of <see cref="IDialogService"/>.
/// Uses <see cref="Page.DisplayAlert"/> for message and confirmation dialogs.
/// Override <see cref="GetMainPage"/> or individual methods to customise behaviour
/// (e.g. use a custom modal <see cref="Page"/> for <see cref="ShowAsync"/>).
/// </summary>
public class MauiDialogService : IDialogService
{
    /// <inheritdoc/>
    public virtual async Task<DialogResult> ShowMessageAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new DialogOptions { ButtonSet = DialogButtonSet.Ok };

        var page = GetMainPage();
        if (page is null) return DialogResult.None;

        await page.DisplayAlert(title, message, options.OkText);
        return DialogResult.Ok;
    }

    /// <inheritdoc/>
    public virtual async Task<DialogResult> ShowConfirmAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new DialogOptions { ButtonSet = DialogButtonSet.OkCancel };

        var page = GetMainPage();
        if (page is null) return DialogResult.None;

        bool useYesNo = options.ButtonSet is DialogButtonSet.YesNo or DialogButtonSet.YesNoCancel;

        string acceptText = useYesNo ? options.YesText : options.OkText;
        string cancelText = useYesNo ? options.NoText : options.CancelText;

        bool accepted = await page.DisplayAlert(title, message, acceptText, cancelText);

        return accepted
            ? (useYesNo ? DialogResult.Yes : DialogResult.Ok)
            : (useYesNo ? DialogResult.No : DialogResult.Cancel);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The default MAUI implementation falls back to <see cref="ShowConfirmAsync"/> using
    /// <c>content.ToString()</c> as the body, since MAUI has no built-in arbitrary-content modal.
    /// Override this method to push a custom <see cref="Page"/> onto the navigation stack.
    /// </remarks>
    public virtual Task<DialogResult> ShowAsync(
        string title,
        object? content = null,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
        => ShowConfirmAsync(title, content?.ToString() ?? string.Empty, options, cancellationToken);

    /// <summary>
    /// Returns the <see cref="Page"/> used to show native dialogs.
    /// Defaults to <see cref="Application.Current"/>.<see cref="Application.MainPage"/>.
    /// Override to provide a different page (e.g. the currently active navigation page).
    /// </summary>
    protected virtual Page? GetMainPage() => Application.Current?.MainPage;
}
