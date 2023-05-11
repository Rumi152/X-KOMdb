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
        printer = new ConsolePrinter();

        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();


        printer.AddRow(new InteractableConsoleRow(new Text("Click to abort"), (row, owner) =>
        {
            //TODO
            fsm.RollbackOrDefault("mainMenu");
            throw new NotImplementedException();
        }));
        printer.AddRow(new Rule("Lists").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.AddRow(new InteractableConsoleRow(new Text("Add new list"), (row, owner) =>
        {
            if (!SessionData.IsLoggedIn())
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[{StandardRenderables.GrassColorHex}]Log in to edit list[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                return;
            }

            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to edit list[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                return;
            }

            fsm.Checkout(new ListCreateViewState(fsm));

        }));

        printer.AddRow(new Rule("Your lists").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());

        printer.StartGroup("lists");
    }

    public override void OnEnter()
    {
        base.OnEnter();
        RefreshLists();

        printer.ResetCursor();
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

    private void RefreshLists()
    {
        if (!SessionData.IsLoggedIn())
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                new Markup($"[{StandardRenderables.GrassColorHex}]Log in to edit list[/]").ToBasicConsoleRow(),
                new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
            return;
        }

        if (SessionData.HasSessionExpired(out User dbUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                new Markup($"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to edit list[/]").ToBasicConsoleRow(),
                new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
            return;
        }
        printer.ClearMemoryGroup("lists");
        using var context = new XkomContext();
        //TODO users list only
        context.Attach(dbUser);
        var x1 = context.Lists.Include(x => x.User).ToList();
            var lists = x1.Where(x => x.User == dbUser).ToList();
        if (!lists.Any())
            printer.AddRow(new Text("No lists were found").ToBasicConsoleRow(), "lists");

        lists.ToList().ForEach(x =>
        {
            printer.AddRow(new InteractableConsoleRow(new Markup(x.Name), (row, printer) =>
            {
                if (!SessionData.IsLoggedIn())
                {
                    fsm.Checkout(new FastLoginViewState(fsm,
                        new Markup($"[{StandardRenderables.GrassColorHex}]Log in to edit list[/]").ToBasicConsoleRow(),
                        new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                    return;
                }

                if (SessionData.HasSessionExpired(out User dbUser))
                {
                    fsm.Checkout(new FastLoginViewState(fsm,
                        new Markup($"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to edit list[/]").ToBasicConsoleRow(),
                        new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                    return;
                }

                fsm.Checkout(new ListViewState(fsm, x));
            }), "lists");
        });
    }
}


