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
    internal class FastLoginViewState : ViewState
    {
        private readonly EmailInputConsoleRow emailRow;
        private readonly PasswordInputConsoleRow passwordRow;

        public FastLoginViewState(ViewStateMachine stateMachine, string markupMessage, ViewState loginRollbackTarget,  ViewState abortRollbackTarget, string loginMarkupMessage = "Log in", string abortMarkupMessage = "Click to abort") : base(stateMachine)
        {
            printer = new ConsolePrinter();

            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
            printer.StartContent();

            printer.AddRow(new Markup(markupMessage).ToBasicConsoleRow());
            printer.AddRow(new InteractableConsoleRow(new Markup(abortMarkupMessage), (_, _) => fsm.Checkout(abortRollbackTarget)));

            printer.AddRow(new Rule("Logging in").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
            printer.EnableScrolling();

            const int labelPad = 8;
            emailRow = new EmailInputConsoleRow($"{"Email",-labelPad} : ", 256);
            passwordRow = new PasswordInputConsoleRow($"{"Password",-labelPad} : ", 32);
            printer.AddRow(emailRow);
            printer.AddRow(passwordRow);

            printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

            printer.AddRow(new InteractableConsoleRow(new Markup(loginMarkupMessage), (row, owner) =>
            {
                if (!TryLogIn())
                    return;

                fsm.Checkout(loginRollbackTarget);
            }));
            printer.StartGroup("errors");
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
