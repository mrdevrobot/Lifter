using Lifter.Core.Dialog;
using Microsoft.JSInterop;

namespace Lifter.Blazor;

/// <summary>
/// Default Blazor WebAssembly implementation of <see cref="IDialogService"/>.
/// Uses the browser's native <c>alert</c> and <c>confirm</c> APIs via JS interop.
/// Override individual methods or subclass to use a custom modal component
/// (e.g. BootstrapBlazor, MudBlazor, Radzen, etc.).
/// </summary>
public class BlazorDialogService : IDialogService
{
    private readonly IJSRuntime _js;

    public BlazorDialogService(IJSRuntime js)
    {
        _js = js;
    }

    /// <inheritdoc/>
    public virtual async Task<DialogResult> ShowMessageAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        await _js.InvokeVoidAsync("alert", cancellationToken, FormatMessage(title, message));
        return DialogResult.Ok;
    }

    /// <inheritdoc/>
    public virtual async Task<DialogResult> ShowConfirmAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var confirmed = await _js.InvokeAsync<bool>("confirm", cancellationToken, FormatMessage(title, message));

        options ??= new DialogOptions { ButtonSet = DialogButtonSet.OkCancel };

        return confirmed
            ? (options.ButtonSet == DialogButtonSet.YesNo || options.ButtonSet == DialogButtonSet.YesNoCancel
                ? DialogResult.Yes
                : DialogResult.Ok)
            : DialogResult.Cancel;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The default implementation falls back to <see cref="ShowConfirmAsync"/> using
    /// <c>content.ToString()</c> as the message body.
    /// Override this method to render a proper modal with custom Blazor content.
    /// </remarks>
    public virtual Task<DialogResult> ShowAsync(
        string title,
        object? content = null,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
        => ShowConfirmAsync(title, content?.ToString() ?? string.Empty, options, cancellationToken);

    private static string FormatMessage(string title, string message)
        => string.IsNullOrWhiteSpace(title) ? message : $"{title}\n\n{message}";
}
