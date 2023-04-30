using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows.User;

namespace XKOMapp.ViewsFSM.States
{
    internal class RegisteringViewState : ViewState
    {
        public RegisteringViewState(ViewStateMachine stateMachine) : base(stateMachine)
        {
            printer = new ConsolePrinter();

            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
            printer.StartContent();
            printer.EnableScrolling();

            const int labelPad = 16;
            NameInputConsoleRow nameRow = new NameInputConsoleRow($"{"Name", -labelPad} : ", 32, (key) =>
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

            NameInputConsoleRow surnameRow = new NameInputConsoleRow($"{"Last name", -labelPad} : ", 32, (key) =>
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

            EmailInputConsoleRow emailRow = new EmailInputConsoleRow($"{"Email: ",-labelPad} : ", 256); //TODO
            printer.AddRow(emailRow);

            PasswordInputConsoleRow passwordRow = new PasswordInputConsoleRow($"{"Password",-labelPad} : ", 32);
            printer.AddRow(passwordRow);

            PasswordInputConsoleRow passwordConfirmRow = new PasswordInputConsoleRow($"{"Confirm password",-labelPad} : ", 32);
            printer.AddRow(passwordConfirmRow);
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
    }
}
