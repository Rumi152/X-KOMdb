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

        public FastLoginViewState(ViewStateMachine stateMachine, params IConsoleRow[] additionalRows) : base(stateMachine)
        {
            printer = new ConsolePrinter();

            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());

            foreach(var extraRow in additionalRows)
                printer.AddRow(extraRow);

            printer.AddRow(new Rule("Logging in").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
            printer.StartContent();
            printer.EnableScrolling();

            const int labelPad = 8;
            emailRow = new EmailInputConsoleRow($"{"Email",-labelPad} : ", 256);
            printer.AddRow(emailRow);

            passwordRow = new PasswordInputConsoleRow($"{"Password",-labelPad} : ", 32);
            printer.AddRow(passwordRow);

            printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

            printer.AddRow(new InteractableConsoleRow(new Text("Log In"), (row, owner) =>
            {
                if (!TryLogIn())
                    return;

                fsm.RollbackOrDefault(this); //TODO
            }));
            printer.StartGroup("errors");
        }

        public override void OnEnter()
        {
            base.OnEnter();

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


        private bool TryLogIn()
        {
            printer.ClearMemoryGroup("errors");

            string email = emailRow.CurrentInput;
            string password = passwordRow.CurrentInput;

            if (SessionData.TryLogIn(email, password))
                return true;

            printer.AddRow(new Markup("[red]Wrong password and/or email[/]").ToBasicConsoleRow(), "errors");
            return false;
        }
    }
}
