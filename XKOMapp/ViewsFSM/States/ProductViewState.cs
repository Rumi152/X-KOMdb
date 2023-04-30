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

    public ProductViewState(ViewStateMachine stateMachine, Product product) : base(stateMachine)
    {
        this.product = product;
        printer = new ConsolePrinter();

        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to searching"), (row, owner) => fsm.RollbackOrDefault("productsSearch")));
        printer.AddRow(new InteractableConsoleRow(new Text("Add to favourites"), (row, owner) => throw new NotImplementedException()));//TODO
        printer.AddRow(new InteractableConsoleRow(new Text("Add to cart"), (row, owner) => throw new NotImplementedException()));//TODO
        printer.AddRow(new InteractableConsoleRow(new Text("Add/Remove from list"), (row, owner) => throw new NotImplementedException()));//TODO

        printer.AddRow(new Rule($"{product.Category?.Name} category").HeavyBorder().LeftJustified().RuleStyle(Style.Parse("#0e8f75")).ToBasicConsoleRow());

        printer.AddRow(new Text(product.Name).ToBasicConsoleRow());
        printer.AddRow(new Markup($"[lime]{product.Price:F2}[/] PLN").ToBasicConsoleRow());
        printer.AddRow(new Markup($"Made by {("[#96fa96]" + product.Company?.Name.EscapeMarkup() + "[/]") ?? "Unknown company"}").ToBasicConsoleRow());
        printer.AddRow(new Markup($"[#96fa96]{product.NumberAvailable}[/] left in magazine").ToBasicConsoleRow());

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

        printer.ResetCursor();
        ShowProperties();
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
        printer.ClearMemoryGroup("properties");
        printer.ClearMemoryGroup("reviews");

        using var context = new XkomContext();
        var reviews = context
            .Reviews
            .Include(x => x.Product)
            .Include(x => x.User)
            .Where(x => x.ProductId == product.Id)
            .OrderBy(x => x.User != null && x.User.Id == SessionData.LoggedUserID)
            .ToList();

        if (reviews.IsNullOrEmpty())
        {
            printer.AddRow(new Text("No reviews, share some thought about this product").ToBasicConsoleRow(), "reviews");
            DisplayReviewInput();
            return;
        }

        var barChart = new BarChart()
            .Width(64)
            .AddItem($"[yellow]{new string('*', 6)}[/][dim]{new string('*', 0)}[/]", reviews.Where(x => x.StarRating == 6).Count(), new Color(0xFF, 0xFF, 0x00))
            .AddItem($"[yellow]{new string('*', 5)}[/][dim]{new string('*', 1)}[/]", reviews.Where(x => x.StarRating == 5).Count(), new Color(0xFF, 0xED, 0x4B))
            .AddItem($"[yellow]{new string('*', 4)}[/][dim]{new string('*', 2)}[/]", reviews.Where(x => x.StarRating == 4).Count(), new Color(0xFC, 0xD1, 0x2A))
            .AddItem($"[yellow]{new string('*', 3)}[/][dim]{new string('*', 3)}[/]", reviews.Where(x => x.StarRating == 3).Count(), new Color(0xFF, 0xC3, 0x00))
            .AddItem($"[yellow]{new string('*', 2)}[/][dim]{new string('*', 4)}[/]", reviews.Where(x => x.StarRating == 2).Count(), new Color(0xF8, 0xB4, 0x12))
            .AddItem($"[yellow]{new string('*', 1)}[/][dim]{new string('*', 5)}[/]", reviews.Where(x => x.StarRating == 1).Count(), new Color(0xFF, 0xA6, 0x00));
        printer.AddRow(new MultiLineConsoleRow(barChart, 6), "reviews");

        printer.AddRow(new Text("").ToBasicConsoleRow(), "reviews");
        DisplayReviewInput();

        reviews.ForEach(x =>
        {
            printer.AddRow(new Text("").ToBasicConsoleRow(), "reviews");

            var stars = $"[yellow]{new string('*', x.StarRating)}[/][dim]{new string('*', 6 - x.StarRating)}[/]";
            string header;
            if (x.User is null)
                header = $"| [[deleted user]] {stars} |";
            else if (x.User.Id == SessionData.LoggedUserID)
                header = $"| [lime][[You]][/] {stars} |";
            else
                header = $"| [[{x.User.Name} {x.User.LastName}]] {stars} |";

            string description = x.Description.ReplaceLineEndings(" ");
            int descriptionHeight = (int)Math.Ceiling(description.Length / (Console.WindowWidth - 10f));

            var panel = new Panel(description).HeavyBorder();
            panel.Header = new PanelHeader(header);
            panel.Height = descriptionHeight + 2;
            panel.Width = 64;
            printer.AddRow(new MultiLineConsoleRow(panel, descriptionHeight + 2), "reviews");
        });
    }

    private void DisplayReviewInput()
    {
        ReviewInputPanelConsoleRow panel = new ReviewInputPanelConsoleRow();
        printer.AddRow(panel, "reviews");

        printer.AddRow(new InteractableDynamicConsoleRow("Click to post review", (row, owner) =>
        {
            var converted = (InteractableDynamicConsoleRow)row;

            if (!SessionData.IsLoggedIn())
            {
                converted.SetMarkupText("Click to post review [red]Placeholder.NotLoggedIn[/]");
                return;
            }

            if (SessionData.HasSessionExpired(out User dbUser))
            {
                converted.SetMarkupText("Click to post review [red]Placeholder.SessionExpired[/]");
                return;
            }

            using var context = new XkomContext();

            if (context.Reviews.Include(x => x.User).Any(x => x.UserId == dbUser.Id && x.ProductId == product.Id))
            {
                converted.SetMarkupText("Click to post review [red]Placeholder.ReviewAlreadyWritten[/]");
                return;
            }

            if (panel.StarRating == 0)
            {
                converted.SetMarkupText("Click to post review [red]Please select star rating[/]");
                return;
            }

            context.Attach(dbUser);
            context.Attach(product);
            var review = new Review()
            {
                Product = product,
                Description = panel.Description,
                StarRating = panel.StarRating,
                User = dbUser
            };
            context.Reviews.Add(review);
            context.SaveChanges();

            ShowReviews();
            ShowAverageStars();
        }
        ), "reviews");
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
            printer.AddRow(new Markup("Average " + $"[yellow]{new string('*', avgStarsRounded)}[/][dim]{new string('*', 6 - avgStarsRounded)}[/] {avgStars}").ToBasicConsoleRow(), "averageStars");
        };
    }
}
