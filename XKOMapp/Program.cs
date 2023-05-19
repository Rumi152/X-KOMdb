using XKOMapp.ViewsFSM;
using XKOMapp.ViewsFSM.States;

namespace XKOMapp;
internal class Program
{
    private static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.Title = "X-KOMapp";
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;

        ViewStateMachine? fsm = new();
        //DONT TOUCH THESE TIMUR
        fsm.SaveState("productsSearch", new ProductSearchViewState(fsm));
        fsm.SaveState("listBrowse", new ListBrowseViewState(fsm));
        fsm.SaveState("mainMenu", new MainMenuViewState(fsm));

        fsm.Checkout("mainMenu");

        //checking for input in loop
        while (true)
        {
            try
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo info = Console.ReadKey(true);
                    fsm?.PassKeystroke(info);
                }

                fsm?.Tick();
            }
            catch (Exception ex)
            {
                fsm?.Checkout(new ErrorViewState(fsm, ex));
            }
        }
    }
}