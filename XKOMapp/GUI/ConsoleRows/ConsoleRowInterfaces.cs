using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows;

/// <summary>
/// Basic renderable ConsoleRow
/// </summary>
public interface IConsoleRow
{
    IRenderable GetRenderContent();
}

/// <summary>
/// ConsoleRow with action on click
/// </summary>
public interface IInteractableConsoleRow : IConsoleRow
{
    void OnInteraction(ConsolePrinter printer);
}

/// <summary>
/// ConsoleRow with actions on hovering start and end
/// </summary>
public interface IHoverConsoleRow : IConsoleRow
{
    void OnHoverStart();
    void OnHoverEnd();
}

/// <summary>
/// ConsoleRow with custom displayed cursor
/// </summary>
public interface ICustomCursorConsoleRow : IConsoleRow
{
    string GetCustomCursor();
    string GetCustomCursorBackground();
}

/// <summary>
/// ConsoleRow which can be hidden
/// </summary>
public interface IHideableConsoleRow : IConsoleRow
{
    public bool IsHidden { get; protected set; }

    void Hide()
    {
        if (IsHidden)
            return;

        IsHidden = true;
        OnHide();
    }
    void Show()
    {
        if (!IsHidden)
            return;

        IsHidden = false;
        OnShow();
    }

    protected void OnHide();
    protected void OnShow();
}

/// <summary>
/// Delegate for action invoked by ConsoleRow
/// </summary>
/// <param name="row">Row invoking action</param>
/// <param name="printer">Printer owning row</param>
public delegate void ConsoleRowAction(IConsoleRow row, ConsolePrinter printer);