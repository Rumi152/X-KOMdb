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
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;


        var fsm = new ViewStateMachine();
        //DONT TOUCH THESE TIMUR
        fsm.SaveState("productsSearch", new ProductSearchViewState(fsm));
        fsm.SaveState("listBrowse", new ListBrowseViewState(fsm));

        //fsm.Checkout("productsSearch");
        //fsm.Checkout("listBrowse");
        fsm.Checkout(new RegisteringViewState(fsm));


        //checking for input in loop
        while (true)
        {
            fsm?.Tick();

            if (Console.KeyAvailable)
            {
                var info = Console.ReadKey(true);
                fsm?.PassKeystroke(info);
            }
        }
    }
}