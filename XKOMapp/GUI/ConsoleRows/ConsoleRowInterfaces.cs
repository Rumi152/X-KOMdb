using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows;

public interface IConsoleRow
{
    IRenderable GetRenderContent();
}

public interface IInteractableConsoleRow : IConsoleRow
{
    void OnInteraction();
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

public delegate void ConsoleRowAction(IConsoleRow row);