using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI.ConsoleRows.ProductSearching;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States;

public class ProductViewState : ViewState
{
    private readonly Product product;

    public ProductViewState(ViewStateMachine stateMachine, Product product) : base(stateMachine)
    {
        this.product = product;
        printer = new ConsolePrinter();

        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();
        printer.EnableScrolling();

        printer.AddRow(new Text(product.Name).ToBasicConsoleRow());
        printer.AddRow(new Markup($"[lime]{product.Price, -9:F2}[/] PLN").ToBasicConsoleRow());
        var panel = new Panel(product.Description).Header("Description").Expand().DoubleBorder();
        panel.Height = 7;
        printer.AddRow(new MultiLineConsoleRow(panel, 7));

        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());
    }

    public override void OnEnter()
    {
        base.OnEnter();

        Display();
    }

    protected override void OnKeystrokePassed(ConsoleKeyInfo info)
    {
        base.OnKeystrokePassed(info);

        printer.PassKeystroke(info);
    }

    protected override void OnKeystrokePassedFinally(ConsoleKeyInfo info)
    {
        base.OnKeystrokePassedFinally(info);

        Display();
    }
}
