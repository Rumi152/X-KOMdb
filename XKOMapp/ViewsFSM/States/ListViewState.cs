using Spectre.Console;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI;
using XKOMapp.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using XKOMapp.GUI.ConsoleRows.ListAndCart;
using XKOMapp.GUI.ConsoleRows.Cart;

namespace XKOMapp.ViewsFSM.States;

internal class ListViewState : ViewState
{
    private readonly List<Product> ghosts = new();
    private readonly List list;
    private ListNameInputConsoleRow nameRow = null!;
    public ListViewState(ViewStateMachine stateMachine, List list) : base(stateMachine)
    {
        this.list = list;
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to browsing"), (row, owner) =>
        {
            fsm.Checkout("listBrowse");
        }));
        printer.AddRow(new Rule("List").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.StartGroup("name");
        printer.StartGroup("errors");

        printer.AddRow(new BasicConsoleRow(new Text($"Link: {list.Link}"))); //TODO long display support

        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

        printer.AddRow(new InteractableConsoleRow(new Text("Delete unavailable"), (row, own) =>
        {
            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to delete products[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            if (HasListExpired())
                return;

            using var context = new XkomContext();

            var unavailableProductsList = context.ListProducts
                .Include(x => x.Product)
                .Where(x => x.ListId == list.Id)
                .Where(x => x.Product.NumberAvailable <= 0);

            unavailableProductsList
                .Where(x => !ghosts.Contains(x.Product))
                .ToList()
                .ForEach(x => ghosts.Add(x.Product));

            context.RemoveRange(unavailableProductsList);
            context.SaveChanges();
            RefreshProducts();
        }));

        printer.AddRow(new InteractableConsoleRow(new Text("Clone list"), (row, own) =>
        {
            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to clone list[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            if (HasListExpired())
                return;

            using var context = new XkomContext();
            context.Attach(dbUser);

            string name = $"{list.Name}-copy";
            var clonedList = new List()
            {
                Name = name[..Math.Min(name.Length, 32)],
                Link = ListCreateViewState.GetLink(),
                User = dbUser
            };
            context.Add(clonedList);

            context.ListProducts
            .Include(x => x.Product)
                .Where(x => x.ListId == list.Id)
                .Select(x => new
                {
                    x.Product,
                    x.Number
                })
                .ToList()
                .ForEach(x =>
                {
                    var newProdList = new ListProduct()
                    {
                        Product = x.Product,
                        List = clonedList,
                        Number = x.Number
                    };
                    context.Add(newProdList);
                });

            context.SaveChanges();

            fsm.Checkout(new ListViewState(fsm, clonedList));
        }));
        printer.AddRow(new InteractableConsoleRow(new Markup("[red]Delete list[/]"), (row, own) =>
        {
            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to delete list[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            using var context = new XkomContext();
            var deleteList = context.Lists.SingleOrDefault(x => x.Id == list.Id);
            if (deleteList is not null)
            {
                context.Remove(deleteList);
                context.SaveChanges();
            }

            fsm.Checkout("listBrowse");
        }));

        printer.AddRow(new Rule("Products in this list").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.StartGroup("lists");
    }

    private void OnNameInputClick()
    {
        if (SessionData.HasSessionExpired(out User dbUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to save changes[/]",
                loginRollbackTarget: this,
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        if (HasListExpired())
            return;

        if (!ValidateInput())
            return;

        using var context = new XkomContext();
        context.Attach(list);

        list.Name = nameRow.CurrentInput;
        context.SaveChanges();

        RefreshName();
    }

    public override void OnEnter()
    {
        base.OnEnter();

        printer.ClearMemoryGroup("errors");
        printer.ResetCursor();
        RefreshProducts();
        RefreshName();
        ghosts.Clear();
    }

    private void RefreshProducts()
    {
        if (HasListExpired())
            return;

        RefreshGhosts();

        printer.ClearMemoryGroup("products");

        using var context = new XkomContext();
        var productsProj = context.ListProducts
            .Include(x => x.Product)
            .Where(x => x.List == list)
            .ToList()
            .Select(x => new
            {
                x.Product,
                x.Number
            })
            .Concat(ghosts.Select(x => new
            {
                Product = x,
                Number = 0
            }))
            .ToList();

        if (!productsProj.Any())
        {
            printer.AddRow(new Text("No products were found").ToBasicConsoleRow(), "products");
            return;
        }

        productsProj
            .OrderBy(x => x.Product.Name)
            .ToList()
            .ForEach(proj =>
            {
                printer.AddRow(new ProductInListConsoleRow
                    (
                        product: proj.Product,
                        productAmount: proj.Number,
                        onProductAmountChange: amount =>
                        {
                            if (SessionData.HasSessionExpired(out User loggedUser))
                            {
                                fsm.Checkout(new FastLoginViewState(fsm,
                                    markupMessage: $"[red]Session expired[/]",
                                    loginRollbackTarget: this,
                                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                                    abortMarkupMessage: "Back to main menu"
                                ));
                                return;
                            }

                            using XkomContext context = new();

                            //REFACTOR forgive me
                            try { context.Attach(loggedUser); }
                            catch (InvalidOperationException) { }
                            try { context.Attach(list); }
                            catch (InvalidOperationException) { }
                            try { context.Attach(proj.Product); }
                            catch (InvalidOperationException) { }

                            ListProduct? listProduct = context.ListProducts.SingleOrDefault(lp => lp.ListId == list.Id && lp.ProductId == proj.Product.Id);

                            if (amount == 0)
                            {
                                if (listProduct is not null)
                                    context.Remove(listProduct);

                                context.SaveChanges();

                                if (!ghosts.Contains(proj.Product))
                                    ghosts.Add(proj.Product);
                            }
                            else
                            {
                                if (listProduct is not null)
                                    listProduct.Number = amount;
                                else
                                {
                                    listProduct = new ListProduct()
                                    {
                                        List = list,
                                        Product = proj.Product,
                                        Number = amount
                                    };
                                    context.Add(listProduct);
                                }

                                context.SaveChanges();
                            }

                            RefreshProducts();
                        },
                        onInteraction: () => fsm.Checkout(new ProductViewState(fsm, proj.Product, this, "Back to list"))
                    ), "products");
            });
    }

    private void RefreshGhosts()
    {
        using XkomContext context = new();

        ghosts.RemoveAll(product => context.ListProducts.Any(pair => pair.ProductId == product.Id && pair.ListId == list.Id));
    }

    private void RefreshName()
    {
        if (HasListExpired())
            return;

        printer.ClearMemoryGroup("name");
        nameRow = new ListNameInputConsoleRow($"Name: {list.Name} | New name: ", 32, OnNameInputClick);
        printer.AddRow(nameRow, "name");
    }
    private bool ValidateInput()
    {
        string name = nameRow.CurrentInput;
        printer.ClearMemoryGroup("errors");

        if (name.Length == 0)
            return false;

        if (name.Length < 2)
        {
            printer.AddRow(new Markup("[red]Name is too short[/]").ToBasicConsoleRow(), "errors");
            return false;
        }

        if (name.Length > 32)
        {
            printer.AddRow(new Markup("[red]Name is too long[/]").ToBasicConsoleRow(), "errors");
            return false;
        }
        return true;
    }


    private bool HasListExpired()
    {
        using var context = new XkomContext();

        if (context.Lists.Any(x => x.Id == list.Id))
            return false;

        fsm.Checkout(new MessageViewState(fsm, "List no longer exists", fsm.GetSavedState("listBrowse"), "Back to browsing"));
        return true;
    }
}


