using Spectre.Console;
using Spectre.Console.Rendering;
using XKOMapp.GUI;

namespace XKOMapp.GUI.ConsoleRows;

internal class PriceRangeInputConsoleRow : IModesConsoleRow, ICustomKeystrokeListenerConsoleRow, ICustomCursorConsoleRow, IHoverConsoleRow, IInteractableConsoleRow
{
    public int ModesCount => 2;
    private int modeIndex = 0;
    int IModesConsoleRow.ModeIndex { get => modeIndex; set => modeIndex = value; }

    public string LowestPrice { get; private set; } = "";
    public string HighestPrice { get; private set; } = "";

    private ConsolePrinter? owner;
    private bool isHovered = false;

    private readonly ConsoleRowAction? onHoverEnd;
    private readonly ConsoleRowAction? onInteraction;

    public PriceRangeInputConsoleRow(ConsoleRowAction? onHoverEnd, ConsoleRowAction? onInteraction)
    {
        this.onHoverEnd = onHoverEnd;
        this.onInteraction = onInteraction;
    }

    public string GetCustomCursor()
    {
        return "[blink]\u00BB[/]";
    }

    public string GetCustomCursorBackground()
    {
        return " ";
    }

    public IRenderable GetRenderContent()
    {
        var style1 = modeIndex == 0 && isHovered ? " underline" : "";
        var style2 = modeIndex == 1 && isHovered ? " underline" : "";
        return new Markup($"[lime{style1}]{LowestPrice,-6}[/] - [lime{style2}]{HighestPrice,-6}[/] PLN");
    }

    public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        if (keystrokeInfo.Key == ConsoleKey.RightArrow)
            (this as IModesConsoleRow).IncrementModeIndex();
        else if (keystrokeInfo.Key == ConsoleKey.LeftArrow)
            (this as IModesConsoleRow).DecrementModeIndex();
        else if (char.IsDigit(keystrokeInfo.KeyChar))
        {
            if (modeIndex == 0 && LowestPrice.Length < 6)
                LowestPrice += keystrokeInfo.KeyChar;
            else if (modeIndex == 1 && HighestPrice.Length < 6)
                HighestPrice += keystrokeInfo.KeyChar;
        }
        else if (keystrokeInfo.Key == ConsoleKey.Backspace)
        {
            if (modeIndex == 0 && LowestPrice.Length > 0)
                LowestPrice = LowestPrice.Remove(LowestPrice.Length - 1);
            else if (modeIndex == 1 && HighestPrice.Length > 0)
                HighestPrice = HighestPrice.Remove(HighestPrice.Length - 1);
        }
    }

    public void SetOwnership(ConsolePrinter owner)
    {
        this.owner = owner;
    }

    void IModesConsoleRow.OnModeChange()
    {

    }

    public void OnHoverStart()
    {
        isHovered = true;
    }

    public void OnHoverEnd()
    {
        isHovered = false;
        onHoverEnd?.Invoke(this, owner);
    }

    public void OnInteraction()
    {
        onInteraction?.Invoke(this, owner);
    }
}