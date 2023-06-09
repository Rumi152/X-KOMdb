﻿using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;
using Spectre.Console.Rendering;
using System.Diagnostics;
using System.Text.Json;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI.ConsoleRows.ProductDetails;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States;

public class ProductViewState : ViewState
{
    private readonly Product product;
    private bool isInPropertiesView = true;

    private readonly ViewState? backButtonTarget;
    private readonly string? backButtonMessage;

    private int reviewWriteStars = 0;
    private string reviewWriteDescription = "";

    public ProductViewState(ViewStateMachine stateMachine, Product product) : base(stateMachine)
    {
        this.product = product;
    }
    public ProductViewState(ViewStateMachine stateMachine, Product product, ViewState backButtonTarget, string backButtonMessage) : base(stateMachine)
    {
        this.product = product;
        this.backButtonTarget = backButtonTarget;
        this.backButtonMessage = backButtonMessage;
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        if (HasProductExpired())
            return;

        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        if(backButtonTarget is null)
        printer.AddRow(new InteractableConsoleRow(new Text("Back to searching"), (row, owner) => fsm.Checkout("productsSearch")));
        else
            printer.AddRow(new InteractableConsoleRow(new Markup(backButtonMessage ?? "Back"), (row, owner) => fsm.Checkout(backButtonTarget)));

        printer.StartGroup("favourite");
        printer.AddRow(new InteractableConsoleRow(new Text("Add to cart"), (row, owner) =>
        {
            if (!SessionData.IsLoggedIn())
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[{StandardRenderables.GrassColorHex}]Log in to use cart[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: this,
                    abortMarkupMessage: "Click to abort"
                ));
                return;
            }

            if (SessionData.HasSessionExpired(out User loggedUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to use cart[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: this,
                    abortMarkupMessage: "Click to abort"
                ));
                return;
            }

            using XkomContext context = new();

            //REFACTOR forgive me
            try { context.Attach(product); }
            catch (InvalidOperationException) { }
            try { context.Attach(loggedUser); }
            catch (InvalidOperationException) { }

            //add new active cart if doesnt exist
            if (!context.Carts.Any(x => x.Id == loggedUser.ActiveCartId))
            {
                var newCart = new Cart()
                {
                    User = loggedUser
                };

                context.Carts.Add(newCart);
                loggedUser.ActiveCart = newCart;
            }

            context.SaveChanges();

            Cart activeCart = context.Carts.Single(x => x.Id == loggedUser.ActiveCartId);

            var cartProduct = context.CartProducts.SingleOrDefault(x => x.CartId == activeCart.Id && x.ProductId == product.Id);
            if (cartProduct is null)
            {
                var newRecord = new CartProduct()
                {
                    Product = product,
                    Cart = activeCart,
                    Amount = 1
                };

                context.CartProducts.Add(newRecord);
                cartProduct = newRecord;
            }
            else
            {
                if (cartProduct.Amount < 99999)
                    cartProduct.Amount++;
            }

            context.SaveChanges();

            fsm.Checkout(new CartViewState(fsm));
        }));
        printer.AddRow(new InteractableConsoleRow(new Text("Add to list"), (row, owner) => fsm.Checkout(new ProductListViewState(fsm, product))));

        printer.AddRow(new Rule($"{product.Category?.Name} category").HeavyBorder().LeftJustified().RuleStyle(Style.Parse("#0e8f75")).ToBasicConsoleRow());

        printer.AddRow(new Text(product.Name).ToBasicConsoleRow());
        printer.AddRow(new Markup($"[lime]{product.Price:F2}[/] PLN").ToBasicConsoleRow());
        printer.AddRow(new Markup($"Made by {($"[{StandardRenderables.GrassColorHex}]" + product.Company?.Name.EscapeMarkup() + "[/]") ?? "Unknown company"}").ToBasicConsoleRow());
        printer.AddRow(new Markup($"[{((product.NumberAvailable == 0) ? "red" : StandardRenderables.GrassColorHex)}]{product.NumberAvailable}[/] left in magazine").ToBasicConsoleRow());

        printer.StartGroup("averageStars");
        RefreshAverageStars();

        printer.AddRow(new ReviewsAndPropertiesModeConsoleRow(ShowProperties, ShowReviews));
        printer.EnableScrolling();

        printer.StartGroup("properties");
        printer.StartGroup("reviews-chart");
        printer.StartGroup("reviews-input");
        printer.StartGroup("reviews-descriptionInput");
        printer.StartGroup("reviews-postInput");
        printer.StartGroup("reviews-postInput-errors");
        printer.StartGroup("reviews-all");

        ShowProperties();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (HasProductExpired())
            return;

        if (isInPropertiesView) ShowProperties();
        else ShowReviews();

        RefreshAverageStars();
        RefreshFavouriteButton();
    }


    private void ShowProperties()
    {
        if (HasProductExpired())
            return;

        isInPropertiesView = true;
        printer.ClearMemoryGroup("properties");
        printer.ClearMemoryGroup("reviews");

        if (product.Properties is null || product.Properties.IsNullOrEmpty())
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
        if (HasProductExpired())
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
            .OrderByDescending(x => x.User != null && SessionData.IsLoggedIn() && x.User.Id == SessionData.GetUserOffline()!.Id)
            .ToList();

        if (reviews.IsNullOrEmpty())
        {
            printer.AddRow(new Text("No reviews yet, share some thought about this product").ToBasicConsoleRow(), "reviews-chart");
            DisplayReviewInput(reviews);
            return;
        }

        DisplayReviewChart(reviews);

        printer.AddRow(new Text("").ToBasicConsoleRow(), "reviews-chart");
        DisplayReviewInput(reviews);

        DisplayAllReviews(reviews);
        RefreshAverageStars();
    }

    private void DisplayAllReviews(List<Review> reviews)
    {
        reviews.ForEach(x =>
        {
            printer.AddRow(new Text("").ToBasicConsoleRow(), "reviews-all");
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

        var descriptionLines = GetWrappedDescription((review.Description.Length == 0) ? "[dim]No description provided[/]" : review.Description);

        printer.AddRow(new Markup(header).ToBasicConsoleRow(), "reviews-all");
        descriptionLines.ForEach(x => printer.AddRow(((review.Description.Length == 0) ? (IRenderable)new Markup(x) : new Text(x)).ToBasicConsoleRow(), "reviews-all"));
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

            printer.AddRow(new Markup($"{stars} {barValue} {numberValue}").ToBasicConsoleRow(), "reviews-chart");
        }
    }

    private void DisplayReviewInput(List<Review> reviews)
    {
        void onClick(IConsoleRow row, ConsolePrinter? owner)
        {
            if (HasProductExpired())
                return;

            if (!SessionData.IsLoggedIn())
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[{StandardRenderables.GrassColorHex}]Log in to write reviews[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: this,
                    abortMarkupMessage: "Click to abort"
                ));
                return;
            }

            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to write reviews[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: this,
                    abortMarkupMessage: "Click to abort"
                ));
                return;
            }

            using var context = new XkomContext();
            context.Attach(dbUser);
            try
            {
                context.Attach(product);
            }
            catch(InvalidOperationException)
            {
                //REFACTOR please forgive me
            }

            printer.ClearMemoryGroup("reviews-postInput-errors");

            if (reviewWriteStars == 0)
            {
                printer.AddRow(new Markup("[red]Please select star rating[/]").ToBasicConsoleRow(), "reviews-postInput-errors");
                return;
            }

            var previousReviews = context.Reviews.Include(x => x.User).Include(x => x.Product).Where(x => x.UserId == dbUser.Id && x.ProductId == product.Id);
            if (!previousReviews.IsNullOrEmpty())
            {
                //printer.AddRow(new Markup("[red]You have already written review[/]").ToBasicConsoleRow(), "reviews-postInput-errors");
                context.RemoveRange(previousReviews);
                context.SaveChanges();
            }

            var review = new Review()
            {
                Product = product,
                Description = reviewWriteDescription,
                StarRating = reviewWriteStars,
                User = dbUser
            };
            context.Reviews.Add(review);
            context.SaveChanges();

            reviewWriteDescription = "";
            reviewWriteStars = 0;
            RefreshAverageStars();
            ShowReviews();
        }

        printer.AddRow(new ReviewInputHeaderConsoleRow(
            getRenderable: () =>
            {
                string stars = $"[yellow]{new string('*', reviewWriteStars)}[/][dim]{new string('*', 6 - reviewWriteStars)}[/]";
                string userDisplay = $"[{StandardRenderables.GoldColorHex}]Write[/]";
                string header = $"{userDisplay} {stars}";

                return new Markup(header);
            },
            onRightArrow: () =>
            {
                reviewWriteStars = Math.Clamp(reviewWriteStars + 1, 1, 6);
                printer.SetBufferDirty();
            },
            onLeftArrow: () =>
            {
                reviewWriteStars = Math.Clamp(reviewWriteStars - 1, 1, 6);
                printer.SetBufferDirty();
            }
        ), "reviews-input");

        DisplayReviewDescriptionInput();

        string acceptButtonText = reviews.Any(x => x != null && x.UserId == SessionData.GetUserOffline()?.Id) ? "  Click to replace review" : "  Click to write review";
        printer.AddRow(new InteractableConsoleRow(new Markup(acceptButtonText), onClick), "reviews-postInput");
    }

    private void DisplayReviewDescriptionInput()
    {
        printer?.ClearMemoryGroup("reviews-descriptionInput");

        var descriptionLines = GetWrappedDescription((reviewWriteDescription.Length == 0) ? "[dim]Write something about product (optional)[/]" : reviewWriteDescription);

        List<ReviewDescriptionInputConsoleRow> descriptionRows = new();
        descriptionRows = descriptionLines
            .Select((x, i) => new ReviewDescriptionInputConsoleRow(
                renderable: new Markup(x),
                isFirst: i == 0,
                isLast: i == descriptionLines.Count - 1,
                isDescriptionHoveredGetter: () => descriptionRows.Any(x => x.IsHovered),
                descriptionGetter: () => reviewWriteDescription,
                descriptionSetter: (description) => reviewWriteDescription = description,
                onCharacterWrite: () => DisplayReviewDescriptionInput()
            ))
            .ToList();

        descriptionRows.ForEach(x => printer?.AddRow(x, "reviews-descriptionInput"));
    }


    private void RefreshFavouriteButton()
    {
        printer.ClearMemoryGroup("favourite");

        if (HasProductExpired())
            return;

        using XkomContext context = new();

        if (SessionData.IsLoggedIn() && context.FavouriteProducts.Any(x => x.ProductId == product.Id && x.UserId == SessionData.GetUserOffline()!.Id))
            ShowRemoveFavouriteButton();
        else
            ShowAddFavouriteButton();
    }

    private void ShowAddFavouriteButton()
    {
        printer.AddRow(new InteractableConsoleRow(new Markup("[yellow dim]*[/]"), (row, owner) =>
        {
            if (SessionData.HasSessionExpired(out User loggedUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to add product to favourites[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: this,
                    abortMarkupMessage: "Click to abort"
                ));
                return;
            }

            using XkomContext context = new();
            //REFACTOR forgive me
            try { context.Attach(product); }
            catch (InvalidOperationException) { }
            try { context.Attach(loggedUser); }
            catch (InvalidOperationException) { }

            if (!context.FavouriteProducts.Any(x => x.ProductId == product.Id && x.UserId == loggedUser.Id))
            {
                context.FavouriteProducts.Add(new FavouriteProduct()
                {
                    Product = product,
                    User = loggedUser
                });
                context.SaveChanges();
            }

            RefreshFavouriteButton();

        }), "favourite");
    }

    private void ShowRemoveFavouriteButton()
    {
        printer.AddRow(new InteractableConsoleRow(new Markup("[yellow]*[/]"), (row, owner) =>
        {
            if (SessionData.HasSessionExpired(out User loggedUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to remove product from favourites[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: this,
                    abortMarkupMessage: "Click to abort"
                ));
                return;
            }

            using XkomContext context = new();

            var joinTable = context.FavouriteProducts.SingleOrDefault(x => x.ProductId == product.Id && x.UserId == loggedUser.Id);

            if (joinTable is not null)
            {
                context.Remove(joinTable);
                context.SaveChanges();
            }

            RefreshFavouriteButton();

        }), "favourite");
    }


    private void RefreshAverageStars()
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


    private bool HasProductExpired()
    {
        using var context = new XkomContext();

        if (context.Products.Any(x => x.Id == product.Id))
            return false;

        fsm.Checkout(new MessageViewState(fsm, "Product you are looking for no longer exists", fsm.GetSavedState("productsSearch"), "Back to searching"));
        return true;
    }


    private static List<string> GetWrappedDescription(string description)
    {
        description = description.ReplaceLineEndings(" ");
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

            int takenLength = new string(description.Take(width).ToArray()).LastIndexOf(' ');
            if (takenLength++ == -1) takenLength = width;

            descriptionLines.Add(leftPad + new string(description.Take(takenLength).ToArray()).Trim());
            description = new(description.Skip(takenLength).ToArray());
        }

        return descriptionLines;
    }
}
