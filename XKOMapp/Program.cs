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
        var fsm = new ViewStateMachine();
        fsm.AddState("productsSearch", new ProductSearchViewState(fsm, "Laptop 15s AMD ryzen", 9870000.99m, "HP"));
        fsm.Checkout("productsSearch");

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