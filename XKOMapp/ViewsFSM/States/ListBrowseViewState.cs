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

namespace XKOMapp.ViewsFSM.States;
internal class ListBrowseViewState : ViewState
{
    public ListBrowseViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
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
}


