using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using XKOMapp.GUI.ConsoleRows;

namespace XKOMapp.GUI;

public class ConsolePrinter
{
    private class ContentStartMarker : IHideableConsoleRow
    {
        bool ISwitchableConsoleRow.IsActive { get => false; set { } }

        public IRenderable GetRenderContent() => throw new Exception("Marker should never be asked for RenderContent");
        public void SetOwnership(ConsolePrinter owner) { }

        void ISwitchableConsoleRow.OnTurningOff() { }
        void ISwitchableConsoleRow.OnTurningOn() => throw new Exception("Marker should never be turned on");
    }
    private class ScrollerStartMarker : IHideableConsoleRow
    {
        bool ISwitchableConsoleRow.IsActive { get => false; set { } }

        public IRenderable GetRenderContent() => throw new Exception("Marker should never be asked for RenderContent");
        public void SetOwnership(ConsolePrinter owner) { }

        void ISwitchableConsoleRow.OnTurningOff() { }
        void ISwitchableConsoleRow.OnTurningOn() => throw new Exception("Marker should never be turned on");
    }
    private class GroupStartMarker : IHideableConsoleRow
    {
        bool ISwitchableConsoleRow.IsActive { get => false; set { } }

        public IRenderable GetRenderContent() => throw new Exception("Marker should never be asked for RenderContent");
        public void SetOwnership(ConsolePrinter owner) { }

        void ISwitchableConsoleRow.OnTurningOff() { }
        void ISwitchableConsoleRow.OnTurningOn() => throw new Exception("Marker should never be turned on");
    }

    /// <summary>
    /// Number of rows after which screen starts to scroll
    /// </summary>
    private readonly int cursorStickyStart = 5;
    /// <summary>
    /// Number of rows left at bottom of screen
    /// </summary>
    private readonly int paddingBottom = 0;

    public readonly ConsoleKey InteractionKey = ConsoleKey.Enter;
    public readonly ConsoleKey UpKey = ConsoleKey.UpArrow;
    public readonly ConsoleKey DownKey = ConsoleKey.DownArrow;

    private Grid content = null!;
    private readonly List<IRenderable> preContent = new();
    private readonly List<IConsoleRow> memory = new();
    private readonly List<string?> memoryGroupingKeys = new();

    /// <summary>
    /// Current index of row pointed by cursor
    /// <para>Null if there is no content rows</para>
    /// </summary>
    public int? CursorIndex { get; private set; } = null;
    private IConsoleRow? currentCursorRow = null;
    private IConsoleRow? previousCursorRow = null;
    private int? contentStart
    {
        get
        {
            var x = memory.FindIndex(x => x is ContentStartMarker);
            return x == -1 ? null : x;
        }
    }
    private int? scrollingStart
    {
        get
        {
            var x = memory.FindIndex(x => x is ScrollerStartMarker);
            return x == -1 ? null : x;
        }
    }


    public ConsolePrinter() => ClearMemory();

    public ConsolePrinter(int cursorStickyStart, int paddingBottom)
    {
        this.cursorStickyStart = cursorStickyStart;
        this.paddingBottom = paddingBottom;
        ClearMemory();
    }


    /// <summary>
    /// Resets cursor to lowest value possible
    /// </summary>
    public void ResetCursor()
    {
        CursorIndex = 0;

        ClampCursorUp();
        FinalizeCursorChange();
    }

    /// <summary>
    /// Moves cursor up if possible
    /// </summary>
    public void CursorUp()
    {
        if (CursorIndex is not null)
            CursorIndex--;

        ClampCursorUp();
        FinalizeCursorChange();
    }

