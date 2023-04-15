using Spectre.Console;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States;

public class ProductSearchViewState : ViewState
{
    private readonly PriceRangeInputConsoleRow priceRangeInputConsoleRow;

    public ProductSearchViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
        //using var context = new XkomContext();

        //var newProduct1 = new Product();
        //newProduct1.Name = "Logitech G402 Mouse";
        //newProduct1.Description = "";
        //newProduct1.Price = 250.5m;
        //newProduct1.NumberAvailable = 124;
        //context.Add(newProduct1);

        //var newProduct2 = new Product();
        //newProduct2.Name = "Laptop HP 15s";
        //newProduct2.Description = "";
        //newProduct2.Price = 3199.99m;
        //newProduct2.NumberAvailable = 156;
        //context.Add(newProduct2);

        //var newProduct3 = new Product();
        //newProduct3.Name = "SSD Disk";
        //newProduct3.Description = "";
        //newProduct3.Price = 600m;
        //newProduct3.NumberAvailable = 0;
        //context.Add(newProduct3);

        //context.SaveChanges();

        printer = new ConsolePrinter();
        printer.EnableScrolling();

        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        //TODO
        //Company search
        //Name search
        //Category search
        //reset filters
        //orderby: newest, highest ratings, cheapest, most expensive

        printer.AddRow(new InteractableConsoleRow(new Text("Placeholder"), (row, printer) => RefreshProducts()));
        priceRangeInputConsoleRow = new PriceRangeInputConsoleRow((row, printer) => RefreshProducts(), (row, printer) => RefreshProducts());
        printer.AddRow(priceRangeInputConsoleRow);
        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());
        printer.StartGroup("products");
    }

    public override void OnEnter()
    {
        base.OnEnter();

        RefreshProducts();
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

    private void RefreshProducts()
    {
        printer?.ClearMemoryGroup("products");

        using var context = new XkomContext();

        var noPriceContraints = priceRangeInputConsoleRow.LowestPrice.Length == 0 && priceRangeInputConsoleRow.HighestPrice.Length == 0;
        //TODO constraints and ordering
        var products = context.Products
            .Where(x => noPriceContraints || (x.Price >= ((priceRangeInputConsoleRow.LowestPrice.Length == 0) ? 0 : int.Parse(priceRangeInputConsoleRow.LowestPrice)) && x.Price <= ((priceRangeInputConsoleRow.HighestPrice.Length == 0) ? 999999 : int.Parse(priceRangeInputConsoleRow.HighestPrice))))
            .ToList();

        if (products.Any())
        {
            products.ForEach(x =>
            {
                var priceString = x.NumberAvailable > 0 ? $"[lime]{x.Price,-9:0.00}[/] PLN" : "[red]Unavailable[/]";
                printer?.AddRow(new Markup($"{x.Name.EscapeMarkup(),-32} | {priceString}").ToBasicConsoleRow(), "products");
            });
        }
        else
            printer?.AddRow(new Text("No products matching your criteria were found").ToBasicConsoleRow(), "products");
    }
}
