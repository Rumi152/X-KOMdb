﻿using Spectre.Console;
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
using XKOMapp.GUI.ConsoleRows.Cart;

namespace XKOMapp.ViewsFSM.States;

internal class CartViewState : ViewState
{
    private readonly List<Product> ghosts = new();


    public CartViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to searching porducts"), (row, owner) => fsm.Checkout("productsSearch")));
        printer.AddRow(new InteractableConsoleRow(new Text("Back to menu"), (row, owner) => fsm.Checkout("mainMenu")));
        printer.AddRow(new InteractableConsoleRow(new Markup("Go to ordering [red](COMING SOON)[/]"), (row, own) => 
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
            //fsm.Checkout(new OrderingViewState(fsm));
        }));

        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

        printer.AddRow(new InteractableConsoleRow(new Text("Snapshot to list"), (row, onwer) =>
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

            using var context = new XkomContext();
            context.Attach(loggedUser);

            var snapshotList = new List()
            {
                Name = $"cart-snapshot-{DateTime.Now:dd.MMM.yyyy-HH:mm}",
                Link = ListCreateViewState.GetLink(),
                User = loggedUser
            };
            context.Add(snapshotList);

            context.CartProducts
            .Include(x => x.Product)
                .Where(x => x.CartId == loggedUser.ActiveCartId)
                .Select(x => new
                {
                    x.Product,
                    x.Amount
                })
                .ToList()
                .ForEach(x =>
                {
                    var newProdList = new ListProduct()
                    {
                        Product = x.Product,
                        List = snapshotList,
                        Number = x.Amount
                    };
                    context.Add(newProdList);
                });

            context.SaveChanges();

            fsm.Checkout(new ListViewState(fsm, snapshotList));
        }));

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

            var cartProducts = context.CartProducts
                .Include(x => x.Product)
                .Where(x => x.CartId == loggedUser.ActiveCartId);

            cartProducts
                .Where(x => !ghosts.Contains(x.Product))
                .ToList()
                .ForEach(x => ghosts.Add(x.Product));

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

            var cartProducts = context.CartProducts
                .Include(x => x.Product)
                .Where(x => x.CartId == loggedUser.ActiveCartId && x.Product.NumberAvailable <= 0);

            cartProducts
                .Where(x => !ghosts.Contains(x.Product))
                .ToList()
                .ForEach(x => ghosts.Add(x.Product));

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

    public override void OnExit()
    {
        base.OnExit();
        ghosts.Clear();
    }


    private void RefreshProducts()
    {
        printer.SetBufferDirty();
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

        RefreshGhosts();

        using XkomContext context = new();

        var productsProj = context.CartProducts
            .Include(x => x.Product)
            .Include(x => x.Product.Company)
            .Include(x => x.Product.Category)
            .AsNoTracking()
            .Where(x => x.CartId == loggedUser.ActiveCartId)
            .ToList()
            .Select(x => new
            {
                Product = x.Product,
                Amount = x.Amount
            })
            .Concat(ghosts.Select(x => new
            {
                Product = x,
                Amount = 0
            }))
            .ToList();

        if (!productsProj.Any())
        {
            printer.AddRow(new BasicConsoleRow(new Markup("You have no products in cart")), "products");
            return;
        }

        productsProj
            .OrderBy(x => x.Product.Name)
            .ToList()
            .ForEach(projected =>
            {
                printer.AddRow(new ProductInListConsoleRow
                    (
                        product: projected.Product,
                        productAmount: projected.Amount,
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
                            try { context.Attach(projected.Product); }
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
                            CartProduct? cartProduct = context.CartProducts.SingleOrDefault(cp => cp.CartId == activeCart.Id && cp.ProductId == projected.Product.Id);

                            if (amount == 0)
                            {
                                if (cartProduct is not null)
                                    context.Remove(cartProduct);

                                context.SaveChanges();

                                if (!ghosts.Contains(projected.Product))
                                    ghosts.Add(projected.Product);
                            }
                            else
                            {
                                if (cartProduct is not null)
                                    cartProduct.Amount = amount;
                                else
                                {
                                    cartProduct = new CartProduct()
                                    {
                                        Cart = activeCart,
                                        Product = projected.Product,
                                        Amount = amount
                                    };
                                    context.Add(cartProduct);
                                }

                                context.SaveChanges();
                            }

                            RefreshProducts();
                        },
                        onInteraction: () => fsm.Checkout(new ProductViewState(fsm, projected.Product, this, "Back to cart"))
                    ), "products");
            });
    }

    private void RefreshGhosts()
    {
        using XkomContext context = new();

        ghosts.RemoveAll(product => context.CartProducts.Any(pair => pair.ProductId == product.Id && pair.CartId == SessionData.GetUserOffline()!.ActiveCartId));
    }
}

