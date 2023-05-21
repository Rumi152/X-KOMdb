using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI.ConsoleRows.User;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States
{
    internal class LoginViewState : ViewState
    {
        private readonly EmailInputConsoleRow emailRow;
        private readonly PasswordInputConsoleRow passwordRow;

        public LoginViewState(ViewStateMachine stateMachine) : base(stateMachine)
        {
            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
            printer.StartContent();

            printer.AddRow(new InteractableConsoleRow(new Text("Click to abort"), (row, owner) =>
            {
                fsm.Checkout("mainMenu");
            }));

            printer.AddRow(new Rule("Logging in").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
            printer.EnableScrolling();

            const int labelPad = 8;
            emailRow = new EmailInputConsoleRow($"{"Email",-labelPad} : ", 256);
            printer.AddRow(emailRow);

            passwordRow = new PasswordInputConsoleRow($"{"Password",-labelPad} : ", 32);
            printer.AddRow(passwordRow);

            printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());
            printer.AddRow(new InteractableConsoleRow(new Markup($"Don't have an account? [{StandardRenderables.GrassColorHex}] Create one[/]"), (row, owner) => fsm.Checkout(new RegisteringViewState(fsm))));
            printer.AddRow(new InteractableConsoleRow(new Text("Log In"), (row, owner) =>
            {
                if (!TryLogIn())
                    return;
                

                fsm.Checkout(new UserDetailsViewState(fsm));
            }));
            printer.StartGroup("errors");
        }

        public override void OnEnter()
        {
            base.OnEnter();

            printer?.ResetCursor();
        }


        private bool TryLogIn()
        {
            printer?.ClearMemoryGroup("errors");

            string email = emailRow.CurrentInput;
            string password = passwordRow.CurrentInput;

            if (SessionData.TryLogIn(email, password, out _))
                return true;

            printer?.AddRow(new Markup("[red]Wrong password and/or email[/]").ToBasicConsoleRow(), "errors");
            return false;
        }
    }
}
