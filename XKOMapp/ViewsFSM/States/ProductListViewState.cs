﻿using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States;
internal class ProductListViewState : ViewState
{
    private readonly Product product;
    public ProductListViewState(ViewStateMachine stateMachine, Product product) : base(stateMachine)
    {
        this.product = product;
    }
    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Click to abort"), (row, owner) =>
        {
            fsm.Checkout("productsSearch");
        }));
        printer.AddRow(new Rule("Lists").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.AddRow(new InteractableConsoleRow(new Text("Add new list"), (row, owner) =>
        {
            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to add list[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }
            //todo fastcreatelist
            fsm.Checkout(new ListCreateViewState(fsm));
        }));

        printer.AddRow(new Rule("Your lists").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());

        printer.StartGroup("lists");
    }

    public override void OnEnter()
    {
        if (SessionData.HasSessionExpired(out User dbUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: "[red]Session expired[/]",
                loginRollbackTarget: this,
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        base.OnEnter();

        printer.ClearMemoryGroup("lists");

        using var context = new XkomContext();
        context.Attach(dbUser);
        var lists = context.Lists.Where(x => x.User == dbUser);
        if (!lists.Any())
            printer.AddRow(new Text("No lists were found").ToBasicConsoleRow(), "lists");

        lists.ToList().ForEach(x =>
        {
            printer.AddRow(new InteractableConsoleRow(new Markup(x.Name), (row, printer) =>
            {
                if (SessionData.HasSessionExpired(out User dbUser))
                {
                    fsm.Checkout(new FastLoginViewState(fsm,
                        markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to edit list[/]",
                        loginRollbackTarget: new ListViewState(fsm, x),
                        abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                        abortMarkupMessage: "Back to main menu"
                    ));
                    return;
                }
                //todo czekboksy
                using var context = new XkomContext();
                var newProdList = new ListProduct()
                {
                    ProductId = product.Id,
                    ListId = x.Id,
                };
                context.Add(newProdList);
                context.SaveChanges();

                fsm.Checkout(new ProductViewState(fsm, product));
            }), "lists");
        });
    }
}
