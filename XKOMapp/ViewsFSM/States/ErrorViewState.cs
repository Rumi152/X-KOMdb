using Spectre.Console;
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
    private readonly Exception exception;
    private readonly ViewState buttonTarget;
    private readonly string buttonMessage;

    public ErrorViewState(ViewStateMachine stateMachine, Exception exception) : base(stateMachine)
    {
        this.exception = exception;
        this.buttonTarget = fsm.GetSavedState("mainMenu");
        this.buttonMessage = "Back to main menu";
    }

    public ErrorViewState(ViewStateMachine stateMachine, Exception exception, ViewState buttonTarget, string buttonMessage) : base(stateMachine)
    {
        this.exception = exception;
        this.buttonTarget = buttonTarget;
        this.buttonMessage = buttonMessage;
    }

    protected override void InitialPrinterBuild(ConsolePrinter printer)
    {
        printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
        printer.StartContent();

        printer.AddRow(new InteractableConsoleRow(new Markup(buttonMessage), (row, owner) => fsm.Checkout(buttonTarget)));
        printer.AddRow(new InteractableConsoleRow(new Markup("[red]Exit app[/]"), (row, owner) => Environment.Exit(0)));
        GetWrappedMessage(exception.Message).ForEach(x => printer.AddRow(new Text(x).ToBasicConsoleRow()));
    }

    public override void OnEnter()
    {
        base.OnEnter();

        printer.ResetCursor();
    }

    //REFACTOR maybe add it to ConsolePrinter
    private static List<string> GetWrappedMessage(string message)
    {
        var text = message.ReplaceLineEndings(" ");
        List<string> lines = new();

        while (text.Length > 0)
        {
            const string leftPad = "  ";
            int width = Math.Max(32, Console.WindowWidth - 8);

            if (text.Length <= width)
            {
                lines.Add(leftPad + text.Trim());
                break;
            }

            int takenLength = new string(text.Take(width).ToArray()).LastIndexOf(' ');
            if (takenLength++ == -1) takenLength = width;

            lines.Add(leftPad + new string(text.Take(takenLength).ToArray()).Trim());
            text = new(text.Skip(takenLength).ToArray());
        }

        return lines;
    }
}

