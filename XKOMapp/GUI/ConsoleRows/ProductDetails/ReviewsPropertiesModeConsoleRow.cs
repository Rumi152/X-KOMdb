using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows.ProductDetails;

public class ReviewsAndPropertiesModeConsoleRow : ISwitchableConsoleRow, ICustomCursorConsoleRow, ICustomKeystrokeListenerConsoleRow
{
    private ConsolePrinter owner = null!;

    private readonly Action onPropertiesEnter;
    private readonly Action onReviewsEnter;

    private bool isActive = false;
    bool ISwitchableConsoleRow.IsActive { get => isActive; set => isActive = value; }


    public ReviewsAndPropertiesModeConsoleRow(Action onPropertiesEnter, Action onReviewsEnter)
    {
        this.onPropertiesEnter = onPropertiesEnter;
        this.onReviewsEnter = onReviewsEnter;
    }

    public string GetCustomCursor() => "\u00BB";
    public string GetCustomCursorBackground() => " ";

    public IRenderable GetRenderContent() => new Rule(isActive ? "[dim]Properties[/] ── Reviews" : "Properties ── [dim]Reviews[/]").HeavyBorder().LeftJustified().RuleStyle(Style.Parse("#0e8f75"));
    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

    public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        if (keystrokeInfo.Key == ConsoleKey.RightArrow)
            ((ISwitchableConsoleRow)this).TurnOn();
        if (keystrokeInfo.Key == ConsoleKey.LeftArrow)
            ((ISwitchableConsoleRow)this).TurnOff();
    }

    void ISwitchableConsoleRow.OnTurningOff()
    {
        onPropertiesEnter();
        owner.SetBufferDirty();
    }

    void ISwitchableConsoleRow.OnTurningOn()
    {
        onReviewsEnter();
        owner.SetBufferDirty();
    }
}
