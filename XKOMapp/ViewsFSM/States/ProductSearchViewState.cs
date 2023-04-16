using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI.ConsoleRows.ProductSearching;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States;

public class ProductSearchViewState : ViewState
{
    private readonly PriceRangeInputConsoleRow priceRangeInputConsoleRow;
    private readonly SearchContraintInputConsoleRow nameSearchInputRow;
    private readonly SearchContraintInputConsoleRow companySearchInputRow;

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
        //Category search
        //reset filters
        //orderby: newest, highest ratings, cheapest, most expensive

        const int namePadding = 8;
        priceRangeInputConsoleRow = new PriceRangeInputConsoleRow($"{"Price", -namePadding}: ", (row, printer) => RefreshProducts(), (row, printer) => RefreshProducts());
        nameSearchInputRow = new SearchContraintInputConsoleRow($"{"Name", -namePadding}: ", 32, (row, printer) => RefreshProducts(), (row, printer) => RefreshProducts());
        companySearchInputRow = new SearchContraintInputConsoleRow($"{"Company", -namePadding}: ", 64, (row, printer) => RefreshProducts(), (row, printer) => RefreshProducts());
        printer.AddRow(priceRangeInputConsoleRow);
        printer.AddRow(nameSearchInputRow);
        printer.AddRow(companySearchInputRow);

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
        var noCompanyConstraints = companySearchInputRow.currentInput.Length == 0;

        //TODO constraints and ordering
        var products = context.Products
            .Where(x => x.Name.Contains(nameSearchInputRow.currentInput))
            .Include(x => x.Company)
            .Where(x => noCompanyConstraints || (x.Company != null && x.Company.Name.Contains(companySearchInputRow.currentInput)))
            .Where(x => noPriceContraints || (x.Price >= ((priceRangeInputConsoleRow.LowestPrice.Length == 0) ? 0 : int.Parse(priceRangeInputConsoleRow.LowestPrice)) && x.Price <= ((priceRangeInputConsoleRow.HighestPrice.Length == 0) ? int.MaxValue : int.Parse(priceRangeInputConsoleRow.HighestPrice))))
            .ToList();

        if (products.Count == 0)
            printer?.AddRow(new Text("No products matching your criteria were found").ToBasicConsoleRow(), "products");

        products.ForEach(x =>
        {
            var priceString = x.NumberAvailable > 0 ? $"[lime]{x.Price,-9:F2}[/] PLN" : "[red]Unavailable[/]";
            var companyString = x.Company is null ? new string(' ', 32) : ((x.Company.Name.Length <= 29) ? $"{x.Company.Name,-29}" : $"{x.Company.Name[..30]}...");
            printer?.AddRow(new Markup($"{x.Name.EscapeMarkup(),-32} | {priceString + new string(' ', 13 - priceString.RemoveMarkup().Length)} | {companyString}").ToBasicConsoleRow(), "products");
        });
    }
}
