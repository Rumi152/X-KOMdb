using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI.ConsoleRows.User;

namespace XKOMapp.ViewsFSM.States
{
    internal class RegisteringViewState : ViewState
    {
        private readonly NameInputConsoleRow nameRow;
        private readonly NameInputConsoleRow surnameRow;
        private readonly EmailInputConsoleRow emailRow;
        private readonly PasswordInputConsoleRow passwordRow;
        private readonly PasswordInputConsoleRow passwordConfirmRow;

        public RegisteringViewState(ViewStateMachine stateMachine) : base(stateMachine)
        {
            printer = new ConsolePrinter();

            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
            printer.StartContent();
            printer.EnableScrolling();

            const int labelPad = 16;
            nameRow = new NameInputConsoleRow($"{"Name",-labelPad} : ", 32, (key) =>
                {
                    if (char.IsPunctuation(key))
                        return false;

                    if (char.IsWhiteSpace(key))
                        return false;

                    if (char.IsSeparator(key))
                        return false;

                    if (char.IsSymbol(key))
                        return false;

                    return true;
                });
            printer.AddRow(nameRow);

            surnameRow = new NameInputConsoleRow($"{"Last name",-labelPad} : ", 32, (key) =>
            {
                if (char.IsPunctuation(key))
                    return false;

                if (char.IsWhiteSpace(key))
                    return false;

                if (char.IsSeparator(key))
                    return false;

                if (char.IsSymbol(key))
                    return false;

                return true;
            });
            printer.AddRow(surnameRow);

            emailRow = new EmailInputConsoleRow($"{"Email: ",-labelPad} : ", 256);
            printer.AddRow(emailRow);

            passwordRow = new PasswordInputConsoleRow($"{"Password",-labelPad} : ", 32);
            printer.AddRow(passwordRow);

            passwordConfirmRow = new PasswordInputConsoleRow($"{"Confirm password",-labelPad} : ", 32);
            printer.AddRow(passwordConfirmRow);

            printer.AddRow(new InteractableConsoleRow(new Text("Create account"), (row, owner) =>
            {
                ValidateInput();
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


        private bool ValidateInput()
        {
            printer.ClearMemoryGroup("errors");

            bool isValid = true;

            string name = nameRow.CurrentInput;
            string lastName = surnameRow.CurrentInput;
            string email = emailRow.CurrentInput;
            string password = passwordRow.CurrentInput;
            string passwordConfirm = passwordConfirmRow.CurrentInput;

            if(email.Length < 3)
            {
                printer.AddRow(new Markup("[red]Email is too short[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
            else if(email.Length > 256)
            {
                printer.AddRow(new Markup("[red]Email is too long[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
            else if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                printer.AddRow(new Markup("[red]Email has wrong characters or is formatted wrong[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }

            return isValid;
        }
    }
}
