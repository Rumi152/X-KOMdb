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

namespace XKOMapp.ViewsFSM.States;

public class ProductSearchViewState : ViewState
{
    public ProductSearchViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
        //using var context = new XkomContext();

        //var newProduct1 = new Product();
        //newProduct1.Name = "Logitech G402 Mouse";
        //newProduct1.Description = "";
        //newProduct1.Price = 250.5m;
        //newProduct1.NumberAvailable = 124;
        //context.Add(newProduct1);

        //var newProduct2 = new Product();
        //newProduct2.Name = "Laptop HP 15s";
        //newProduct2.Description = "";
        //newProduct2.Price = 3199.99m;
        //newProduct2.NumberAvailable = 156;
        //context.Add(newProduct2);

        //var newProduct3 = new Product();
        //newProduct3.Name = "SSD Disk";
        //newProduct3.Description = "";
        //newProduct3.Price = 600m;
        //newProduct3.NumberAvailable = 0;
        //context.Add(newProduct3);

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
        //reset filters
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
            var priceString = x.NumberAvailable > 0 ? $"[lime]{x.Price.ToString("0.00"),-9}[/] PLN" : "[red]Unavailable[/]";
            printer?.AddRow(new Markup($"{x.Name.EscapeMarkup(),-32} | {priceString}").ToBasicConsoleRow(), "products");
        });
    }
}
