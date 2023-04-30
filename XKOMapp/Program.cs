using Spectre.Console;
using XKOMapp.GUI;
using XKOMapp.Models;
using XKOMapp.ViewsFSM;
using XKOMapp.ViewsFSM.States;

namespace XKOMapp;

internal class Program
{
    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.Title = "X-KOMapp";

        var fsm = new ViewStateMachine();
        fsm.SaveState("productsSearch", new ProductSearchViewState(fsm));
        fsm.Checkout("productsSearch");

        fsm.Checkout(new FastLoginViewState(fsm, new Markup("[red]Session expired[/]\n").ToBasicConsoleRow()));

        //checking for input in loop
        while (true)
        {
            if (Console.KeyAvailable)
            {
                var info = Console.ReadKey(true);
                fsm?.PassKeystroke(info);
            }
        }
    }
}