using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
namespace XKOMapp.GUI;

public class ConsolePrinter
{
    private Grid content;
    private readonly List<IRenderable> preContent = new();

    private readonly List<ConsoleRow> rows = new List<ConsoleRow>();
    private int? contentStart = null;

    /// <summary>
    /// current index of Cursor from top
    /// </summary>
    public int CursorIndex { get; private set; } = 0;

    public ConsolePrinter()
    {
        ClearMemory();
    }

    /// <summary>
    /// Resets cursor to lowest value possible
    /// </summary>
    public void ResetCursor()
    {
        CursorIndex = 0;
        ClampCursor();
    }

    /// <summary>
    /// Clams cursor with allowed positions
    /// </summary>
    public void ClampCursor()
    {
        CursorIndex = Math.Clamp(CursorIndex, contentStart ?? 0, rows.Count - 1);
    }

    /// <summary>
    /// Moves cursor up if possible
    /// </summary>
    public void CursorUp()
    {
        CursorIndex--;
        ClampCursor();
    }

    /// <summary>
    /// Moves cursor down if possible
    /// </summary>
    public void CursorDown()
    {
        CursorIndex++;
        ClampCursor();
    }

    /// <summary>
    /// Interact with row hovered on right now
    /// </summary>
    public void Interract()
    {
        if (!contentStart.HasValue)
            return;

        rows.ElementAtOrDefault(CursorIndex)?.OnInteraction();
    }


    /// <summary>
    /// Add new row to memory
    /// </summary>
    /// <param name="row">ConsoleRow to add</param>
    public void AddRow(ConsoleRow row) => rows.Add(row);

    /// <summary>
    /// Ends header section and starts interactible content (resets after clearing memory)
    /// </summary>
    public void StartContent() => contentStart = rows.Count;

    /// <summary>
    /// Clears buffer and memory
    /// </summary>
    public void ClearMemory()
    {
        rows.Clear();
        ClearBuffer();
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

        if (!contentStart.HasValue)
            preContent.AddRange(rows.Select(row => row.GetRenderContent()));
        else
        {
            int index = 0;
            rows.ForEach(row =>
            {
                if (index < contentStart)
                    preContent.Add(row.GetRenderContent());
                else
                {
                    bool hovered = (index == CursorIndex);
                    content.AddRow(new Text(hovered ? ">" : " "), row.GetRenderContent(hovered));
                }

                index++;
            });
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
            AnsiConsole.WriteLine();
        });
        AnsiConsole.Write(content);
    }

    /// <summary>
    /// Clears console
    /// </summary>
    public void ClearScreen() => Console.Clear();
}
