using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows.ProductDetails;
internal class ReviewInputHeaderConsoleRow : ICustomCursorConsoleRow, ICustomKeystrokeListenerConsoleRow
{
    private readonly Action onRightArrow;
    private readonly Action onLeftArrow;
    private readonly Func<IRenderable> getRenderable;

    private ConsolePrinter owner = null!;

    public ReviewInputHeaderConsoleRow(Func<IRenderable> getRenderable, Action onRightArrow, Action onLeftArrow)
    {
        this.onRightArrow = onRightArrow;
        this.onLeftArrow = onLeftArrow;
        this.getRenderable = getRenderable;
    }


    public string GetCustomCursor() => "\u00BB";
    public string GetCustomCursorBackground() => " ";

    public IRenderable GetRenderContent() => getRenderable();
    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

    public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        if (keystrokeInfo.Key == ConsoleKey.RightArrow)
            onRightArrow();
        else if (keystrokeInfo.Key == ConsoleKey.LeftArrow)
            onLeftArrow();
    }

}
