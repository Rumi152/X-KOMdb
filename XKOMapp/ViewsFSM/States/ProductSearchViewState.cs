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
    public class ProductSearchViewState : ViewState
    {
        public ProductSearchViewState(ViewStateMachine stateMachine) : base(stateMachine)
        {
            //using var context = new XkomContext();
            //var newProduct = new Product();
            //newProduct.Name = "Logitech G402 Mouse";
            //newProduct.Description = "";
            //newProduct.Price = 250.5m;
            //newProduct.IsAvailable = true;
            //context.Add(newProduct);
            //context.SaveChanges();

            printer = new ConsolePrinter();
            printer.EnableScrolling();

            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
            printer.StartContent();

            //TODO
            //Company search
            //Price range
            //Name search
            //Category search
            //reset filters unavailable
            //orderby: newest, highest ratings, cheapest, most expensive

            printer.AddRow(new InteractableConsoleRow(new Text("Placeholder"), (row, printer) => RefreshProducts()));
            printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());
            printer.StartGroup("products");
        }

        public override void OnEnter()
        {
            base.OnEnter();

            RefreshProducts();
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

        private void RefreshProducts()
        {
            printer?.ClearMemoryGroup("products");

            using var context = new XkomContext();

            //TODO constraints and ordering
            var products = context.Products.ToList();

            products.ForEach(x =>
            {
                var priceString = x.IsAvailable ? $"[lime]{x.Price.ToString("0.00"),-9}[/] PLN" : "[red]Unavailable[/]";
                printer?.AddRow(new Markup($"{x.Name.EscapeMarkup(),-32} | {priceString}").ToBasicConsoleRow(), "products");
            });
        }
    }
}
