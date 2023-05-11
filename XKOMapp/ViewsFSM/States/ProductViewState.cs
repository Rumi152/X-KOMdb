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
    private bool propertiesView = true;
    readonly ReviewInputPanelConsoleRow reviewInputPanel = new ReviewInputPanelConsoleRow();

    public ProductViewState(ViewStateMachine stateMachine, Product product) : base(stateMachine)
    {
        this.product = product;
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

        ShowProperties();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (propertiesView) ShowProperties();
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
        propertiesView = true;
        printer.ClearMemoryGroup("properties");
        printer.ClearMemoryGroup("reviews");

        if (product.Properties is not null)
        {
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
        else
        {
            printer.AddRow(new Text("Failed to load product's properties").ToBasicConsoleRow(), "properties");
        }
    }

    private void ShowReviews()
    {
        propertiesView = false;
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
            printer.AddRow(new Text("").ToBasicConsoleRow(), "reviews");
            int? loggedUserID = SessionData.GetUserOffline()?.Id;

            var stars = $"[yellow]{new string('*', x.StarRating)}[/][dim]{new string('*', 6 - x.StarRating)}[/]";
            string header;
            if (x.User is null)
                header = $"| [[deleted user]] {stars} |";
            else if (x.UserId == loggedUserID)
                header = $"| [{StandardRenderables.GoldColorHex}][[You]][/] {stars} |";
            else
                header = $"| [[{x.User.Name} {x.User.LastName}]] {stars} |";

            string description;
            if (x.Description.Length == 0)
                description = "[dim]No description provided[/]";
            else
                description = x.Description.ReplaceLineEndings(" ");

            int descriptionHeight = (int)Math.Ceiling(description.Length / (Console.WindowWidth - 10f));

            var panel = new Panel(description).HeavyBorder();
            panel.Header = new PanelHeader(header);
            panel.Height = descriptionHeight + 2;
            panel.Width = 64;
            printer.AddRow(new MultiLineConsoleRow(panel, descriptionHeight + 2), "reviews");
        });
    }

    private void DisplayReviewChart(List<Review> reviews)
    {
        var barChart = new BarChart()
            .Width(32)
            .AddItem($"[yellow]{new string('*', 6)}[/][dim]{new string('*', 0)}[/]", reviews.Where(x => x.StarRating == 6).Count(), new Color(0xFF, 0xFF, 0x00))
            .AddItem($"[yellow]{new string('*', 5)}[/][dim]{new string('*', 1)}[/]", reviews.Where(x => x.StarRating == 5).Count(), new Color(0xFF, 0xED, 0x4B))
            .AddItem($"[yellow]{new string('*', 4)}[/][dim]{new string('*', 2)}[/]", reviews.Where(x => x.StarRating == 4).Count(), new Color(0xFC, 0xD1, 0x2A))
            .AddItem($"[yellow]{new string('*', 3)}[/][dim]{new string('*', 3)}[/]", reviews.Where(x => x.StarRating == 3).Count(), new Color(0xFF, 0xC3, 0x00))
            .AddItem($"[yellow]{new string('*', 2)}[/][dim]{new string('*', 4)}[/]", reviews.Where(x => x.StarRating == 2).Count(), new Color(0xF8, 0xB4, 0x12))
            .AddItem($"[yellow]{new string('*', 1)}[/][dim]{new string('*', 5)}[/]", reviews.Where(x => x.StarRating == 1).Count(), new Color(0xFF, 0xA6, 0x00));
        printer.AddRow(new MultiLineConsoleRow(barChart, 6), "reviews");
    }

    private void DisplayReviewInput()
    {
        printer.AddRow(reviewInputPanel, "reviews");

        ConsoleRowAction onClick = (row, owner) =>
        {
            var converted = (InteractableDynamicConsoleRow)row;

            if (!SessionData.IsLoggedIn())
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[{StandardRenderables.GrassColorHex}]Log in to write reviews[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.RollbackOrDefault(this)))); //TODO main menu rollback
                return;
            }
            
            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to write reviews[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.RollbackOrDefault(this)))); //TODO main menu rollback
                return;
            }

            using var context = new XkomContext();

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
        };

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
                .FirstOrDefault()
                ?.DefaultIfEmpty()
                ?.Average(x => x?.StarRating) ?? 0;
            int avgStarsRounded = (int)Math.Round(avgStars);
            printer.AddRow(new Markup("Average " + $"[yellow]{new string('*', avgStarsRounded)}[/][dim]{new string('*', 6 - avgStarsRounded)}[/] {avgStars:0.0}").ToBasicConsoleRow(), "averageStars");
        };
    }
}
