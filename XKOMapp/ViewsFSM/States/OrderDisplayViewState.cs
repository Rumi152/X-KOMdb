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
internal class OrderDisplayViewState : ViewState
{
    private readonly ViewStateMachine stateMachine;
    private readonly Order order;
    private readonly ViewState rollbackTarget;
    private readonly string rollbackButtonText;

    public OrderDisplayViewState(ViewStateMachine stateMachine, Order order, ViewState rollbackTarget, string rollbackButtonText) : base(stateMachine)
    {
        this.stateMachine = stateMachine;
        this.order = order;
        this.rollbackTarget = rollbackTarget;
        this.rollbackButtonText = rollbackButtonText;
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Markup(rollbackButtonText), (row, owner) => fsm.Checkout(rollbackTarget)));

        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.StartGroup("data");

        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

        printer.StartGroup("products");
    }

    public override void OnEnter()
    {
        base.OnEnter();
        RefreshData();
        RefreshProducts();
    }


    private void RefreshData()
    {
        if (SessionData.HasSessionExpired(out User dbUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: $"[red]Session expired[/]",
                loginRollbackTarget: this,
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        printer.ClearMemoryGroup("data");

        const int padding = 23;
        printer.AddRow(new Text($"{"ID",-padding}: {order.Id}").ToBasicConsoleRow(), "data");
        printer.AddRow(new Text($"{"Status",-padding}: {order.Status.Name}").ToBasicConsoleRow(), "data");
        printer.AddRow(new Text($"{"Ordered",-padding}: {order.OrderDate:dd.MMM.yyyy mm:HH}").ToBasicConsoleRow(), "data");
        printer.AddRow(new Text($"{"Payment method",-padding}: {order.PaymentMethod?.Name ?? "Other"}").ToBasicConsoleRow(), "data");
        printer.AddRow(new Text($"{"Installation assistance",-padding}: {(order.NeedInstallationAssistance ? "Yes" : "No")}").ToBasicConsoleRow(), "data");

        printer.AddRow(new Text($"{"Products price",-padding}: [lime]{order.Price + order.Cart.Discount,-9:F2}[/] PLN").ToBasicConsoleRow(), "data");
        printer.AddRow(new Text($"{"Discount",-padding}: -[red]{order.Cart.Discount,-9:F2}[/] PLN").ToBasicConsoleRow(), "data");
        printer.AddRow(new Text($"{"Final price",-padding}: [lime]{order.Price,-9:F2}[/] PLN").ToBasicConsoleRow(), "data");

        printer.AddRow(new Text($"{"City",-padding}: {order.ShipmentInfo.City.Name}").ToBasicConsoleRow(), "data");
        printer.AddRow(new Text($"{"Street",-padding}: {order.ShipmentInfo.StreetName}").ToBasicConsoleRow(), "data");
        printer.AddRow(new Text($"{"Building number",-padding}: {order.ShipmentInfo.BuildingNumber}").ToBasicConsoleRow(), "data");
        printer.AddRow(new Text($"{"Apartment number",-padding}: {order.ShipmentInfo.ApartmentNumber}").ToBasicConsoleRow(), "data");
    }

    private void RefreshProducts()
    {
        if (SessionData.HasSessionExpired(out User dbUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: $"[red]Session expired[/]",
                loginRollbackTarget: this,
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        printer.ClearMemoryGroup("products");

        order.Cart.CartProducts.ToList().ForEach(x => DisplayCartProduct(x));
    }


    private void DisplayCartProduct(CartProduct cp)
    {
        Product product = cp.Product;

        var priceString = $"[lime]{product.Price,-9:F2}[/] PLN";
        var companyString = product.Company is null ? new string(' ', 32) : (product.Company.Name.Length <= 29 ? $"{product.Company.Name,-29}" : $"{product.Company.Name[..30]}...");

        var displayString = $"{cp.Amount,-5}x {product.Name.EscapeMarkup(),-32} | {priceString + new string(' ', 13 - priceString.RemoveMarkup().Length)} | {companyString}";
        printer.AddRow(new InteractableConsoleRow(new Markup(displayString), (row, printer) => fsm.Checkout(new ProductViewState(fsm, product, this, "Back to user"))), "products");
    }
}
