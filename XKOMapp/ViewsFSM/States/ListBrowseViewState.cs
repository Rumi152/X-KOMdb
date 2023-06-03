using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI;
using Microsoft.EntityFrameworkCore.Update.Internal;
using XKOMapp.Models;
using Microsoft.EntityFrameworkCore;
using XKOMapp.GUI.ConsoleRows.ListAndCart;

namespace XKOMapp.ViewsFSM.States;
internal class ListBrowseViewState : ViewState
{
    private readonly ListLinkInputConsoleRow linkInput;
    public ListBrowseViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
        linkInput = new("Clone from link: ", 64, CloneFromLink);
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Click to abort"), (row, owner) =>
        {
            fsm.Checkout("mainMenu");
        }));
        printer.AddRow(new Rule("Lists").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.AddRow(linkInput);
        printer.StartGroup("link-error");

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

            using var context = new XkomContext();
            context.Attach(dbUser);
            var newList = new List()
            {
                Name = $"newList-{DateTime.Now:dd.MMM.yyyy-HH:mm}",
                Link = ListCreateViewState.GetLink(),
                User = dbUser
            };
            context.Add(newList);
            context.SaveChanges();

            RefreshLists();
        }));

        printer.AddRow(new Rule("Your lists").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());

        printer.StartGroup("lists");
    }

    public override void OnEnter()
    {
        base.OnEnter();
        RefreshLists();
        printer.ClearMemoryGroup("link-error");
    }

    private void RefreshLists()
    {
        if (SessionData.HasSessionExpired(out User loggedUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: "[red]Session expired[/]",
                loginRollbackTarget: this,
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        printer.ClearMemoryGroup("lists");

        using var context = new XkomContext();
        context.Attach(loggedUser);
        var lists = context.Lists.Where(x => x.User == loggedUser);
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

                fsm.Checkout(new ListViewState(fsm, x));
            }), "lists");
        });
    }


    private void CloneFromLink(string link)
    {
        if (SessionData.HasSessionExpired(out User loggedUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to add list[/]",
                loginRollbackTarget: this,
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        printer.ClearMemoryGroup("link-error");

        using XkomContext context = new();
        context.Attach(loggedUser);

        var list = context.Lists.SingleOrDefault(x => x.Link == link);

        if (list is null)
        {
            printer.AddRow(new Markup("[red]List not found[/]").ToBasicConsoleRow(), "link-error");
            return;
        }

        string name = $"{list.Name}-image";
        var clonedList = new List()
        {
            Name = name[..Math.Min(name.Length, 32)],
            Link = ListCreateViewState.GetLink(),
            User = loggedUser
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

        linkInput.ResetInput();
    }
}


