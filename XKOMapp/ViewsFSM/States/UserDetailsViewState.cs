using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States
{
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

            //TODO edit button unfolding 5 inputs and accept button
            //TODO implement deleting account

            var rule = new Rule("Click to refresh orders").RuleStyle(new Style().Foreground(StandardRenderables.AquamarineColor)).HeavyBorder();
            printer.AddRow(new InteractableConsoleRow(rule, (row, onwer) => RefreshOrders()));

            printer.StartGroup("orders");
        }

        public override void OnEnter()
        {
            if (SessionData.HasSessionExpired(out var loggedUser))
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

            printer.ClearMemoryGroup("credentials");

            RefreshCredentials(loggedUser);

            printer.ResetCursor();
            RefreshOrders();
        }

        private void RefreshOrders()
        {
            if (SessionData.HasSessionExpired(out var loggedUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/]",
                    loginRollbackTarget: new UserDetailsViewState(fsm),
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            printer?.ClearMemoryGroup("orders");

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
                printer?.AddRow(new Text($"{g.Status.Name}:").ToBasicConsoleRow(), "orders");
                g.Orders.ForEach(order =>
                {
                    printer?.AddRow(new Text($"  {order.Id}").ToBasicConsoleRow(), "orders");
                });
            });
        }

        private void RefreshCredentials(User user)
        {
            const int pad = 9;
            printer.AddRow(new Text($"{"Name",-pad} : {user.Name}").ToBasicConsoleRow(), "credentials");
            printer.AddRow(new Text($"{"Last name",-pad} : {user.LastName}").ToBasicConsoleRow(), "credentials");
            printer.AddRow(new Text($"{"Email",-pad} : {new string(user.Email.Take(Console.WindowWidth - 8 - pad).ToArray())}").ToBasicConsoleRow(), "credentials");//REFACTOR add better support for long emails
            printer.AddRow(new Text($"{"Password",-pad} : {user.Password}").ToBasicConsoleRow(), "credentials");//REFACTOR add hiding password
        }
    }
}
