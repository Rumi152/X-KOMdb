using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI;
using XKOMapp.Models;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore;

namespace XKOMapp.ViewsFSM.States;

internal class CartViewState : ViewState
{
    public CartViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to menu"), (row, owner) =>
        {
            fsm.Checkout("mainMenu");
        }));


        printer.AddRow(new InteractableConsoleRow(new Text("Go to ordering"), (row, own) => throw new NotImplementedException()));//TODO ordering viewstate
        printer.AddRow(new InteractableConsoleRow(new Text("Empty this cart"), (row, own) =>
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

            var cartProducts = context.CartProducts.Where(x => x.CartId == loggedUser.ActiveCartId);
            context.RemoveRange(cartProducts);
            context.SaveChanges();
            RefreshProducts();
        }));
        printer.AddRow(new InteractableConsoleRow(new Text("Delete unavailable"), (row, own) =>
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

            var cartProducts = context.CartProducts.Where(x => x.CartId == loggedUser.ActiveCartId && x.Product.NumberAvailable <= 0);
            context.RemoveRange(cartProducts);
            context.SaveChanges();
            RefreshProducts();
        }));

        printer.AddRow(new Rule("Cart").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
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

        List<CartProduct> productsCart = context.CartProducts
            .Include(x => x.Product)
            .Include(x => x.Product.Company)
            .Include(x => x.Product.Category)
            .Where(x => x.CartId == loggedUser.ActiveCartId)
            .ToList();

        if (!productsCart.Any())
        {
            printer.AddRow(new BasicConsoleRow(new Markup("You have no products in cart")), "products");
            return;
        }

        productsCart.ForEach(x =>
        {
            var product = x.Product;

            string amountString = "1x";//$"{x.Amount}x"; TEMP
            string priceString = product.NumberAvailable > 0 ? $"[lime]{product.Price,-9:F2}[/] PLN" : "[red]Unavailable[/]";
            string companyString = product.Company is null ? new string(' ', 32) : ((product.Company.Name.Length <= 29) ? $"{product.Company.Name,-29}" : $"{product.Company.Name[..30]}...");
            string displayString = $"{amountString} {product.Name.EscapeMarkup(),-32} | {priceString + new string(' ', 13 - priceString.RemoveMarkup().Length)} | {companyString}";

            printer.AddRow(new InteractableConsoleRow(new Markup(displayString), (row, printer) => fsm.Checkout(new ProductViewState(fsm, product, this, "Back to cart"))), "products");
        });
    }
}

