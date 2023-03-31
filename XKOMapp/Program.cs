using Spectre.Console;
using System.Diagnostics;
using XKOMapp.GUI;

namespace XKOMapp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            var printer = new ConsolePrinter();

            printer.AddRow(AnsiConsoleExtensions.StandardHeader.ToConsoleRow());
            printer.AddRow(new ConsoleRow(new Text("Yoo1")));
            printer.AddRow(new ConsoleRow(new Text("Yoo2")));
            printer.AddRow(new ConsoleRow(new Text("Yoo3")));
            printer.AddRow(new ConsoleRow(new Text("Yoo4")));
            printer.AddRow(AnsiConsoleExtensions.StandardLine.ToConsoleRow());
            printer.StartContent();
            printer.AddRow(new ConsoleRow(
                new Text("Yoo5"),
                new Markup("[yellow]Yoo5[/]")
            ));
            printer.AddRow(new ConsoleRow(new Text("Yoo6")));

            printer.AddRow(new ConsoleRow(new Markup("[red]Yooo7[/]")));

            printer.AddRow(new ConsoleRow(new Text("Yoo8")));

            printer.AddRow(new ConsoleRow(new Panel("Yooo9 (Prequel)").HeavyBorder().Header("Mario")));

            printer.AddRow(new ConsoleRow(new Text("Yoo10")));

            printer.AddRow(new ConsoleRow(
                new Panel("Yooo11 (Sequel)").RoundedBorder().Header("Luigi"),
                (row) => printer.AddRow(new ConsoleRow(new Text("no one expected the spanish inquisition")))));

            printer.ReloadBuffer();
            printer.PrintBuffer();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true).Key;

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
                            break;
                    }

                    printer.ClearScreen();
                    printer.ReloadBuffer();
                    printer.PrintBuffer();
                }
            }

        }
    }
}