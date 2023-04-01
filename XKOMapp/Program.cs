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

            printer.AddRow(AnsiConsoleExtensions.StandardHeader.ToConsoleRow()); //creates row with standard header
            printer.AddRow(new ConsoleRow(new Text("Info")));
            printer.AddRow(new ConsoleRow(new Text("Info2")));
            printer.AddRow(AnsiConsoleExtensions.StandardLine.ToConsoleRow()); //creates row with standard separator line
            printer.StartContent(); //starts interactible content

            //row that is different when hovered
            printer.AddRow(new ConsoleRow(
                new Text("White"),
                new Markup("[yellow]Yellow[/]")
            ));
            printer.AddRow(new ConsoleRow(new Text("Text1")));
            printer.AddRow(new ConsoleRow(new Markup("[red]Text2[/]"))); //stylized text
            printer.AddRow(new ConsoleRow(new Text("Text3")));
            printer.AddRow(new ConsoleRow(new Panel("Text4").HeavyBorder().Header("Ex"))); //panel
            printer.AddRow(new ConsoleRow(new Text("Text5")));

            //row with interaction on clicked enter
            printer.AddRow(new ConsoleRow(
                new Panel("Yooo11 (Sequel)").RoundedBorder().Header("Luigi"),
                (row) => printer.AddRow(new ConsoleRow(new Text("no one expected the spanish inquisition")))));

            printer.ReloadBuffer(); //reloading buffers from rows list
            printer.PrintBuffer(); //rendering from buffers

            //checking for input in loop
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

                    //refreshing screen
                    printer.ClearScreen();
                    printer.ReloadBuffer();
                    printer.PrintBuffer();
                }
            }

        }
    }
}