namespace Lifter.Core.Dialog;

/// <summary>
/// Represents the outcome of a dialog interaction.
/// </summary>
public sealed class DialogResult
{
    /// <summary>The dialog was dismissed without any explicit button press (e.g. closed via the X button or cancelled).</summary>
    public static readonly DialogResult None = new(DialogButton.None);

    /// <summary>The user pressed OK.</summary>
    public static readonly DialogResult Ok = new(DialogButton.Ok);

    /// <summary>The user pressed Cancel.</summary>
    public static readonly DialogResult Cancel = new(DialogButton.Cancel);

    /// <summary>The user pressed Yes.</summary>
    public static readonly DialogResult Yes = new(DialogButton.Yes);

    /// <summary>The user pressed No.</summary>
    public static readonly DialogResult No = new(DialogButton.No);

    /// <summary>The user pressed Close.</summary>
    public static readonly DialogResult Close = new(DialogButton.Close);

    /// <summary>Gets the button that the user pressed.</summary>
    public DialogButton Button { get; }

    /// <summary>
    /// Returns <see langword="true"/> when the user confirmed the dialog
    /// (i.e. pressed <see cref="DialogButton.Ok"/> or <see cref="DialogButton.Yes"/>).
    /// </summary>
    public bool Confirmed => Button is DialogButton.Ok or DialogButton.Yes;

    private DialogResult(DialogButton button) => Button = button;
}
