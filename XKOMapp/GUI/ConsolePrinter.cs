using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using XKOMapp.GUI.ConsoleRows;

namespace XKOMapp.GUI;

public class ConsolePrinter
{
    /// <summary>
    /// Number of rows after which screen starts to scroll
    /// </summary>
    const int cursorStickyStart = 5;
    /// <summary>
    /// Number of rows left at bottom of screen
    /// </summary>
    const int paddingBottom = 1;

    private Grid content;
    private readonly List<IRenderable> preContent = new();
    private readonly List<IConsoleRow> rows = new List<IConsoleRow>();

    /// <summary>
    /// Current index of Cursor from top
    /// <para>Null if there is no content rows</para>
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
        CursorIndex = 0;

        ClampCursorUp();
        OnCursorChange();
    }

    /// <summary>
    /// Moves cursor up if possible
    /// </summary>
    public void CursorUp()
    {
        if (CursorIndex is not null)
            CursorIndex--;

        ClampCursorUp();
        OnCursorChange();
    }

    /// <summary>
    /// Moves cursor down if possible
    /// </summary>
    public void CursorDown()
    {
        if (CursorIndex is not null)
            CursorIndex++;

        ClampCursorDown();
        OnCursorChange();
    }

    /// <summary>
    /// Invoke on possible change of cursor position
    /// </summary>
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

    /// <summary>
    /// Clamps cursor with allowed positions
    /// <para>Prefers closest higher position if current in not possible</para>
    /// </summary>
    private void ClampCursorUp()
    {
        var availableIndexes = GetAvailableCursorIndexes();

        if (availableIndexes.Count == 0)
        {
            CursorIndex = null;
            return;
        }

        CursorIndex = Math.Clamp(CursorIndex ?? 0, contentStart.Value, rows.Count - 1);

        if (availableIndexes.Contains(CursorIndex.Value))
            return;

        var previousIndex = CursorIndex;
        CursorIndex = availableIndexes
            .Where(index => index < previousIndex)
            .LastOrDefault(-1);

        if (CursorIndex == -1)
        {
            CursorIndex = availableIndexes
                .Where(index => index > previousIndex)
                .First();
        }
    }

    /// <summary>
    /// Clamps cursor with allowed positions
    /// <para>Prefers closest lower position if current in not possible</para>
    /// </summary>
    private void ClampCursorDown()
    {
        var availableIndexes = GetAvailableCursorIndexes();

        if (availableIndexes.Count == 0)
        {
            CursorIndex = null;
            return;
        }

        CursorIndex = Math.Clamp(CursorIndex ?? 0, contentStart.Value, rows.Count - 1);

        if (availableIndexes.Contains(CursorIndex.Value))
            return;

        var previousIndex = CursorIndex;
        CursorIndex = availableIndexes
            .Where(index => index > previousIndex)
            .FirstOrDefault(-1);

        if (CursorIndex == -1)
        {
            CursorIndex = availableIndexes
                .Where(index => index < previousIndex)
                .Last();
        }
    }

    /// <summary>
    /// Gets available cursor positions (not hidden content)
    /// </summary>
    /// <returns>List<int> of available positions for cursor</returns>
    private List<int> GetAvailableCursorIndexes()
    {
        if (contentStart is null)
            return new List<int>();

        return Enumerable.Range(contentStart.Value, Math.Max(0, rows.Count - contentStart.Value))
            .Where(index => rows[index] is not IHideableConsoleRow converted || !converted.IsHidden)
            .ToList();
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

        converted.OnInteraction(this);
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
        ClampCursorUp();
        OnCursorChange();

        int endLineIndex = 0;
        for (int index = 0; index < rows.Count; index++)
        {
            IConsoleRow row = rows[index];

            bool isHidden = row is IHideableConsoleRow hideableConverted && hideableConverted.IsHidden;
            if (isHidden)
                continue;

            bool isContent = contentStart is not null && index >= contentStart;
            int lineSpan = (row as ICustomLineSpanConsoleRow)?.GetRenderHeight() ?? 1;
            int startLineIndex = endLineIndex;

            if (!isContent)
            {
                endLineIndex = lineSpan + startLineIndex;
                if (endLineIndex > Console.WindowHeight - paddingBottom)
                    return;

                preContent.Add(row.GetRenderContent());
                continue;
            }

            if (index < (CursorIndex ?? 0) - cursorStickyStart)
            {
                endLineIndex = 1 + startLineIndex;
                continue;
            }

            endLineIndex = lineSpan + startLineIndex;
            if (endLineIndex > Console.WindowHeight - paddingBottom + Math.Max(0, (CursorIndex ?? 0) - cursorStickyStart - contentStart.Value))
                return;

            bool hovered = (index == CursorIndex);

            string cursor = ">";
            string background = " ";
            if (row is ICustomCursorConsoleRow customCursorConverted)
            {
                cursor = customCursorConverted.GetCustomCursor();
                background = customCursorConverted.GetCustomCursorBackground();
            }

            content.AddRow(new Markup(hovered ? cursor : background), row.GetRenderContent());
        }
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
    public void ClearScreen() => AnsiConsole.Clear();
}
