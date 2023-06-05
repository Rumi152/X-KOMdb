using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
    private readonly BuldingNumberInputConsoleRow BuildingNumberRow = new($"{"Building",-labelPad} : ", 8);
    private readonly BuldingNumberInputConsoleRow ApartmentNumberRow = new($"{"Apartment",-labelPad} : ", 8);
    private readonly BuldingNumberInputConsoleRow AssistanceChoiceRow = new($"{"Need assistance? 0/1",-labelPad} : ", 2);//TEMP
    private readonly DiscountInputConsoleRow DiscountRow = new($"{"Insert coupon code",-labelPad} : ");

    private PaymentMethod? chosenMethod;

    public OrderingViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
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

        printer.AddRow(new InteractableConsoleRow(new Text("Order"), (row, own) =>
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
            //REFACTOR forgive me
            try { context.Attach(loggedUser); }
            catch(InvalidOperationException) { }

            var shipmentInfo = new ShipmentInfo()
            {
                CityName = CityNameRow.CurrentInput,
                StreetName = StreetNameRow.CurrentInput,
                BuildingNumber = int.Parse(BuildingNumberRow.CurrentInput),//TEMP
                ApartmentNumber = int.Parse(ApartmentNumberRow.CurrentInput)//TEMP
            };
            context.Add(shipmentInfo);

            var activeCart = context.Carts
                .Include(x => x.CartProducts)
                    .ThenInclude(x => x.Product)
                .SingleOrDefault(x => x.Id == loggedUser.ActiveCartId);

            if(activeCart is null || activeCart.CartProducts.IsNullOrEmpty())
            {
                fsm.Checkout(new MessageViewState(fsm,
                    message: $"[red]Your cart is empty[/]",
                    buttonTarget: this,
                    buttonMessage: "Back to cart"
                ));
                return;
            }

            var order = new Order()
            {
                Cart = activeCart,
                Status = RandomStatus(),
                OrderDate = DateTime.Now,
                PaymentMethod = chosenMethod,
                Price = activeCart.CartProducts.Aggregate(0m, (acc, current) => acc + current.Product.Price * current.Amount),
                ShipmentInfo = shipmentInfo,
                NeedInstallationAssistance = true, //TEMP
                Discount = 12 //TEMP
            };

            loggedUser.ActiveCart = null;
            context.Add(order);
            context.SaveChanges();

            fsm.Checkout(new UserDetailsViewState(fsm));
        }));
        printer.StartGroup("errors");
    }
    public override void OnEnter()
    {
        base.OnEnter();

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

        using var context = new XkomContext();
        var activeCart = context.Carts
            .Find(loggedUser.ActiveCartId);

        if (activeCart is null || !context.CartProducts.Any(x => x.CartId == activeCart.Id))
        {
            fsm.Checkout(new MessageViewState(fsm,
                message: $"[red]Your cart is empty[/]",
                buttonTarget: this,
                buttonMessage: "Back to cart"
            ));
            return;
        }

        ResetInputs();
        RefreshMethods();
    }


    private static OrderStatus RandomStatus()
    {
        using var context = new XkomContext();

        var statuses = context.OrderStatuses.AsNoTracking().ToList();

        var rand = new Random();
        int number = rand.Next(0, statuses.Count);

        return statuses[number];
    }

    private void RefreshMethods()
    {
        printer.ClearMemoryGroup("method");

        using var context = new XkomContext();
        var methods = context.PaymentMethods.ToList();

        if (!methods.Any())
            printer.AddRow(new Text("No payment methods are available").ToBasicConsoleRow(), "method");

        methods.ForEach(x =>
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

        string city = CityNameRow.CurrentInput;
        string street = StreetNameRow.CurrentInput;
        string building = BuildingNumberRow.CurrentInput;
        string apartment = ApartmentNumberRow.CurrentInput;
        string assistance = AssistanceChoiceRow.CurrentInput;
        string promoCode = DiscountRow.CurrentInput;

        if (city.Length < 1)
        {
            printer.AddRow(new Markup("[red]Please provide city name[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }
        
        if (chosenMethod is null)
        {
            printer.AddRow(new Markup("[red]Please choose payment method[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        if (street.Length < 1)
        {
            printer.AddRow(new Markup("[red]Please provide street name[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        if (building.Length < 1)
        {
            printer.AddRow(new Markup("[red]Please provide building number[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        if (apartment.Length < 1)
        {
            printer.AddRow(new Markup("[red]Please provide apartment number[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        return isValid;
    }


    private void ResetInputs()
    {
        chosenMethod = null;
        CityNameRow.ResetInput();
        StreetNameRow.ResetInput();
        BuildingNumberRow.ResetInput();
        ApartmentNumberRow.ResetInput();
        AssistanceChoiceRow.ResetInput();
        DiscountRow.ResetInput();
    }
}