    /// <summary>
    /// Moves cursor down if possible
    /// </summary>
    public void CursorDown()
    {
        if (CursorIndex is not null)
            CursorIndex++;

        ClampCursorDown();
        FinalizeCursorChange();
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

        CursorIndex = Math.Clamp(CursorIndex ?? 0, 0, memory.Count - 1);

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

        CursorIndex = Math.Clamp(CursorIndex ?? 0, 0, memory.Count - 1);

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
    /// Invoke on possible change of cursor position after clamping cursor
    /// </summary>
    private void FinalizeCursorChange()
    {
        if (CursorIndex is null)
            currentCursorRow = null;
        else
            currentCursorRow = memory[CursorIndex.Value];

        if (previousCursorRow == currentCursorRow)
            return;

        if (previousCursorRow is not null)
        {
            if (previousCursorRow is IHoverConsoleRow converted)
            {
                converted.OnHoverEnd();
            }
        }
        if (currentCursorRow is not null)
        {
            if (currentCursorRow is IHoverConsoleRow converted)
            {
                converted.OnHoverStart();
            }
        }

        previousCursorRow = currentCursorRow;
    }

    /// <summary>
    /// Gets available cursor positions (active content)
    /// </summary>
    /// <returns>List<int> of available positions for cursor</returns>
    private List<int> GetAvailableCursorIndexes()
    {
        if (contentStart is null)
            return new List<int>();

        var active = Enumerable.Range(0, memory.Count)
            .Where(index => index >= contentStart && (memory[index] is not IDeactivableConsoleRow converted || converted.IsActive));

        var focused = Enumerable.Range(0, memory.Count)
            .Where(index => memory[index] is IFocusableConsoleRow converted && converted.IsActive)
            .Intersect(active);

        if (focused.Any())
            return focused.ToList();

        return active.ToList();
    }


    /// <summary>
    /// Add new row to memory
    /// </summary>
    /// <param name="row">ConsoleRow to add</param>
    public void AddRow(IConsoleRow row)
    {
        row.SetOwnership(this);
        memory.Add(row);
        memoryGroupingKeys.Add(null);
    }

    /// <summary>
    /// Add new row to memory group
    /// </summary>
    /// <param name="row">ConsoleRow to add</param>
    /// <param name="group">Group to assign row to</param>
    public void AddRow(IConsoleRow row, string group)
    {
        row.SetOwnership(this);

        var index = memoryGroupingKeys.FindLastIndex(x => x == group);
        if (index == -1)
        {
            memory.Add(row);
            memoryGroupingKeys.Add(group);
        }
        else
        {
            memory.Insert(index + 1, row);
            memoryGroupingKeys.Insert(index + 1, group);
        }
    }

    /// <summary>
    /// Starts new group without any content
    /// </summary>
    /// <param name="id">Id of group</param>
    /// <exception cref="Exception">If group already exists</exception>
    public void StartGroup(string id)
    {
        if (memoryGroupingKeys.Contains(id))
            throw new Exception("Row group already exists");

        memory.Add(new GroupStartMarker());
        memoryGroupingKeys.Add(id);
    }

    /// <summary>
    /// Clears memory
    /// </summary>
    public void ClearMemory()
    {
        memory.Clear();
        memoryGroupingKeys.Clear();
        ResetCursor();
    }

    /// <summary>
    /// Deletes memory group and its content
    /// </summary>
    /// <param name="group"></param>
    public void DeleteMemoryGroup(string group)
    {
        Enumerable.Range(0, memory.Count)
            .Where(index => memoryGroupingKeys[index] == group)
            .Reverse()
            .ToList()
            .ForEach(index =>
            {
                memory.RemoveAt(index);
                memoryGroupingKeys.RemoveAt(index);
            });
    }

    /// <summary>
    /// Clears memory group content without deleting it
    /// </summary>
    /// <param name="group"></param>
    public void ClearMemoryGroup(string group)
    {
        Enumerable.Range(0, memory.Count)
            .Where(index => memoryGroupingKeys[index] == group)
            .Where(index => memory[index] is not GroupStartMarker)
            .Reverse()
            .ToList()
            .ForEach(index =>
            {
                memory.RemoveAt(index);
                memoryGroupingKeys.RemoveAt(index);
            });
    }


    /// <summary>
    /// Ends header section and starts interactible content (resets after clearing memory)
    /// </summary>
    public void StartContent() => AddRow(new ContentStartMarker());

    /// <summary>
    /// Enables scrolling for current memory starting from this row
    /// </summary>
    public void EnableScrolling() => AddRow(new ScrollerStartMarker());



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
        FinalizeCursorChange();

        int endLineIndex = 0;
        int linesShifted = 0;
        int linesShiftStartIndex = memory
            .Take(CursorIndex ?? 0)
            .Where(x => x is not IDeactivableConsoleRow converted || converted.IsActive)
            .Sum(x => (x as ICustomLineSpanConsoleRow)?.GetRenderHeight() ?? 1)
            - cursorStickyStart;
        int linesEndTotalIndex = memory
            .Where(x => x is not IDeactivableConsoleRow converted || converted.IsActive)
            .Sum(x => (x as ICustomLineSpanConsoleRow)?.GetRenderHeight() ?? 1);
        for (int index = 0; index < memory.Count; index++)
        {
            IConsoleRow row = memory[index];

            bool isHidden = row is IHideableConsoleRow hideableConverted && !hideableConverted.IsActive;
            if (isHidden)
                continue;

            bool isContent = contentStart is not null && index >= contentStart;
            bool isScrollable = scrollingStart is not null && index >= scrollingStart;
            int lineSpan = (row as ICustomLineSpanConsoleRow)?.GetRenderHeight() ?? 1;
            bool isHovered = (row == currentCursorRow);

            int startLineIndex = endLineIndex;
            endLineIndex = lineSpan + startLineIndex;

            if (isScrollable && endLineIndex < linesShiftStartIndex)
            {
                if (linesEndTotalIndex - linesShifted > Console.WindowHeight - 1 - paddingBottom)
                {
                    linesShifted += (row as ICustomLineSpanConsoleRow)?.GetRenderHeight() ?? 1;
                    continue;
                }
            }

            if (!isContent)
            {
                if (endLineIndex > Console.WindowHeight - 1 - paddingBottom)
                    return;

                preContent.Add(row.GetRenderContent());
                continue;
            }

            if (endLineIndex - linesShifted > Console.WindowHeight - 1 - paddingBottom)
                return;

            string cursor = ">";
            string background = " ";
            if (row is ICustomCursorConsoleRow customCursorConverted)
            {
                cursor = customCursorConverted.GetCustomCursor();
                background = customCursorConverted.GetCustomCursorBackground();
            }

            content.AddRow(new Markup(isHovered ? cursor : background), row.GetRenderContent());
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
    /// Prints content of memory
    /// <para>Also reloads buffers</para>
    /// </summary>
    public void PrintMemory()
    {
        ReloadBuffer();
        PrintBuffer();
    }

    /// <summary>
    /// Clears console
    /// </summary>
    public static void ClearScreen() => AnsiConsole.Clear();


    /// <summary>
    /// Interact with row hovered on right now
    /// </summary>
    public void Interract() => (currentCursorRow as IInteractableConsoleRow)?.OnInteraction();

    /// <summary>
    /// Pass pressed key to process
    /// </summary>
    /// <param name="keystrokeInfo">ConsoleKeyInfo of pressed key</param>
    public void PassKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        var key = keystrokeInfo.Key;

        if (key != DownKey && key != UpKey && key != InteractionKey)
        {
            PassCustomKeystroke(keystrokeInfo);
            return;
        }

        if (currentCursorRow is IStandardKeystrokeOverrideConsoleRow converted)
        {
            if (!converted.ProcessStandardKeystroke(keystrokeInfo))
                return;
        }

        if (key == DownKey)
            CursorDown();
        else if (key == UpKey)
            CursorUp();
        else if (key == InteractionKey)
            Interract();
    }

    /// <summary>
    /// Pass non-standard pressed key to process
    /// </summary>
    /// <param name="keystrokeInfo">ConsoleKeyInfo of pressed key</param>
    private void PassCustomKeystroke(ConsoleKeyInfo keystrokeInfo) => (currentCursorRow as ICustomKeystrokeListenerConsoleRow)?.ProcessCustomKeystroke(keystrokeInfo);
}
