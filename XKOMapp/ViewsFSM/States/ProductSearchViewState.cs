using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI.ConsoleRows.ProductSearching;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States;

public class ProductSearchViewState : ViewState
{
    const int namePadding = 15;
    private readonly PriceRangeInputConsoleRow priceRangeInputConsoleRow;
    private readonly FavouritesCheckboxConsoleRow favouritesOnlyInputConsoleRow;
    private readonly SearchContraintInputConsoleRow nameSearchInputRow;
    private readonly SearchContraintInputConsoleRow companySearchInputRow;
    private readonly ChoiceMenuParentConsoleRow categorySearchChoiceParent;
    private readonly ChoiceMenuParentConsoleRow orderbyChoiceParent;
    private readonly List<ChoiceMenuChildConsoleRow> sortingOptions = new()
        {
            new ChoiceMenuChildConsoleRow("Newest"),
            new ChoiceMenuChildConsoleRow("Best rated"),
            new ChoiceMenuChildConsoleRow("Cheapest"),
            new ChoiceMenuChildConsoleRow("Most expensive", true)
        };

    public ProductSearchViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
        priceRangeInputConsoleRow = new($"{"Price",-namePadding}: ", RefreshProducts, RefreshProducts);
        favouritesOnlyInputConsoleRow = new($"{"Only favourites",-namePadding}  ", OnFavouritesOnlyInteraction);
        nameSearchInputRow = new($"{"Name",-namePadding}: ", 32, RefreshProducts, RefreshProducts);
        companySearchInputRow = new($"{"Company",-namePadding}: ", 64, RefreshProducts, RefreshProducts);
        categorySearchChoiceParent = new($"{"Category",-namePadding}: ", 4, 2, RefreshProducts, RefreshCategories);

        orderbyChoiceParent = new($"{"Sorting by",-namePadding}: ", 5, 2, RefreshProducts, null);
        orderbyChoiceParent.SetChildren(sortingOptions);
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to main menu"), (row, owner) =>
        {
            fsm.Checkout("mainMenu");
            ResetFilters();
        }));
        printer.AddRow(new Rule("Products").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());

        printer.AddRow(priceRangeInputConsoleRow);
        printer.AddRow(favouritesOnlyInputConsoleRow);
        printer.AddRow(nameSearchInputRow);
        printer.AddRow(companySearchInputRow);
        printer.AddRow(categorySearchChoiceParent);
        printer.StartGroup("categorySearch");
        printer.AddRow(orderbyChoiceParent);
        sortingOptions.ForEach(x => printer.AddRow(x));

        printer.AddRow(new InteractableConsoleRow(new Rule("Click to reset filters").RuleStyle(Style.Parse("#0e8f75")), (row, printer) =>
        {
            ResetFilters();
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
        printer.ClearMemoryGroup("products");

        if (!SessionData.IsLoggedIn())
            favouritesOnlyInputConsoleRow.Uncheck();

        using var context = new XkomContext();

        var noPriceContraints = priceRangeInputConsoleRow.LowestPrice.Length == 0 && priceRangeInputConsoleRow.HighestPrice.Length == 0;
        var noCompanyConstraints = companySearchInputRow.currentInput.Length == 0;
        var noCategoryConstraints = categorySearchChoiceParent.GetCurrentCategory() == "All";

        var products = context.Products
            .Where(x => x.Name.Contains(nameSearchInputRow.currentInput))
            .Include(x => x.Company)
            .Include(x => x.Category)
            .Include(x => x.FavouriteProducts)
            .Where(x => !favouritesOnlyInputConsoleRow.IsChecked || (SessionData.IsLoggedIn() && x.FavouriteProducts.Any(pair => pair.ProductId == x.Id && pair.UserId == SessionData.GetUserOffline()!.Id)))
            .Where(x => noCompanyConstraints || (x.Company != null && x.Company.Name.Contains(companySearchInputRow.currentInput)))
            .Where(x => noCategoryConstraints || (x.Category != null && x.Category.Name.Contains(categorySearchChoiceParent.GetCurrentCategory())))
            .Where(x => noPriceContraints || (x.Price >= ((priceRangeInputConsoleRow.LowestPrice.Length == 0) ? 0 : int.Parse(priceRangeInputConsoleRow.LowestPrice)) && x.Price <= ((priceRangeInputConsoleRow.HighestPrice.Length == 0) ? 999_999 : int.Parse(priceRangeInputConsoleRow.HighestPrice))));

        products = orderbyChoiceParent.GetCurrentCategory() switch
        {
            "Cheapest" => products.OrderBy(x => x.Price).ThenByDescending(x => x.Name),
            "Most expensive" => products.OrderByDescending(x => x.Price).ThenByDescending(x => x.Name),
            "Best rated" => products.Include(x => x.Reviews).OrderByDescending(x => x.Reviews.Average(x => x.StarRating)).ThenByDescending(x => x.Name),
            "Newest" => products.OrderByDescending(x => x.IntroductionDate).ThenByDescending(x => x.Name),
            _ => products.OrderByDescending(x => x.Name)
        };

        if (!products.Any())
            printer.AddRow(new Text("No products matching your criteria were found").ToBasicConsoleRow(), "products");

        products.ToList().ForEach(x =>
        {
            var priceString = x.NumberAvailable > 0 ? $"[lime]{x.Price,-9:F2}[/] PLN" : "[red]Unavailable[/]";
            var companyString = x.Company is null ? new string(' ', 32) : ((x.Company.Name.Length <= 29) ? $"{x.Company.Name,-29}" : $"{x.Company.Name[..30]}...");
            var displayString = $"{x.Name.EscapeMarkup(),-32} | {priceString + new string(' ', 13 - priceString.RemoveMarkup().Length)} | {companyString}";
            printer.AddRow(new InteractableConsoleRow(new Markup(displayString), (row, printer) => fsm.Checkout(new ProductViewState(fsm, x))), "products");
        });
    }

    private void RefreshCategories()
    {
        printer.ClearMemoryGroup("categorySearch");

        using var context = new XkomContext();

        List<ChoiceMenuChildConsoleRow> toAdd = context
            .ProductCategories
            .Select(x => x.Name)
            .OrderBy(x => x)
            .Select(x => new ChoiceMenuChildConsoleRow(x, false))
            .ToList();

        toAdd.Insert(0, new ChoiceMenuChildConsoleRow("All"));
        toAdd.Last().IsOnEnd = true;

        toAdd.ForEach(x => printer.AddRow(x, "categorySearch"));
        categorySearchChoiceParent.SetChildren(toAdd);
    }

    private void ResetFilters()
    {
        priceRangeInputConsoleRow.ResetRange();
        favouritesOnlyInputConsoleRow.Uncheck();
        nameSearchInputRow.ResetInput();
        companySearchInputRow.ResetInput();
        categorySearchChoiceParent.ResetCategory();
        orderbyChoiceParent.ResetCategory();
    }


    private void OnFavouritesOnlyInteraction()
    {
        if (favouritesOnlyInputConsoleRow.IsChecked && !SessionData.IsLoggedIn())
        {
            favouritesOnlyInputConsoleRow.Uncheck();

            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: $"[{StandardRenderables.GrassColorHex}]Log in to search your favourite products[/]",
                loginRollbackTarget: this,
                abortRollbackTarget: this
            ));
            return;
        }

        RefreshProducts();
    }
}
