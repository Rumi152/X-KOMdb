using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows;

public class CompositeConsoleRow : IInteractableConsoleRow, ICustomCursorConsoleRow, ICustomLineSpanConsoleRow, IHoverConsoleRow, IFocusableConsoleRow, ICustomKeystrokeListenerConsoleRow
{
    private ConsolePrinter owner = null!;

    private bool isFocused = false;
    private IRenderable renderable = new Text("");
    private string cursor = ">";
    private string cursorBackground = " ";
    private int height = 1;
    private ConsoleRowAction? onHoverEnd;
    private ConsoleRowAction? onHoverStart;
    private ConsoleRowAction? onInteraction;
    private Action<IConsoleRow, ConsolePrinter, ConsoleKeyInfo>? onKeystroke;


    public CompositeConsoleRow SetIsFocused(bool value)
    {
        isFocused = value;
        return this;
    }
    public CompositeConsoleRow SetFocused() => SetIsFocused(true);
    public CompositeConsoleRow SetUnfocused() => SetIsFocused(false);

    public CompositeConsoleRow SetRenderable(IRenderable renderable)
    {
        this.renderable = renderable;
        return this;
    }

    public CompositeConsoleRow SetCursor(string value)
    {
        cursor = value;
        return this;
    }
    public CompositeConsoleRow SetCursorBackground(string value)
    {
        cursorBackground = value;
        return this;
    }

    public CompositeConsoleRow SetHeight(int value)
    {
        height = value;
        return this;
    }

    public CompositeConsoleRow SetOnHoverEnd(ConsoleRowAction action)
    {
        onHoverEnd = action;
        return this;
    }
    public CompositeConsoleRow SetOnHoverStart(ConsoleRowAction action)
    {
        onHoverStart = action;
        return this;
    }
    public CompositeConsoleRow SetOnInteraction(ConsoleRowAction action)
    {
        onInteraction = action;
        return this;
    }
    public CompositeConsoleRow SetOnKeystroke(Action<IConsoleRow, ConsolePrinter, ConsoleKeyInfo> action)
    {
        onKeystroke = action;
        return this;
    }



    bool ISwitchableConsoleRow.IsActive { get => isFocused; set => isFocused = value; }


    IRenderable IConsoleRow.GetRenderContent() => renderable;

    string ICustomCursorConsoleRow.GetCustomCursor() => cursor;
    string ICustomCursorConsoleRow.GetCustomCursorBackground() => cursorBackground;

    int ICustomLineSpanConsoleRow.GetRenderHeight() => height;

    void IHoverConsoleRow.OnHoverEnd() => onHoverEnd?.Invoke(this, owner);
    void IHoverConsoleRow.OnHoverStart() => onHoverStart?.Invoke(this, owner);
    void IInteractableConsoleRow.OnInteraction() => onInteraction?.Invoke(this, owner);
    void ICustomKeystrokeListenerConsoleRow.ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo) => onKeystroke?.Invoke(this, owner, keystrokeInfo);

    void IConsoleRow.SetOwnership(ConsolePrinter owner) => this.owner = owner;
    void ISwitchableConsoleRow.OnTurningOff() { }
    void ISwitchableConsoleRow.OnTurningOn() { }
}
