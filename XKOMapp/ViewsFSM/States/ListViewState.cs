﻿using Spectre.Console;
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

internal class ListViewState: ViewState
{
    private readonly List list;
    ListNameInputConsoleRow nameRow;
    public ListViewState(ViewStateMachine stateMachine, List list) : base(stateMachine)
    {
        this.list = list;
        printer = new ConsolePrinter();

        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();


        printer.AddRow(new InteractableConsoleRow(new Text("Click to abort"), (row, owner) =>
        {
            //TODO
            fsm.RollbackOrDefault("ListBrowseViewState");
            throw new NotImplementedException();
        }));
        printer.AddRow(new Rule("List").RuleStyle(Style.Parse(StandardRenderables.AquamarineColorHex)).HeavyBorder().ToBasicConsoleRow());
        printer.EnableScrolling();

        nameRow = new ListNameInputConsoleRow($"Name: {list.Name} | New name: ", 32);
        printer.AddRow(nameRow);
        printer.AddRow(new BasicConsoleRow(new Text($"Link: {list.Link}")));

        printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

        printer.AddRow(new InteractableConsoleRow(new Text("Delete unavailable"), (row, own) => throw new NotImplementedException())); //TODO deleting unavailavle products
        printer.AddRow(new InteractableConsoleRow(new Text("Clone list"), (row, own) =>
        {
            if (!SessionData.IsLoggedIn())
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[{StandardRenderables.GrassColorHex}]Log in to clone list[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                return;
            }

            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to clone list[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
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
        }));
        printer.AddRow(new InteractableConsoleRow(new Markup("[red]Delete list[/]"), (row, own) => 
        {
            if (!SessionData.IsLoggedIn())
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[{StandardRenderables.GrassColorHex}]Log in to delete list[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                return;
            }

            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to delete list[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                return;
            }

            using var context = new XkomContext();
            var deleteList = context.Lists.SingleOrDefault(x => x.Id == list.Id);
            if (deleteList != null)
            {
                context.Remove(deleteList);
                context.SaveChanges();
            }
                
            fsm.Checkout("ListBrowseViewState");
        }));
        printer.AddRow(new InteractableConsoleRow(new Text("Save changes"), (row, own) =>
        {
            if (!SessionData.IsLoggedIn())
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[{StandardRenderables.GrassColorHex}]Log in to edit list[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                return;
            }

            if (SessionData.HasSessionExpired(out User dbUser))
            {
                fsm.Checkout(new FastLoginViewState(fsm,
                    new Markup($"[red]Session expired[/] - [{StandardRenderables.GrassColorHex}]Log in to edit list[/]").ToBasicConsoleRow(),
                    new InteractableConsoleRow(new Markup("Click to abort"), (row, owner) => fsm.Checkout(this))));
                return;
            }

            if (!ValidateInput())
                return;
            if (nameRow.CurrentInput.Length != 0)
            {
                using var context = new XkomContext();
                var updatelist = context.Lists.First(x => x.Id == list.Id);
                updatelist.Name = nameRow.CurrentInput;
                context.SaveChanges();
            }

            fsm.Checkout("ListBrowseViewState");
        }));

        printer.StartGroup("errors");
    }

    public override void OnEnter()
    {
        base.OnEnter();

        printer.ResetCursor();
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

    private static string GetLink()
    {
        string link = "https://www.x-kom.pl/list/";
        var random = new Random();
        List<int> notAvailalbe = new List<int> { 58, 59, 60, 61, 62, 63, 64, 91, 92, 93, 94, 95, 96 };

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

        bool isValid = true;

        string name = nameRow.CurrentInput;

        if (name.Length == 0)
            return isValid;
        if (name.Length < 2)
        {
            printer.AddRow(new Markup("[red]Name is too short[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }
        else if (name.Length > 32)
        {
            printer.AddRow(new Markup("[red]Name is too long[/]").ToBasicConsoleRow(), "errors");
            isValid = false;
        }
        return isValid;
    }
}

