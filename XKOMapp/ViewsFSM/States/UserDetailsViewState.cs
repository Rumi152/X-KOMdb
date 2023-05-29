using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System.Text.RegularExpressions;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI.ConsoleRows.User;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States;

internal class UserDetailsViewState : ViewState
{
    public UserDetailsViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();
        printer.AddRow(new InteractableConsoleRow(new Text("Back to main menu"), (row, owner) => fsm.Checkout("mainMenu")));
        printer.AddRow(new InteractableConsoleRow(new Markup("[red]Log out[/]"), (row, onwer) =>
        {
            SessionData.LogOut();

            fsm.Checkout(new LoginViewState(fsm));
        }));
        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

        printer.EnableScrolling();

        printer.StartGroup("credentials");

        printer.StartGroup("editing-button");
        printer.StartGroup("editing-unfold");
        printer.StartGroup("editing-unfold-errors");

        printer.StartGroup("deleting");

        Rule rule = new Rule("Click to refresh orders").RuleStyle(new Style().Foreground(StandardRenderables.AquamarineColor)).HeavyBorder();
        printer.AddRow(new InteractableConsoleRow(rule, (row, onwer) => RefreshOrders()));

        printer.StartGroup("orders");
    }

    public override void OnEnter()
    {
        if (SessionData.HasSessionExpired(out User? loggedUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: $"[red]Session expired[/]",
                loginRollbackTarget: new UserDetailsViewState(fsm),
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        base.OnEnter();

        DisplayDeleteBase();
        RefreshCredentials(loggedUser);
        HideEditInput();
        RefreshOrders();

        printer.ResetCursor();
    }

    private void RefreshOrders()
    {
        if (SessionData.HasSessionExpired(out User? loggedUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: $"[red]Session expired[/]",
                loginRollbackTarget: new UserDetailsViewState(fsm),
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        printer.ClearMemoryGroup("orders");

        using var context = new XkomContext();
        context.Attach(loggedUser);
        var orders = context
            .Orders
            .Include(x => x.Status)
            .Include(x => x.Cart)
            .ThenInclude(x => x.User)
            .Where(x => x.Cart.User == loggedUser)
            .ToList();

        var statusGrouping = orders
            .GroupBy(x => x.Status, x => x, (status, groupedOrders) => new
            {
                Orders = groupedOrders.ToList(),
                Status = status
            })
            .ToList();

        statusGrouping.ForEach(g =>
        {
            printer.AddRow(new Text($"{g.Status.Name}:").ToBasicConsoleRow(), "orders");
            g.Orders.ForEach(order =>
            {
                printer.AddRow(new Text($"  {order.Id}").ToBasicConsoleRow(), "orders");
            });
        });
    }

    private void RefreshCredentials(User user)
    {
        printer.ClearMemoryGroup("credentials");

        const int pad = 9;
        printer.AddRow(new Text($"{"Name",-pad} : {user.Name}").ToBasicConsoleRow(), "credentials");
        printer.AddRow(new Text($"{"Last name",-pad} : {user.LastName}").ToBasicConsoleRow(), "credentials");
        printer.AddRow(new Text($"{"Email",-pad} : {new string(user.Email.Take(Console.WindowWidth - 8 - pad).ToArray())}").ToBasicConsoleRow(), "credentials");//REFACTOR add better support for long emails
        printer.AddRow(new Text($"{"Password",-pad} : {user.Password}").ToBasicConsoleRow(), "credentials");//REFACTOR add hiding password
    }


    private void DisplayDeleteBase()
    {
        printer.ClearMemoryGroup("deleting");

        printer.AddRow(new InteractableConsoleRow(new Markup("[red]Delete account[/]"), (row, onwer) => DisplayDeleteSure()), "deleting");
    }

    private void DisplayDeleteSure()
    {
        printer.ClearMemoryGroup("deleting");

        printer.AddRow(new InteractableConsoleRow(new Markup("[red]You sure dawg?[/]"), (row, onwer) =>
        {
            if (SessionData.HasSessionExpired(out User? loggedUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            using XkomContext context = new();
            context.Remove(loggedUser);
            context.SaveChanges();

            SessionData.LogOut();
            fsm.Checkout("mainMenu");

        }), "deleting");
    }



    private void HideEditInput()
    {
        printer.ClearMemoryGroup("editing");

        printer.AddRow(new InteractableConsoleRow(new Markup($"[{StandardRenderables.GrassColorHex}]Edit data[/]"), (row, owner) =>
        {
            if (SessionData.HasSessionExpired(out User? loggedUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            ShowEditInput();
            printer.CursorToGroup("editing-button");

        }), "editing-button");
    }

    private void ShowEditInput()
    {
        printer.ClearMemoryGroup("editing");

        //TODO make them all focused and with prettier cursor

        printer.AddRow(new InteractableConsoleRow(new Markup($"[red]Cancel[/]"), (row, owner) => HideEditInput()), "editing-unfold");

        const int labelPad = 16;
        const string indent = "  ";
        NameInputConsoleRow nameRow = new($"{indent}{"Name",-labelPad} : ", 32);
        NameInputConsoleRow surnameRow = new($"{indent}{"Last name",-labelPad} : ", 32);
        EmailInputConsoleRow emailRow = new($"{indent}{"Email",-labelPad} : ", 256);
        PasswordInputConsoleRow passwordRow = new($"{indent}{"Password",-labelPad} : ", 32);
        PasswordInputConsoleRow passwordConfirmRow = new($"{indent}{"Confirm password",-labelPad} : ", 32);

        printer.AddRow(nameRow, "editing-unfold");
        printer.AddRow(surnameRow, "editing-unfold");
        printer.AddRow(emailRow, "editing-unfold");
        printer.AddRow(passwordRow, "editing-unfold");
        printer.AddRow(passwordConfirmRow, "editing-unfold");

        printer.AddRow(new InteractableConsoleRow(new Markup($"[{StandardRenderables.GrassColorHex}]Accept[/]"), (row, owner) =>
            {
                if (SessionData.HasSessionExpired(out User? loggedUser))
                {
                    fsm.Checkout(new FastLoginViewState(fsm,
                        markupMessage: $"[red]Session expired[/]",
                        loginRollbackTarget: this,
                        abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                        abortMarkupMessage: "Back to main menu"
                    ));
                    return;
                }

                string name = nameRow.CurrentInput;
                string lastName = surnameRow.CurrentInput;
                string email = emailRow.CurrentInput;
                string password = passwordRow.CurrentInput;
                string passwordConfirm = passwordConfirmRow.CurrentInput;

                if (!ValidateInput(name, lastName, email, password, passwordConfirm))
                    return;

                using XkomContext context = new();
                context.Attach(loggedUser);

                if (name.Length > 0)
                    loggedUser.Name = name;
                if (lastName.Length > 0)
                    loggedUser.LastName = lastName;
                if (email.Length > 0)
                    loggedUser.Email = email;
                if (password.Length > 0)
                    loggedUser.Password = password;

                context.SaveChanges();
                SessionData.TryLogIn(email, password, out _);

                HideEditInput();

            }), "editing-unfold");

        printer.CursorToGroup("editing-unfold");
    }

    private bool ValidateInput(string name, string lastName, string email, string password, string passwordConfirm)
    {
        printer.ClearMemoryGroup("editing-unfold-errors");

        bool isValid = true;

        if (email.Length > 0)
        {
            if (email.Length < 3)
            {
                printer.AddRow(new Markup("[red]Email is too short[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
            else if (email.Length > 256)
            {
                printer.AddRow(new Markup("[red]Email is too long[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
            else if (!Regex.IsMatch(email, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                printer.AddRow(new Markup("[red]Email has wrong characters or is formatted wrong[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
            else
            {
                using (var context = new XkomContext())
                {
                    if (context.Users.Any(x => x.Email == email))
                    {
                        printer.AddRow(new Markup("[red]Email is already used[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                        isValid = false;
                    }
                }
            }
        }


        if(password.Length > 0)
        {
            if (password.Length < 6)
            {
                printer.AddRow(new Markup("[red]Password must be at least 6 characters[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
            else if (password.Length > 64)
            {
                printer.AddRow(new Markup("[red]Password cannot be longer than 64 characters[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
            if (!password.Any(x => char.IsLower(x)))
            {
                printer.AddRow(new Markup("[red]Password must have lowercase character[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
            if (!password.Any(x => char.IsUpper(x)))
            {
                printer.AddRow(new Markup("[red]Password must have uppercase character[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
            if (!password.Any(x => char.IsDigit(x)))
            {
                printer.AddRow(new Markup("[red]Password must have digit[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }

            if (password != passwordConfirm)
            {
                printer.AddRow(new Markup("[red]Password must match[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
        }


        if(name.Length > 0)
        {
            if (name.Length < 1)
            {
                printer.AddRow(new Markup("[red]Please input your name[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
            else if (name.Length > 32)
            {
                printer.AddRow(new Markup("[red]Name is too long[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
        }


        if(lastName.Length > 0)
        {
            if (lastName.Length < 1)
            {
                printer.AddRow(new Markup("[red]Please input your last name[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
            else if (lastName.Length > 32)
            {
                printer.AddRow(new Markup("[red]Last name is too long[/]").ToBasicConsoleRow(), "editing-unfold-errors");
                isValid = false;
            }
        }


        return isValid;
    }
}
