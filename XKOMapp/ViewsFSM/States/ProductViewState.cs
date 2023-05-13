using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;
using System.Text.Json;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI.ConsoleRows.ProductDetails;
using XKOMapp.GUI.ConsoleRows.ProductSearching;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States;

public class ProductViewState : ViewState
{
    private readonly Product product;
    private bool isInPropertiesView = true;
    readonly ReviewInputPanelConsoleRow reviewInputPanel = new ReviewInputPanelConsoleRow();

    public ProductViewState(ViewStateMachine stateMachine, Product product) : base(stateMachine)
    {
        this.product = product;

        if (!AssureProductExists())
            return;

        printer = new ConsolePrinter();

        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to searching"), (row, owner) => fsm.Checkout("productsSearch")));
        printer.AddRow(new InteractableConsoleRow(new Text("Add to favourites"), (row, owner) => throw new NotImplementedException()));//TODO
        printer.AddRow(new InteractableConsoleRow(new Text("Add to cart"), (row, owner) => throw new NotImplementedException()));//TODO
        printer.AddRow(new InteractableConsoleRow(new Text("Add/Remove from list"), (row, owner) => throw new NotImplementedException()));//TODO

        printer.AddRow(new Rule($"{product.Category?.Name} category").HeavyBorder().LeftJustified().RuleStyle(Style.Parse("#0e8f75")).ToBasicConsoleRow());

        printer.AddRow(new Text(product.Name).ToBasicConsoleRow());
        printer.AddRow(new Markup($"[lime]{product.Price:F2}[/] PLN").ToBasicConsoleRow());
        printer.AddRow(new Markup($"Made by {($"[{StandardRenderables.GrassColorHex}]" + product.Company?.Name.EscapeMarkup() + "[/]") ?? "Unknown company"}").ToBasicConsoleRow());
        printer.AddRow(new Markup($"[{((product.NumberAvailable == 0) ? "#red" : StandardRenderables.GrassColorHex)}]{product.NumberAvailable}[/] left in magazine").ToBasicConsoleRow());

        printer.StartGroup("averageStars");
        ShowAverageStars();

        printer.AddRow(new ReviewsAndPropertiesModeConsoleRow(ShowProperties, ShowReviews));
        printer.EnableScrolling();

        printer.StartGroup("properties");
        printer.StartGroup("reviews");
        printer.StartGroup("allReviews");

        ShowProperties();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (isInPropertiesView) ShowProperties();
        else ShowReviews();

        ShowAverageStars();
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


    private void ShowProperties()
    {
        if (!AssureProductExists())
            return;

        isInPropertiesView = true;
        printer.ClearMemoryGroup("properties");
        printer.ClearMemoryGroup("reviews");

        if (product.Properties is null)
        {
            printer.AddRow(new Text("Product has no properties").ToBasicConsoleRow(), "properties");
            return;
        }

        try
        {
            Dictionary<string, object> properties = JsonSerializer.Deserialize<Dictionary<string, object>>(product.Properties)!;
            var longestKey = properties.Keys.Max(x => x.Length);
            properties.ToList().ForEach(x =>
            {
                string valueToPrint = x.Value switch
                {
                    bool => ((bool)x.Value) ? "Yes" : "No",
                    _ => x.Value.ToString() ?? "",
                };
                printer.AddRow(new Text($"{x.Key.PadRight(longestKey)} : {valueToPrint}").ToBasicConsoleRow(), "properties");
            });
        }
        catch
        {
            printer.AddRow(new Text("Failed to load product's properties").ToBasicConsoleRow(), "properties");
        }
    }

    private void ShowReviews()
    {
        if (!AssureProductExists())
            return;

        isInPropertiesView = false;
        printer.ClearMemoryGroup("properties");
        printer.ClearMemoryGroup("reviews");

        using var context = new XkomContext();
        var reviews = context
            .Reviews
            .Include(x => x.Product)
            .Include(x => x.User)
            .Where(x => x.ProductId == product.Id)
            .OrderByDescending(x => x.User != null && SessionData.GetUserOffline() != null && x.User.Id == SessionData.GetUserOffline()!.Id)
            .ToList();

        if (reviews.IsNullOrEmpty())
        {
            printer.AddRow(new Text("No reviews, share some thought about this product").ToBasicConsoleRow(), "reviews");
            DisplayReviewInput();
            return;
        }

        DisplayReviewChart(reviews);

        printer.AddRow(new Text("").ToBasicConsoleRow(), "reviews");
        DisplayReviewInput();

        DisplayAllReviews(reviews);
        ShowAverageStars();
    }


    private void DisplayAllReviews(List<Review> reviews)
    {
        reviews.ForEach(x =>
        {
            printer.AddRow(new Text("").ToBasicConsoleRow(), "allReviews");
            DisplayReview(x);
        });
    }

    private void DisplayReview(Review review)
    {
        int? offlineUserID = SessionData.GetUserOffline()?.Id;
        string stars = $"[yellow]{new string('*', review.StarRating)}[/][dim]{new string('*', 6 - review.StarRating)}[/]";

        string userDisplay;
        if (review.User is null)
            userDisplay = $"[[ deleted user ]]";
        else if (review.UserId == offlineUserID)
            userDisplay = $"[{StandardRenderables.GoldColorHex}]You[/]";
        else
            userDisplay = $"{review.User.Name} {review.User.LastName}";

        string header = $"{userDisplay} {stars}";

        string description;
        if (review.Description.Length == 0)
            description = "[dim]No description provided[/]";
        else
            description = review.Description.ReplaceLineEndings(" ");

        List<string> descriptionLines = new();
        while (description.Length > 0)
        {
            const string leftPad = "  ";
            int width = Math.Max(32, Console.WindowWidth - 8);

            if (description.RemoveMarkup().Length <= width)
            {
                descriptionLines.Add(leftPad + description.Trim());
                break;
            }

            int takenLength = new string(description.Take(width).ToArray()).LastIndexOf(' ') + 1;
            if (takenLength == -1) takenLength = width;

            descriptionLines.Add(leftPad + new string(description.Take(takenLength).ToArray()).Trim());
            description = new(description.Skip(takenLength).ToArray());
        }

        printer.AddRow(new Markup(header).ToBasicConsoleRow(), "allReviews");
        descriptionLines.ForEach(x => printer.AddRow(new Text(x).ToBasicConsoleRow(), "allReviews"));
    }

    private void DisplayReviewChart(List<Review> reviews)
    {
        const int chartWidth = 32;
        //doin' meth
        //get count of every start rating given
        List<int> values = Enumerable.Range(1, 6)
            .Select(x => reviews.Where(review => review.StarRating == x).Count())
            .ToList();

        //get number of star ratings product has the most
        int maxValue = values.Max();

        //map these amounts into 0..32 range
        List<int> mappedValues = values
            .Select(x => x / (float)maxValue * chartWidth)
            .Select(x => (int)Math.Ceiling(x))
            .ToList();

        //render char
        for (int i = 5; i >= 0; i--)
        {
            string stars = $"[yellow]{new string('*', i + 1)}[/][dim]{new string('*', 5 - i)}[/]";
            string barValue = $"[{StandardRenderables.GoldColorHex}]{new string('-', mappedValues[i])}[/]";
            string numberValue = $"{values[i]}";

            printer.AddRow(new Markup($"{stars} {barValue} {numberValue}").ToBasicConsoleRow(), "reviews");
        }
    }

    private void DisplayReviewInput()
    {
        printer.AddRow(reviewInputPanel, "reviews");

        void onClick(IConsoleRow row, ConsolePrinter? owner)
        {
            if (!AssureProductExists())
                return;

            if (!SessionData.IsLoggedIn())
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[{StandardRenderables.GrassColorHex}]Log in to write reviews[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                return;
            }

            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to write reviews[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                return;
            }

            using var context = new XkomContext();
            var converted = (InteractableDynamicConsoleRow)row;

            if (context.Reviews.Include(x => x.User).Include(x => x.Product).Any(x => x.UserId == dbUser.Id && x.ProductId == product.Id))
            {
                converted.SetMarkupText("Click to post review [red]Placeholder.ReviewAlreadyWritten[/]");//TODO
                return;
            }

            if (reviewInputPanel.StarRating == 0)
            {
                converted.SetMarkupText("Click to post review [red]Please select star rating[/]");
                return;
            }

            context.Attach(dbUser);
            context.Attach(product);
            var review = new Review()
            {
                Product = product,
                Description = reviewInputPanel.Description,
                StarRating = reviewInputPanel.StarRating,
                User = dbUser
            };
            context.Reviews.Add(review);
            context.SaveChanges();

            ShowReviews();
            ShowAverageStars();
        }

        printer.AddRow(new InteractableDynamicConsoleRow("Click to post review", onClick), "reviews");
    }

    private void ShowAverageStars()
    {
        printer.ClearMemoryGroup("averageStars");

        using (var context = new XkomContext())
        {
            var avgStars = context.Products
                .Where(x => x.Id == product.Id)
                .Include(x => x.Reviews)
                .Select(x => x.Reviews)
                .SingleOrDefault()
                ?.DefaultIfEmpty()
                ?.Average(x => x?.StarRating) ?? 0;
            int avgStarsRounded = (int)Math.Round(avgStars);
            printer.AddRow(new Markup("Average " + $"[yellow]{new string('*', avgStarsRounded)}[/][dim]{new string('*', 6 - avgStarsRounded)}[/] {avgStars:0.0}").ToBasicConsoleRow(), "averageStars");
        };
    }


    private bool AssureProductExists()
    {
        using var context = new XkomContext();

        if (context.Products.Any(x => x.Id == product.Id))
            return true;

        fsm.Checkout("productsSearch");
        return false;
    }
}
