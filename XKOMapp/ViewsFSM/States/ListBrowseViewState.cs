using Spectre.Console;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.Models;

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

            fsm.Checkout(new ListCreateViewState(fsm));

        }));

        printer.AddRow(new Rule("Your lists").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());

        printer.StartGroup("lists");
    }

    public override void OnEnter()
    {
        base.OnEnter();
        printer?.ResetCursor();

        RefreshLists();

    }

    private void RefreshLists()
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

        printer?.ClearMemoryGroup("lists");
        using XkomContext context = new();
        context.Attach(dbUser);
        IQueryable<List> lists = context.Lists.Where(x => x.User == dbUser);
        if (!lists.Any())
            printer?.AddRow(new Text("No lists were found").ToBasicConsoleRow(), "lists");

        lists.ToList().ForEach(x =>
        {
            printer?.AddRow(new InteractableConsoleRow(new Markup(x.Name), (row, printer) =>
            {
                if (SessionData.HasSessionExpired(out User dbUser))
                {
                    fsm.Checkout(new FastLoginViewState(fsm,
                        markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to edit list[/]",
                        loginRollbackTarget: this,
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


