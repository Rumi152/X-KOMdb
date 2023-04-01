using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics.CodeAnalysis;
using XKOMapp.GUI.ConsoleRows;

namespace XKOMapp.GUI;

public class ConsolePrinter
{
    private Grid content;
    private readonly List<IRenderable> preContent = new();
    private readonly List<IConsoleRow> rows = new List<IConsoleRow>();

    /// <summary>
    /// Current index of Cursor from top
    /// Null if there is no content rows
    /// </summary>
    public int? CursorIndex { get; private set; } = null;
    private int? previousCursorIndex = null;
    private int? contentStart = null;


    public ConsolePrinter() => ClearMemory();


    /// <summary>
    /// Resets cursor to lowest value possible
    /// </summary>
    public void ResetCursor()
    {
        CursorIndex = null;
        ClampCursor();

        OnCursorChange();
    }

    /// <summary>
    /// Moves cursor up if possible
    /// </summary>
    public void CursorUp()
    {
        if (CursorIndex is not null)
            CursorIndex--;

        ClampCursor();

        OnCursorChange();
    }

    /// <summary>
    /// Moves cursor down if possible
    /// </summary>
    public void CursorDown()
    {
        if (CursorIndex is not null)
            CursorIndex++;

        ClampCursor();

        OnCursorChange();
    }

    // Invoke on possible change of cursor position
    private void OnCursorChange()
    {
        if (previousCursorIndex == CursorIndex)
            return;

        if (previousCursorIndex is not null)
        {
            var row = rows.ElementAtOrDefault(previousCursorIndex.Value);
            if (row is not null)
            {
                if (row is IHoverConsoleRow converted)
                {
                    converted.OnHoverEnd();
                }
            }
        }
        if (CursorIndex is not null)
        {
            var row = rows.ElementAtOrDefault(CursorIndex.Value);
            if (row is not null)
            {
                if (row is IHoverConsoleRow converted)
                {
                    converted.OnHoverStart();
                }
            }
        }

        previousCursorIndex = CursorIndex;
    }

    // Clamps cursor with allowed positions
    private void ClampCursor()
    {
        if (contentStart is null || contentStart >= rows.Count)
        {
            CursorIndex = null;
            return;
        }

        CursorIndex = Math.Clamp(CursorIndex ?? 0, contentStart.Value, rows.Count - 1);
    }

    /// <summary>
    /// Interact with row hovered on right now
    /// </summary>
    public void Interract()
    {
        if (CursorIndex is null)
            return;

        IConsoleRow? row = rows.ElementAtOrDefault(CursorIndex.Value);

        if (row is null)
            return;

        if (row is not IInteractableConsoleRow converted)
            return;

        converted.OnInteraction();
    }


    /// <summary>
    /// Add new row to memory
    /// </summary>
    /// <param name="row">ConsoleRow to add</param>
    public void AddRow(IConsoleRow row) => rows.Add(row);

    /// <summary>
    /// Ends header section and starts interactible content (resets after clearing memory)
    /// </summary>
    public void StartContent() => contentStart = rows.Count;

    /// <summary>
    /// Clears buffer, screen and memory
    /// </summary>
    public void ClearMemory()
    {
        rows.Clear();
        ClearBuffer();
        ClearScreen();

        contentStart = null;
        ResetCursor();
    }


    /// <summary>
    /// Clears buffers holding items ready to render
    /// </summary>
    private void ClearBuffer()
    {
        content = new Grid().AddColumns(2);
        content.Columns[0].Width = 1;

        preContent.Clear();
    }

    /// <summary>
    /// Reload buffers using memory
    /// </summary>
    public void ReloadBuffer()
    {
        ClearBuffer();
        ClampCursor();
        OnCursorChange();

        if (contentStart is null)
        {
            preContent.AddRange(rows.Select(row => row.GetRenderContent()));
            return;
        }

        int index = 0;
        rows.ForEach(row =>
        {
            if (index < contentStart)
                preContent.Add(row.GetRenderContent());
            else
            {
                bool hovered = (index == CursorIndex);

                string cursor = ">";
                string background = " ";
                if (row is ICustomCursorConsoleRow converted)
                {
                    cursor = converted.GetCustomCursor();
                    background = converted.GetCustomCursorBackground();
                }

                content.AddRow(new Markup(hovered ? cursor : background), row.GetRenderContent());
            }

            index++;
        });
    }

    /// <summary>
    /// Prints content of buffers
    /// </summary>
    public void PrintBuffer()
    {
        preContent.ForEach(renderable =>
        {
            AnsiConsole.Write(renderable);
        });
        AnsiConsole.Write(content);
    }

    /// <summary>
    /// Clears console
    /// </summary>
    public void ClearScreen() => Console.Clear();
}
