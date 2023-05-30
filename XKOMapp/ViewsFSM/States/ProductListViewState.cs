using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI;
using XKOMapp.Models;
using XKOMapp.GUI.ConsoleRows.ProductList;

namespace XKOMapp.ViewsFSM.States;
internal class ProductListViewState : ViewState
{
    private readonly Product product;
    private ProductNumberInputConsoleRow numberInput;
    public ProductListViewState(ViewStateMachine stateMachine, Product product) : base(stateMachine)
    {
        this.product = product;
        numberInput = new ProductNumberInputConsoleRow($"Number of products: ", 5);
    }
    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Click to abort"), (row, owner) =>
        {
            fsm.Checkout("productsSearch");
        }));
        printer.AddRow(new Rule("Lists").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.AddRow(numberInput);
        printer.StartGroup("errors");

        printer.AddRow(new InteractableConsoleRow(new Text("Add new list"), (row, owner) =>
        {
            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to add list[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }
            //todo fastcreatelist
            fsm.Checkout(new ListCreateViewState(fsm));
        }));

        printer.AddRow(new Rule("Your lists").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());

        printer.StartGroup("lists");
    }

    public override void OnEnter()
    {
        if (SessionData.HasSessionExpired(out User dbUser))
        {
            fsm.Checkout(new FastLoginViewState(fsm,
                markupMessage: "[red]Session expired[/]",
                loginRollbackTarget: this,
                abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                abortMarkupMessage: "Back to main menu"
            ));
            return;
        }

        base.OnEnter();

        printer.ClearMemoryGroup("errors");
        printer.ClearMemoryGroup("lists");

        using var context = new XkomContext();
        context.Attach(dbUser);
        var lists = context.Lists.Where(x => x.User == dbUser);
        if (!lists.Any())
            printer.AddRow(new Text("No lists were found").ToBasicConsoleRow(), "lists");

        lists.ToList().ForEach(x =>
        {
            printer.AddRow(new InteractableConsoleRow(new Markup(x.Name), (row, printer) =>
            {
                if (SessionData.HasSessionExpired(out User dbUser))
                {
                    fsm.Checkout(new FastLoginViewState(fsm,
                        markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to edit list[/]",
                        loginRollbackTarget: new ListViewState(fsm, x),
                        abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                        abortMarkupMessage: "Back to main menu"
                    ));
                    return;
                }

                if (!ValidateInput())
                    return;

                int number = Convert.ToInt32(numberInput.CurrentInput);
                //todo czekboksy

                using var context = new XkomContext();
                var newProdList = new ListProduct()
                {
                    ProductId = product.Id,
                    ListId = x.Id,
                    Number = number
                };
                context.Add(newProdList);
                context.SaveChanges();

                fsm.Checkout(new ProductViewState(fsm, product));
            }), "lists");
        });
    }
    private bool ValidateInput()
    {
        printer.ClearMemoryGroup("errors");

        bool isValid = true;

        int number;
        string numberString = numberInput.CurrentInput;

        if (numberString == null) 
        {
            printer.AddRow(new Markup("[red]This field cannot be empty[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }
        else
        {
            number = Convert.ToInt32(numberString);
            if (number <= 0)
            {
                printer.AddRow(new Markup("[red]Number is to small[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
            else if (number > product.NumberAvailable)
            {
                printer.AddRow(new Markup("[red]Number is to high[/]").ToBasicConsoleRow(), "errors");
                isValid = false;
            }
        }

        if (numberString?.Length < 1)
        {
            printer.AddRow(new Markup("[red]Number of products must be greater than 0[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }

        return isValid;
    }

}

