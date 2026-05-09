namespace Lifter.Core.Dialog;

/// <summary>
/// Options for configuring the appearance and behaviour of a dialog.
/// </summary>
public class DialogOptions
{
    /// <summary>The label for the OK button. Defaults to "OK".</summary>
    public string OkText { get; set; } = "OK";

    /// <summary>The label for the Cancel button. Defaults to "Cancel".</summary>
    public string CancelText { get; set; } = "Cancel";

    /// <summary>The label for the Yes button. Defaults to "Yes".</summary>
    public string YesText { get; set; } = "Yes";

    /// <summary>The label for the No button. Defaults to "No".</summary>
    public string NoText { get; set; } = "No";

    /// <summary>The label for the Close button. Defaults to "Close".</summary>
    public string CloseText { get; set; } = "Close";

    /// <summary>The set of buttons shown in the dialog. Defaults to <see cref="DialogButtonSet.Ok"/>.</summary>
    public DialogButtonSet ButtonSet { get; set; } = DialogButtonSet.Ok;
}
