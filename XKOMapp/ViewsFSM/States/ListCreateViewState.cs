using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XKOMapp.GUI.ConsoleRows;
using XKOMapp.GUI;
using XKOMapp.Models;
using XKOMapp.GUI.ConsoleRows.List;

namespace XKOMapp.ViewsFSM.States;

internal class ListCreateViewState : ViewState
{
    const int labelPad = 4;
    private readonly ListNameInputConsoleRow nameRow = new($"{"Name",-labelPad} : ", 32, () => { });

    public ListCreateViewState(ViewStateMachine stateMachine) : base(stateMachine)
    {
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Click to abort"), (row, owner) =>
        {
            fsm.Checkout("listBrowse");
        }));

        printer.AddRow(new Rule("Create list").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        printer.AddRow(nameRow);

        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

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

            if (!ValidateInput())
                return;

            using var context = new XkomContext();
            context.Attach(dbUser);
            var newList = new List()
            {
                Name = nameRow.CurrentInput,
                Link = GetLink(),
                User = dbUser
            };
            context.Add(newList);
            context.SaveChanges();

            fsm.Checkout(new ListViewState(fsm, newList));
        }));

        printer.StartGroup("errors");
    }

    public override void OnEnter()
    {
        base.OnEnter();

        nameRow.ResetInput();
        printer.ClearMemoryGroup("errors");
        printer.ResetCursor();
    }

    private static string GetLink()
    {
        string link = "https://www.x-kom.pl/list/";
        var random = new Random();
        List<int> notAvailalbe = new() { 58, 59, 60, 61, 62, 63, 64, 91, 92, 93, 94, 95, 96 };

        while (link.Length<128)
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
