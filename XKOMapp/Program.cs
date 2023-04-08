using XKOMapp.GUI;

namespace XKOMapp;

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
                printer.PassKeystroke(info);

                //refreshing screen
                printer.ClearScreen();
                printer.PrintMemory();
            }
        }
    }
}