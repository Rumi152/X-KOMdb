﻿using Spectre.Console;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI;
using XKOMapp.Models;
using XKOMapp.GUI.ConsoleRows.List;
using System.Linq;

namespace XKOMapp.ViewsFSM.States;

internal class ListViewState : ViewState
{
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

            var listProducts = context.ListProducts.Where(x => x.List == list).ToList();
            var productIds = listProducts.Select(x => x.ProductId).ToList();
            var unavailableProducts = context.Products.Where(x => productIds.Contains(x.Id) && x.NumberAvailable <= 0).ToList();
            var productRelationToDelete = listProducts.Where(x => unavailableProducts.Contains(x.Product)).ToList();

            if (productRelationToDelete is not null)
            {
                context.RemoveRange(productRelationToDelete);
                context.SaveChanges();
            }
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
            var clonedList = new List()
            {
                Name = $"{list.Name}-copy",
                Link = GetLink(),
                User = dbUser
            };
            context.Add(clonedList);

            var listProducts = context.ListProducts.Where(x => x.List == list).ToList();
            var productIds = listProducts.Select(x => x.ProductId).ToList();
            var products = context.Products.Where(x => productIds.Contains(x.Id)).ToList();
            if (products.Any())
            {
                products.ToList().ForEach(x =>
                {
                    var newProdList = new ListProduct()
                    {
                        ProductId = x.Id,
                        List = clonedList
                    };
                    context.Add(newProdList);
                });
            }
            context.SaveChanges();
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
        printer.StartGroup("errors");
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
    }
    private static string GetLink()
    {
        string link = @"https://www.x-kom.pl/list/";
        var random = new Random();
        List<int> notAvailalbe = new() { 58, 59, 60, 61, 62, 63, 64, 91, 92, 93, 94, 95, 96 };

        while (link.Length < 128)
        {
            int randomNumber = random.Next(48, 122);
            char a = Convert.ToChar(randomNumber);
            if (!notAvailalbe.Contains(a))
            {
                link += a;
            }
        }
        using (var context = new XkomContext())
            if (context.Lists.Any(x => x.Link == link))
                return GetLink();

        return link;
    }
    private void RefreshProducts()
    {
        printer.ClearMemoryGroup("products");

        using var context = new XkomContext();
        var listProducts = context.ListProducts.Where(x => x.List == list).ToList();
        var productIds = listProducts.Select(x => x.ProductId).ToList();
        var products = context.Products.Where(x => productIds.Contains(x.Id)).ToList();

        if (!products.Any())
            printer.AddRow(new Text("No products were found").ToBasicConsoleRow(), "products");
        
        products.ToList().ForEach(x =>
        {
            printer.AddRow(new InteractableConsoleRow(new Markup(x.Name), (row, printer) =>
            {
                if (SessionData.HasSessionExpired(out User dbUser))
                {
                    fsm.Checkout(new FastLoginViewState(fsm,
                        markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to see product[/]",
                        loginRollbackTarget: this,
                        abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                        abortMarkupMessage: "Back to main menu"
                    ));
                    return;
                }

                fsm.Checkout(new ProductViewState(fsm, x));
            }), "products");
        });
    }
    private void RefreshName()
    {
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


