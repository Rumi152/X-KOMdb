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
        fsm.SaveState("ListBrowseViewState", new ListBrowseViewState(fsm));
        fsm.Checkout("ListBrowseViewState");

        //SessionData.TryLogIn("emailtymek", "rogo123");

        //fsm.Checkout(new UserDetailsViewState(fsm));

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