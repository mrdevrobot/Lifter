namespace Lifter.Core.Dialog;

/// <summary>
/// A no-op <see cref="IDialogService"/> implementation suitable for testing,
/// headless environments, or as a placeholder before a real implementation is registered.
/// All methods immediately return a confirmed result without showing any UI.
/// </summary>
public class NullDialogService : IDialogService
{
    /// <inheritdoc/>
    public Task<DialogResult> ShowMessageAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
        => Task.FromResult(DialogResult.Ok);

    /// <inheritdoc/>
    public Task<DialogResult> ShowConfirmAsync(
        string title,
        string message,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
        => Task.FromResult(DialogResult.Ok);

    /// <inheritdoc/>
    public Task<DialogResult> ShowAsync(
        string title,
        object? content = null,
        DialogOptions? options = null,
        CancellationToken cancellationToken = default)
        => Task.FromResult(DialogResult.Ok);
}
