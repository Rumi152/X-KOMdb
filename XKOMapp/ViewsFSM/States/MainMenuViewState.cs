using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;

namespace XKOMapp.ViewsFSM.States;

internal class MainMenuViewState : ViewState
{
    public MainMenuViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new Rule("Main menu").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.AddRow(new InteractableConsoleRow(new Text("Find products"), (row, own) => fsm.Checkout("productsSearch")));

        printer.AddRow(new Rule("Account").RuleStyle(new Style(StandardRenderables.AquamarineColor, null, null, "https://www.google.com/")).ToBasicConsoleRow());

        printer.StartGroup("options-account");
    }

    public override void OnEnter()
    {
        base.OnEnter();

        printer.ClearMemoryGroup("options");

        if (!SessionData.IsLoggedIn())
            printer.AddRow(new InteractableConsoleRow(new Markup($"[{StandardRenderables.GrassColorHex}]Log in[/]"), (row, own) => fsm.Checkout(new LoginViewState(fsm))), "options-account");
        else
        {
            printer.AddRow(new InteractableConsoleRow(new Text("Account view"), (row, own) => fsm.Checkout(new UserDetailsViewState(fsm))), "options-account");
            printer.AddRow(new InteractableConsoleRow(new Text("Your lists"), (row, own) => fsm.Checkout("listBrowse")), "options-account");
        }

        printer.ResetCursor();
    }
}

