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
        private User loggedUser = null!;

        public UserDetailsViewState(ViewStateMachine stateMachine) : base(stateMachine)
        {
            if (!SessionData.IsLoggedIn())
            {
                fsm.Checkout(new FastLoginViewState(fsm, this));
                return;
            }
            if (SessionData.HasSessionExpired(out loggedUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm, this, new Markup("[red]Session expired[/]\n").ToBasicConsoleRow()));
                return;
            }

            printer = new ConsolePrinter();

            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
            printer.StartContent();
            printer.AddRow(new InteractableConsoleRow(new Text("Back to main menu"), (row, owner) => fsm.Checkout("mainMenu")));
            printer.AddRow(new InteractableConsoleRow(new Markup("[red]Log out[/]"), (row, onwer) =>
            {
                SessionData.LogOut();

                fsm.Checkout("mainMenu");
            }));
            printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

            printer.EnableScrolling();

            printer.AddRow(new Text(loggedUser.Name).ToBasicConsoleRow());
            printer.AddRow(new Text(loggedUser.LastName).ToBasicConsoleRow());
            printer.AddRow(new Text(loggedUser.Email).ToBasicConsoleRow());//TODO add support for long emails
            printer.AddRow(new Text(loggedUser.Password).ToBasicConsoleRow());//TODO add hiding password

            var rule = new Rule("Click to refresh orders").RuleStyle(new Style().Foreground(StandardRenderables.AquamarineColor)).HeavyBorder();
            printer.AddRow(new InteractableConsoleRow(rule, (row, onwer) => RefreshOrders()));

            printer.StartGroup("orders");
            RefreshOrders();
        }

        private void RefreshOrders()
        {
            if (SessionData.HasSessionExpired(out loggedUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm, this, new Markup("[red]Session expired[/]").ToBasicConsoleRow()));
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
