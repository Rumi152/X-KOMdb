using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XKOMapp.GUI.ConsoleRows.User;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI;
using XKOMapp.Models;
using XKOMapp.GUI.ConsoleRows.List;
using XKOMapp.GUI.ConsoleRows.ProductSearching;
using System.Security.Cryptography.X509Certificates;
namespace XKOMapp.ViewsFSM.States;

internal class ListViewState : ViewState
{
    private readonly List list;
    private readonly ListNameInputConsoleRow nameRow;
    public ListViewState(ViewStateMachine stateMachine, List list) : base(stateMachine)
    {
        this.list = list;
        nameRow = new($"Name: {list.Name} | New name: ", 32);
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to browsing"), (row, owner) =>
        {
            fsm.Checkout("listBrowse");
        }));
        printer.AddRow(new Rule("List").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.AddRow(nameRow);
        
        printer.AddRow(new BasicConsoleRow(new Text($"Link: {list.Link}"))); //TODO long display support

        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

        printer.AddRow(new InteractableConsoleRow(new Text("Delete unavailable"), (row, own) => throw new NotImplementedException())); //TODO deleting unavailavle products
        printer.AddRow(new InteractableConsoleRow(new Text("Clone list"), (row, own) =>
        {
            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to clone list[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            using var context = new XkomContext();
            context.Attach(dbUser);
            var clonedList = new List()
            {
                Name = $"{list.Name}-copy",
                Link = GetLink(),
                User = dbUser
            };
            context.Add(clonedList);
            context.SaveChanges();

            //TODO checkout this
        }));
        printer.AddRow(new InteractableConsoleRow(new Markup("[red]Delete list[/]"), (row, own) =>
        {
            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to delete list[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            using var context = new XkomContext();
            var deleteList = context.Lists.SingleOrDefault(x => x.Id == list.Id);
            if (deleteList is not null)
            {
                context.Remove(deleteList);
                context.SaveChanges();
            }

            fsm.Checkout("listBrowse");
        }));

        //TODO move to click on name input
        printer.AddRow(new InteractableConsoleRow(new Markup("[green]Save changes[/]"), (row, own) =>
        {
            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    markupMessage: $"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to create list[/]",
                    loginRollbackTarget: this,
                    abortRollbackTarget: fsm.GetSavedState("mainMenu"),
                    abortMarkupMessage: "Back to main menu"
                ));
                return;
            }

            if (!ValidateInput())
                return;

            using var context = new XkomContext();
            var updatelist = context.Lists.SingleOrDefault(x => x.Id == list.Id);

            if(updatelist is not null)
            {
                updatelist.Name = nameRow.CurrentInput;
                context.SaveChanges();
            }

            //TODO rebuild name
        }));

        printer.StartGroup("errors");
    }

    public override void OnEnter()
    {
        base.OnEnter();

        printer.ClearMemoryGroup("errors");
        printer.ResetCursor();
    }

    private static string GetLink()
    {
        string link = @"https://www.x-kom.pl/list/";
        var random = new Random();
        List<int> notAvailalbe = new() { 58, 59, 60, 61, 62, 63, 64, 91, 92, 93, 94, 95, 96 };

        while (link.Length < 128)
        {
            int randomNumber = random.Next(48, 122);
            char a = Convert.ToChar(randomNumber);
            if (!notAvailalbe.Contains(a))
            {
                link += a;
            }
        }
        using (var context = new XkomContext())
            if (context.Lists.Any(x => x.Link == link))
                return GetLink();

        return link;
    }

    private bool ValidateInput()
    {
        printer.ClearMemoryGroup("errors");

        string name = nameRow.CurrentInput;

        if (name.Length == 0)
            return false;

        if (name.Length < 2)
        {
            printer.AddRow(new Markup("[red]Name is too short[/]").ToBasicConsoleRow(), "errors");
            return false;
        }

        if (name.Length > 32)
        {
            printer.AddRow(new Markup("[red]Name is too long[/]").ToBasicConsoleRow(), "errors");
            return false;
        }
        return true;
    }
}


