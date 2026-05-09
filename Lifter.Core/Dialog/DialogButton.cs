namespace Lifter.Core.Dialog;

/// <summary>
/// Represents the button pressed by the user to dismiss a dialog.
/// </summary>
public enum DialogButton
{
    /// <summary>No button was pressed (e.g. dialog was cancelled or closed programmatically).</summary>
    None,
    /// <summary>The user pressed OK.</summary>
    Ok,
    /// <summary>The user pressed Cancel.</summary>
    Cancel,
    /// <summary>The user pressed Yes.</summary>
    Yes,
    /// <summary>The user pressed No.</summary>
    No,
    /// <summary>The user pressed Close.</summary>
    Close,
}
