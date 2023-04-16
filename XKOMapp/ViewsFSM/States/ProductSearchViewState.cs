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
    private readonly CategorySearchParentConsoleRow categorySearchChoiceParent;

    public ProductSearchViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
        printer = new ConsolePrinter();
        printer.EnableScrolling();

        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        //TODO
        //orderby: newest, highest ratings, cheapest, most expensive

        const int namePadding = 9;
        priceRangeInputConsoleRow = new PriceRangeInputConsoleRow($"{"Price",-namePadding}: ", (row, printer) => RefreshProducts(), (row, printer) => RefreshProducts());
        nameSearchInputRow = new SearchContraintInputConsoleRow($"{"Name",-namePadding}: ", 32, (row, printer) => RefreshProducts(), (row, printer) => RefreshProducts());
        companySearchInputRow = new SearchContraintInputConsoleRow($"{"Company",-namePadding}: ", 64, (row, printer) => RefreshProducts(), (row, printer) => RefreshProducts());
        categorySearchChoiceParent = new CategorySearchParentConsoleRow($"{"Category",-namePadding}: ", 4, 2, (row, printer) => RefreshProducts(), (row, printer) => RefreshCategories());
        printer.AddRow(priceRangeInputConsoleRow);
        printer.AddRow(nameSearchInputRow);
        printer.AddRow(companySearchInputRow);
        printer.AddRow(categorySearchChoiceParent); printer.StartGroup("categorySearch");

        printer.AddRow(new InteractableConsoleRow(new Rule("Click to reset filters").RuleStyle(Style.Parse("#0e8f75")), (row, printer) =>
        {
            priceRangeInputConsoleRow.ResetRange();
            nameSearchInputRow.ResetInput();
            companySearchInputRow.ResetInput();
            categorySearchChoiceParent.ResetCategory();
            RefreshProducts();
        }
        ));
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
        printer.ClearMemoryGroup("products");

        using var context = new XkomContext();

        var noPriceContraints = priceRangeInputConsoleRow.LowestPrice.Length == 0 && priceRangeInputConsoleRow.HighestPrice.Length == 0;
        var noCompanyConstraints = companySearchInputRow.currentInput.Length == 0;
        var noCategoryConstraints = categorySearchChoiceParent.GetCurrentCategory() == "All";

        //TODO constraints and ordering
        var products = context.Products
            .Where(x => x.Name.Contains(nameSearchInputRow.currentInput))
            .Include(x => x.Company)
                .Where(x => noCompanyConstraints || (x.Company != null && x.Company.Name.Contains(companySearchInputRow.currentInput)))
            .Include(x => x.Category)
                .Where(x => noCategoryConstraints || (x.Category != null && x.Category.Name.Contains(categorySearchChoiceParent.GetCurrentCategory())))
            .Where(x => noPriceContraints || (x.Price >= ((priceRangeInputConsoleRow.LowestPrice.Length == 0) ? 0 : int.Parse(priceRangeInputConsoleRow.LowestPrice)) && x.Price <= ((priceRangeInputConsoleRow.HighestPrice.Length == 0) ? int.MaxValue : int.Parse(priceRangeInputConsoleRow.HighestPrice))))
            .ToList();

        if (products.Count == 0)
            printer.AddRow(new Text("No products matching your criteria were found").ToBasicConsoleRow(), "products");

        products.ForEach(x =>
        {
            var priceString = x.NumberAvailable > 0 ? $"[lime]{x.Price,-9:F2}[/] PLN" : "[red]Unavailable[/]";
            var companyString = x.Company is null ? new string(' ', 32) : ((x.Company.Name.Length <= 29) ? $"{x.Company.Name,-29}" : $"{x.Company.Name[..30]}...");
            printer.AddRow(new Markup($"{x.Name.EscapeMarkup(),-32} | {priceString + new string(' ', 13 - priceString.RemoveMarkup().Length)} | {companyString}").ToBasicConsoleRow(), "products");
        });
    }

    private void RefreshCategories()
    {
        printer.ClearMemoryGroup("categorySearch");

        using var context = new XkomContext();

        List<CategorySearchChildConsoleRow> toAdd = context
            .ProductCategories
            .Select(x => x.Name)
            .OrderBy(x => x)
            .Select(x => new CategorySearchChildConsoleRow(x))
            .ToList();

        toAdd.Insert(0, new CategorySearchChildConsoleRow("All"));

        toAdd.ForEach(x => printer.AddRow(x, "categorySearch"));
        categorySearchChoiceParent.SetChildren(toAdd);
    }
}
