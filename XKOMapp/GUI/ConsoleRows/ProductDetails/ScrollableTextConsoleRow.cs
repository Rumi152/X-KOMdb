using Spectre.Console;
using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows.ProductDetails;

internal class ScrollableTextConsoleRow : ICustomKeystrokeListenerConsoleRow, ICustomCursorConsoleRow, IHoverConsoleRow
{
    private ConsolePrinter owner = null!;
    private readonly string text;
    private readonly int displayedLength;

    private int scrollIndex = 0;

    public ScrollableTextConsoleRow(string text, int displayedLength)
    {
        this.text = text;
        this.displayedLength = displayedLength;
    }

    public string GetCustomCursor() => "\u00BB";
    public string GetCustomCursorBackground() => " ";

    public IRenderable GetRenderContent()
    {
        return text.Length <= displayedLength ? new Text(text) : (IRenderable)new Text(text[scrollIndex..(displayedLength - scrollIndex)]);
    }

    public void OnHoverEnd()
    {
        scrollIndex = 0;
    }

    public void OnHoverStart()
    {

    }

    public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        if (keystrokeInfo.Key == ConsoleKey.RightArrow)
            scrollIndex = Math.Clamp(scrollIndex + 1, 0, Math.Max(0, text.Length - displayedLength));
        if (keystrokeInfo.Key == ConsoleKey.LeftArrow)
            scrollIndex = Math.Clamp(scrollIndex - 1, 0, Math.Max(0, text.Length - displayedLength));
    }

    public void SetOwnership(ConsolePrinter owner)
    {
        this.owner = owner;
    }
}
