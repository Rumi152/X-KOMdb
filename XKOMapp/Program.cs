using System.Reflection;
using XKOMapp.GUI;

namespace XKOMapp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            var printer = new ConsolePrinter();

            //checking for input in loop
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var info = Console.ReadKey(true);
                    var key = info.Key;

                    switch (key)
                    {
                        case ConsoleKey.DownArrow:
                            printer.CursorDown();
                            break;

                        case ConsoleKey.UpArrow:
                            printer.CursorUp();
                            break;

                        case ConsoleKey.Enter:
                            printer.Interract();
                            break;

                        default:
                            printer.ProcessCustomKeystroke(info);
                            break;
                    }

                    //refreshing screen
                    printer.ClearScreen();
                    printer.ReloadBuffer();
                    printer.PrintBuffer();
                }
            }
        }
    }
}