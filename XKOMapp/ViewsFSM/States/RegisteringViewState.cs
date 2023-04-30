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
            printer.AddRow(new Rule("Registering").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
            printer.StartContent();
            printer.EnableScrolling();

            const int labelPad = 16;
            nameRow = new NameInputConsoleRow($"{"Name",-labelPad} : ", 32);
            printer.AddRow(nameRow);

            surnameRow = new NameInputConsoleRow($"{"Last name",-labelPad} : ", 32);
            printer.AddRow(surnameRow);

            emailRow = new EmailInputConsoleRow($"{"Email",-labelPad} : ", 256);
            printer.AddRow(emailRow);

            passwordRow = new PasswordInputConsoleRow($"{"Password",-labelPad} : ", 32);
            printer.AddRow(passwordRow);

            passwordConfirmRow = new PasswordInputConsoleRow($"{"Confirm password",-labelPad} : ", 32);
            printer.AddRow(passwordConfirmRow);

            printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

            printer.AddRow(new InteractableConsoleRow(new Text("Log in"), (row, owner) => fsm.Checkout(new LoginViewState(fsm))));
            printer.AddRow(new InteractableConsoleRow(new Text("Create account"), (row, owner) =>
            {
                if (!ValidateInput())
                    return;

                using var context = new XkomContext();
                var newUser = new User()
                {
                    Name = nameRow.CurrentInput,
                    LastName = surnameRow.CurrentInput,
                    Email = emailRow.CurrentInput,
                    Password = passwordRow.CurrentInput
                };
                context.Add(newUser);
                context.SaveChanges();

                //TODO checkout user view state
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
            else
            {
                using (var context = new XkomContext())
                {
                    if (context.Users.Any(x => x.Email == email))
                    {
                        printer.AddRow(new Markup("[red]Email is already used[/]").ToBasicConsoleRow(), "errors");
                        isValid = false;
                    }
                }
            }

            if(password.Length < 6)
            {
                printer.AddRow(new Markup("[red]Password must be at least 6 characters[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
            else if(password.Length > 64)
            {
                printer.AddRow(new Markup("[red]Password cannot be longer than 64 characters[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
            if (!password.Any(x => char.IsLower(x)))
            {
                printer.AddRow(new Markup("[red]Password must have lowercase character[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
            if (!password.Any(x => char.IsUpper(x)))
            {
                printer.AddRow(new Markup("[red]Password must have uppercase character[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
            if (!password.Any(x => char.IsDigit(x)))
            {
                printer.AddRow(new Markup("[red]Password must have digit[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }

            if(password != passwordConfirm)
            {
                printer.AddRow(new Markup("[red]Password must match[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }

            if (name.Length < 1)
            {
                printer.AddRow(new Markup("[red]Please input your name[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
            else if (name.Length > 32)
            {
                printer.AddRow(new Markup("[red]Name is too long[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }

            if (lastName.Length < 1)
            {
                printer.AddRow(new Markup("[red]Please input your last name[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
            else if (lastName.Length > 32)
            {
                printer.AddRow(new Markup("[red]Last name is too long[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }

            return isValid;
        }
    }
}
