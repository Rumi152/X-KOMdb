using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows;

public interface IConsoleRow
{
    IRenderable GetRenderContent();
}

public interface IInteractableConsoleRow : IConsoleRow
{
    void OnInteraction(ConsolePrinter printer);
}

public interface IHoverConsoleRow : IConsoleRow
{
    void OnHoverStart();
    void OnHoverEnd();
}

public interface ICustomCursorConsoleRow : IConsoleRow
{
    string GetCustomCursor();
    string GetCustomCursorBackground();
}

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

public delegate void ConsoleRowAction(IConsoleRow row, ConsolePrinter printer);