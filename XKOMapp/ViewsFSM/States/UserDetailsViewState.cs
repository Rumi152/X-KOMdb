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

            printer = new ConsolePrinter();

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

            const int pad = 9;
            printer.AddRow(new Text($"{"Name",-pad} : {loggedUser.Name}").ToBasicConsoleRow());
            printer.AddRow(new Text($"{"Last name",-pad} : {loggedUser.LastName}").ToBasicConsoleRow());
            printer.AddRow(new Text($"{"Email",-pad} : {new string(loggedUser.Email.Take(Console.WindowWidth - 8 - pad).ToArray())}").ToBasicConsoleRow());//REFACTOR add better support for long emails
            printer.AddRow(new Text($"{"Password",-pad} : {loggedUser.Password}").ToBasicConsoleRow());//REFACTOR add hiding password

            //TODO edit button unfolding 5 inputs and accept button

            var rule = new Rule("Click to refresh orders").RuleStyle(new Style().Foreground(StandardRenderables.AquamarineColor)).HeavyBorder();
            printer.AddRow(new InteractableConsoleRow(rule, (row, onwer) => RefreshOrders()));

            printer.StartGroup("orders");
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
    }
}
