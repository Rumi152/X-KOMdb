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
        private readonly string markupMessage;
        private readonly ViewState loginRollbackTarget;
        private readonly ViewState abortRollbackTarget;
        private readonly string loginMarkupMessage;
        private readonly string abortMarkupMessage;

        const int labelPad = 8;
        private readonly EmailInputConsoleRow emailRow = new($"{"Email",-labelPad} : ", 256);
        private readonly PasswordInputConsoleRow passwordRow = new($"{"Password",-labelPad} : ", 32);

        public FastLoginViewState(ViewStateMachine stateMachine, string markupMessage, ViewState loginRollbackTarget,  ViewState abortRollbackTarget, string loginMarkupMessage = "Log in", string abortMarkupMessage = "Click to abort") : base(stateMachine)
        {
            this.markupMessage = markupMessage;
            this.loginRollbackTarget = loginRollbackTarget;
            this.abortRollbackTarget = abortRollbackTarget;
            this.loginMarkupMessage = loginMarkupMessage;
            this.abortMarkupMessage = abortMarkupMessage;
        }

        protected override void InitialPrinterBuild(ConsolePrinter printer)
        {
            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
            printer.StartContent();

            printer.AddRow(new Markup(markupMessage).ToBasicConsoleRow());
            printer.AddRow(new InteractableConsoleRow(new Markup(abortMarkupMessage), (_, _) => fsm.Checkout(abortRollbackTarget)));

            printer.AddRow(new Rule("Logging in").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
            printer.EnableScrolling();

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

        public override void OnEnter()
        {
            base.OnEnter();

            emailRow.ResetInput();
            passwordRow.ResetInput();
            printer.ResetCursor();
            printer.ClearMemoryGroup("errors");
        }

        private bool TryLogIn()
        {
            printer.ClearMemoryGroup("errors");

            string email = emailRow.CurrentInput;
            string password = passwordRow.CurrentInput;

            if (SessionData.TryLogIn(email, password, out _))
                return true;

            printer.AddRow(new Markup("[red]Wrong password and/or email[/]").ToBasicConsoleRow(), "errors");
            return false;
        }
    }
}
