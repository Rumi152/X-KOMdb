using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System.Text.Json;
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

        printer.AddRow(new InteractableConsoleRow(new Text("Back to searching"), (row, owner) => fsm.RollbackOrDefault(this)));
        printer.AddRow(new InteractableConsoleRow(new Text("Add to favourites"), (row, owner) => throw new NotImplementedException()));//TODO

        printer.AddRow(new Rule($"{product.Category?.Name} category").HeavyBorder().LeftJustified().RuleStyle(Style.Parse("#0e8f75")).ToBasicConsoleRow());

        printer.AddRow(new Text(product.Name).ToBasicConsoleRow());
        printer.AddRow(new Markup($"[lime]{product.Price:F2}[/] PLN").ToBasicConsoleRow());
        printer.AddRow(new Markup($"Made by {("[#96fa96]" + product.Company?.Name.EscapeMarkup() + "[/]") ?? "Unknown company"}").ToBasicConsoleRow());
        printer.AddRow(new Markup($"[#96fa96]{product.NumberAvailable}[/] left in magazine").ToBasicConsoleRow());

        printer.AddRow(new Rule("Properties").HeavyBorder().LeftJustified().RuleStyle(Style.Parse("#0e8f75")).ToBasicConsoleRow());
        printer.EnableScrolling();

        if (product.Properties is not null)
        {
            try
            {
                Dictionary<string, object> properties = JsonSerializer.Deserialize<Dictionary<string, object>>(product.Properties)!;
                var longestKey = properties.Keys.Max(x => x.Length);
                properties.ToList().ForEach(x =>
                {
                    printer.AddRow(new Text($"{x.Key.PadRight(longestKey)} : {x.Value}").ToBasicConsoleRow());
                });
            }
            catch
            {
                printer.AddRow(new Text("Failed to load product's properties").ToBasicConsoleRow());
            }
        }
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
