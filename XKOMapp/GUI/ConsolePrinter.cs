using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI;

public class ConsolePrinter
{
    private Grid grid;

    private readonly List<ConsoleRow> rows = new List<ConsoleRow>();
    public IReadOnlyList<ConsoleRow> Rows => rows;

    public int CursorIndex { get; private set; } = 0;

    public ConsolePrinter()
    {
        ResetGrid();
    }

    public void AddRow(ConsoleRow row) => rows.Add(row);

    public void ResetCursor()
    {
        CursorIndex = 0;
        PrintBuffer();
    }

    public void CursorUp()
    {
        CursorIndex = Math.Max(CursorIndex - 1, 0);
        PrintBuffer();
    }

    public void CursorDown()
    {
        CursorIndex = Math.Min(CursorIndex + 1, rows.Count - 1);
        PrintBuffer();
    }

    public void Interract()
    {
        rows[CursorIndex]
            .OnInteraction();
    }

    public void ClearBuffer()
    {
        Console.Clear();
        ResetGrid();
    }

    public void ResetGrid()
    {
        grid = new Grid().AddColumns(2);
        grid.Columns[0].Width = 1;
    }

    public void PrintBuffer()
    {
        Console.Clear();
        ResetGrid();

        int repeat = 0;
        rows.ForEach(row =>
        {
            grid.AddRow(new Text((repeat == CursorIndex) ? ">" : " "), row.GetRenderContent());
            repeat++;
        });

        AnsiConsole.Write(grid);
    }
}
