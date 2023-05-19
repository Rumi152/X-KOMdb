﻿using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;

namespace XKOMapp.ViewsFSM.States;

internal class ErrorViewState : ViewState
{
    public ErrorViewState(ViewStateMachine stateMachine, Exception exception) : base(stateMachine)
    {
        printer = new ConsolePrinter();

        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Text("Back to menu"), (row, owner) => fsm.Checkout("mainMenu")));
        printer.AddRow(new Text(exception.Message).ToBasicConsoleRow());
    }
}
