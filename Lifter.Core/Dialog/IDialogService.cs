namespace Lifter.Core.Dialog;

/// <summary>
/// Provides dialog and modal functionality across platforms.
/// Register a platform-specific implementation via DI, or override individual methods
/// to customise behaviour for specific dialog types.
/// </summary>
public interface IDialogService
{
    /// <summary>
    /// Shows a simple informational message dialog.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="options">Optional display options. When <see langword="null"/>, defaults to a single OK button.</param>
    /// <param name="cancellationToken">Token used to programmatically close the dialog.</param>
    Task<DialogResult> ShowMessageAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a confirmation dialog (e.g. OK/Cancel or Yes/No).
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="message">The question or message to display.</param>
    /// <param name="options">Optional display options. When <see langword="null"/>, defaults to an OK/Cancel button set.</param>
    /// <param name="cancellationToken">Token used to programmatically close the dialog.</param>
    Task<DialogResult> ShowConfirmAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Shows a dialog containing arbitrary content.
    /// Platform implementations may render the content as a native window, a modal overlay, etc.
    /// </summary>
    /// <param name="title">The dialog title.</param>
    /// <param name="content">
    /// Platform-specific content (e.g. an Avalonia <c>Control</c>, a Blazor <c>RenderFragment</c>,
    /// or a MAUI <c>View</c>). When <see langword="null"/> an empty dialog is shown.
    /// </param>
    /// <param name="options">Optional display options.</param>
    /// <param name="cancellationToken">Token used to programmatically close the dialog.</param>
    Task<DialogResult> ShowAsync(
        string title,
        object? content = null,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default);
}
