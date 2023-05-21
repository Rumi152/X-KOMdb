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
    private readonly ChoiceMenuParentConsoleRow categorySearchChoiceParent;
    private readonly ChoiceMenuParentConsoleRow orderbyChoiceParent;

    public ProductSearchViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to main menu"), (row, owner) =>
        {
            fsm.Checkout("mainMenu");
        }));
        printer.AddRow(new Rule("Products").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        
        const int namePadding = 11;
        priceRangeInputConsoleRow = new PriceRangeInputConsoleRow($"{"Price",-namePadding}: ", RefreshProducts, RefreshProducts);
        nameSearchInputRow = new SearchContraintInputConsoleRow($"{"Name",-namePadding}: ", 32, RefreshProducts, RefreshProducts);
        companySearchInputRow = new SearchContraintInputConsoleRow($"{"Company",-namePadding}: ", 64, RefreshProducts, RefreshProducts);
        categorySearchChoiceParent = new ChoiceMenuParentConsoleRow($"{"Category",-namePadding}: ", 4, 2, RefreshProducts, RefreshCategories);
        //TODO favourites only
        List<ChoiceMenuChildConsoleRow> sortingOptions = new()
        {
            new ChoiceMenuChildConsoleRow("Newest"),
            new ChoiceMenuChildConsoleRow("Best rated"),
            new ChoiceMenuChildConsoleRow("Cheapest"),
            new ChoiceMenuChildConsoleRow("Most expensive", true)
        };
        orderbyChoiceParent = new ChoiceMenuParentConsoleRow($"{"Sorting by",-namePadding}: ", 5, 2, RefreshProducts, null);
        orderbyChoiceParent.SetChildren(sortingOptions);

        printer.AddRow(priceRangeInputConsoleRow);
        printer.AddRow(nameSearchInputRow);
        printer.AddRow(companySearchInputRow);
        printer.AddRow(categorySearchChoiceParent); printer.StartGroup("categorySearch");
        printer.AddRow(orderbyChoiceParent);
        sortingOptions.ForEach(x => printer.AddRow(x));

        printer.AddRow(new InteractableConsoleRow(new Rule("Click to reset filters").RuleStyle(Style.Parse("#0e8f75")), (row, printer) =>
        {
            priceRangeInputConsoleRow.ResetRange();
            nameSearchInputRow.ResetInput();
            companySearchInputRow.ResetInput();
            categorySearchChoiceParent.ResetCategory();
            orderbyChoiceParent.ResetCategory();
            RefreshProducts();
        }
        ));
        printer.EnableScrolling();
        printer.StartGroup("products");
    }

    public override void OnEnter()
    {
        base.OnEnter();

        RefreshProducts();
    }


    private void RefreshProducts()
    {
        printer?.ClearMemoryGroup("products");

        using var context = new XkomContext();

        var noPriceContraints = priceRangeInputConsoleRow.LowestPrice.Length == 0 && priceRangeInputConsoleRow.HighestPrice.Length == 0;
        var noCompanyConstraints = companySearchInputRow.currentInput.Length == 0;
        var noCategoryConstraints = categorySearchChoiceParent.GetCurrentCategory() == "All";

        var products = context.Products
            .Where(x => x.Name.Contains(nameSearchInputRow.currentInput))
            .Include(x => x.Company)
                .Where(x => noCompanyConstraints || (x.Company != null && x.Company.Name.Contains(companySearchInputRow.currentInput)))
            .Include(x => x.Category)
                .Where(x => noCategoryConstraints || (x.Category != null && x.Category.Name.Contains(categorySearchChoiceParent.GetCurrentCategory())))
            .Where(x => noPriceContraints || (x.Price >= ((priceRangeInputConsoleRow.LowestPrice.Length == 0) ? 0 : int.Parse(priceRangeInputConsoleRow.LowestPrice)) && x.Price <= ((priceRangeInputConsoleRow.HighestPrice.Length == 0) ? 999_999 : int.Parse(priceRangeInputConsoleRow.HighestPrice))));

        products = orderbyChoiceParent.GetCurrentCategory() switch
        {
            "Cheapest" => products.OrderBy(x => x.Price).ThenBy(x => x.Name),
            "Most expensive" => products.OrderByDescending(x => x.Price).ThenBy(x => x.Name),
            "Best rated" => products.Include(x => x.Reviews).OrderByDescending(x => x.Reviews.Average(x => x.StarRating)).ThenBy(x => x.Name),
            "Newest" or _ => products.OrderByDescending(x => x.IntroductionDate).ThenBy(x => x.Name)
        };

        if (!products.Any())
            printer?.AddRow(new Text("No products matching your criteria were found").ToBasicConsoleRow(), "products");

        products.ToList().ForEach(x =>
        {
            var priceString = x.NumberAvailable > 0 ? $"[lime]{x.Price,-9:F2}[/] PLN" : "[red]Unavailable[/]";
            var companyString = x.Company is null ? new string(' ', 32) : ((x.Company.Name.Length <= 29) ? $"{x.Company.Name,-29}" : $"{x.Company.Name[..30]}...");
            var displayString = $"{x.Name.EscapeMarkup(),-32} | {priceString + new string(' ', 13 - priceString.RemoveMarkup().Length)} | {companyString}";
            printer?.AddRow(new InteractableConsoleRow(new Markup(displayString), (row, printer) => fsm.Checkout(new ProductViewState(fsm, x))), "products");
        });
    }

    private void RefreshCategories()
    {
        printer?.ClearMemoryGroup("categorySearch");

        using var context = new XkomContext();

        List<ChoiceMenuChildConsoleRow> toAdd = context
            .ProductCategories
            .Select(x => x.Name)
            .OrderBy(x => x)
            .Select(x => new ChoiceMenuChildConsoleRow(x, false))
            .ToList();

        toAdd.Insert(0, new ChoiceMenuChildConsoleRow("All"));
        toAdd.Last().IsOnEnd = true;

        toAdd.ForEach(x => printer?.AddRow(x, "categorySearch"));
        categorySearchChoiceParent.SetChildren(toAdd);
    }
}
