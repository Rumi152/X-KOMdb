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

    public int CursorIndex { get; private set; } = 0;

    public ConsolePrinter()
    {
        ClearMemory();
    }

    public void ResetCursor()
    {
        CursorIndex = 0;
        ClampCursor();
    }
    public void ClampCursor()
    {
        CursorIndex = Math.Clamp(CursorIndex, contentStart ?? 0, rows.Count - 1);
    }
    public void CursorUp()
    {
        CursorIndex--;
        ClampCursor();
    }
    public void CursorDown()
    {
        CursorIndex++;
        ClampCursor();
    }

    public void Interract()
    {
        if (!contentStart.HasValue)
            return;

        rows.ElementAtOrDefault(CursorIndex)?.OnInteraction();
    }

    public void AddRow(ConsoleRow row) => rows.Add(row);
    public void StartContent() => contentStart = rows.Count;
    public void ClearMemory()
    {
        rows.Clear();
        ClearBuffer();
    }


    private void ClearBuffer()
    {
        content = new Grid().AddColumns(2);
        content.Columns[0].Width = 1;

        preContent.Clear();
    }
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
                    content.AddRow(new Text((index == CursorIndex) ? ">" : " "), row.GetRenderContent());

                index++;
            });
        }
    }
    public void PrintBuffer()
    {
        preContent.ForEach(renderable =>
        {
            AnsiConsole.Write(renderable);
            AnsiConsole.WriteLine();
        });
        AnsiConsole.Write(content);
    }

    public void ClearScreen() => Console.Clear();
}
