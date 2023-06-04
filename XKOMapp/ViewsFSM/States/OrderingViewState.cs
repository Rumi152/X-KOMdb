using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI.ConsoleRows.User;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM.States;

internal class OrderingViewState : ViewState
{
    const int labelPad = 20;
    private readonly OrderingNameInputConsoleRow CityNameRow = new($"{"City",-labelPad} : ", 64);
    private readonly OrderingNameInputConsoleRow StreetNameRow = new($"{"Street",-labelPad} : ", 64);
    private readonly OrderingNumberInputConsoleRow BuildingNumberRow = new($"{"Building",-labelPad} : ", 4);
    private readonly OrderingNumberInputConsoleRow ApartmentNumberRow = new($"{"Apartment",-labelPad} : ", 3);
    private readonly OrderingNumberInputConsoleRow AssistanceChoiceRow = new($"{"Need assistance? 0/1",-labelPad} : ", 2);
    private readonly DiscountInputConsoleRow DiscountRow = new($"{"Insert coupon code",-labelPad} : ", 16);
    private Cart cart;
    private PaymentMethod chosenMethod = null!;
    private PromoCode discount = null!;

    public OrderingViewState(ViewStateMachine stateMachine, Cart cart) : base(stateMachine)
    {
        this.cart = cart;
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to cart"), (row, owner) =>
        {
            fsm.Checkout(new CartViewState(fsm));
        }));
        printer.AddRow(new Rule("Delivery data").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.AddRow(CityNameRow);
        printer.AddRow(StreetNameRow); 
        printer.AddRow(BuildingNumberRow);
        printer.AddRow(ApartmentNumberRow);

        printer.AddRow(new Rule("Additional").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());

        printer.AddRow(AssistanceChoiceRow);
        printer.AddRow(DiscountRow);

        printer.AddRow(new Rule("Payment methods").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());

        printer.StartGroup("method");

        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

        printer.AddRow(new InteractableConsoleRow(new Text("Save changes"), (row, own) =>
        {
            if (SessionData.HasSessionExpired(out User loggedUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            if (!ValidateInput())
                return;

            using var context = new XkomContext();
            var shipmentInfo = new ShipmentInfo()
            {
                CityName = CityNameRow.CurrentInput,
                StreetName = StreetNameRow.CurrentInput,
                BuildingNumber = int.Parse(BuildingNumberRow.CurrentInput),
                ApartmentNumber = int.Parse(ApartmentNumberRow.CurrentInput)
            };
            context.Add(shipmentInfo);

            var order = new Order()
            {
                Cart = cart,
                Status = RandomStatus(),
                OrderDate = DateTime.Now,
                PaymentMethod = chosenMethod,
                Price = 10,
                ShipmentInfo = shipmentInfo,
                NeedInstallationAssistance = true, //bool.Parse(AssistanceChoiceRow.CurrentInput),
                Discount = 12//(discount.Percentage) * Price
            };
            context.Add(order);
            context.SaveChanges();


        }));
        printer.StartGroup("errors");
    }
    public override void OnEnter()
    {
        base.OnEnter();
        RefreshMethods();
    }
    private static OrderStatus RandomStatus()
    {
        using var context = new XkomContext();
        var rand = new Random();
        int number = rand.Next(1, 4);
        var orderStatus = context.OrderStatuses.Single(x => x.Id == number);

        return orderStatus;
    }

    private void RefreshMethods()
    {
        if (SessionData.HasSessionExpired(out User loggedUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: "[red]Session expired[/]",
                loginRollbackTarget: this,
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        printer.ClearMemoryGroup("method");

        using var context = new XkomContext();
        var methods = context.PaymentMethods;
        if (!methods.Any())
            printer.AddRow(new Text("No methods were found").ToBasicConsoleRow(), "method");
        methods.ToList().ForEach(x =>
        {
            printer.AddRow(new InteractableConsoleRow(new Markup(x.Name), (row, printer) =>
            {
                if (SessionData.HasSessionExpired(out User dbUser))
                {
                    fsm.Checkout(new FastLoginViewState(fsm,
                        markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to select method[/]",
                        loginRollbackTarget: this,
                        abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                        abortMarkupMessage: "Back to main menu"
                    ));
                    return;
                }
                chosenMethod = x;

            }), "method");
        });
    }

    private bool ValidateInput()
    {
        printer.ClearMemoryGroup("errors");

        bool isValid = true;

        string City = CityNameRow.CurrentInput;
        string Street = StreetNameRow.CurrentInput;
        int Building = int.Parse(BuildingNumberRow.CurrentInput);
        int Apartment = int.Parse(ApartmentNumberRow.CurrentInput);
        int Assistance = int.Parse(AssistanceChoiceRow.CurrentInput);
        string Discount = DiscountRow.CurrentInput;

        if (City.Length < 1)
        {
            printer.AddRow(new Markup("[red]City name cannot be empty[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        if (Street.Length < 1)
        {
            printer.AddRow(new Markup("[red]Street name cannot be empty[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        if (BuildingNumberRow.CurrentInput.Length < 1)
        {
            printer.AddRow(new Markup("[red]Building number cannot be empty[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }
        else if (Building < 1)
        {
            printer.AddRow(new Markup("[red]There are no buildings numbered 0[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        if (ApartmentNumberRow.CurrentInput.Length < 1)
        {
            printer.AddRow(new Markup("[red]Apartment number cannot be empty[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }
        else if (Apartment < 1)
        {
            printer.AddRow(new Markup("[red]There are no apartments numbered 0[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        if (Assistance != 0 && Assistance != 1)
        {
            printer.AddRow(new Markup("[red]You need to choose between 0 and 1[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        if (Assistance != 0 && Assistance != 1)
        {
            printer.AddRow(new Markup("[red]You need to choose between 0 and 1[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        if (Discount.Length < 1)
            isValid = false;

        return isValid;
    }
}
