using Spectre.Console;
using System.Diagnostics;
using System.Reflection;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;

namespace XKOMapp
{
    internal class Program
    {
        static ConsolePrinter printer;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            printer = new ConsolePrinter();

            ScrollTest();
        }

        private static void ScrollTest()
        {
            printer.EnableScrolling();

            printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow());
            printer.AddRow(new BasicConsoleRow(new Text("Header1\n")));
            printer.AddRow(new BasicConsoleRow(new Text("Header2\n")));
            printer.AddRow(StandardRenderables.StandardSeparator.ToBasicConsoleRow());

            printer.StartContent(); //starts interactible content zone

            //row that creates new rows on click
            int counter = 0;
            printer.AddRow(new InteractableConsoleRow(
                new Text("ClickMe"),
                (row, _) => printer.AddRow(new BasicConsoleRow(new Text("AddedRow" + ++counter)))));

            printer.AddRow(new BasicConsoleRow(new Text("Text1")));
            printer.AddRow(new MultiLineConsoleRow(new Text("Text2\n\tParagraph1\n\tParagraph2\n\tParagrapgh3"), 4)); //row that takes more than 1 line
            printer.AddRow(new BasicConsoleRow(new Text("Text3")));
            printer.AddRow(new BasicConsoleRow(new Text("Text4")));

            List<IFocusableConsoleRow> focusableRows = new();
            for(int i = 0; i < 4; i++)
                focusableRows.Add( new FocusableConsoleRow(new Text("\tFocused" + 1)));
            printer.AddRow(new FocusableGroupParentConsoleRow(
                new Text("FocusMode"),
                focusableRows
                ));
            focusableRows.ForEach(x => printer.AddRow(x));

            printer.AddRow(new MultiLineConsoleRow(new Text("Text5\n\tParagrapgh\n\tParagrapgh\n\tParagrapgh\n\tParagrapgh"), 5)); //row that takes more than 1 line
            printer.AddRow(new BasicConsoleRow(new Text("Text6")));
            printer.AddRow(new MultiLineConsoleRow(new Text("Text7\n\tParagraph1\n\tParagraph2\n\tParagrapgh3"), 4)); //row that takes more than 1 line
            printer.AddRow(new HideOnClickConsoleRow(new Text("ClickToHide"))); //row that disapears on click

            //row that creates new rows on click
            int counter2 = 0;
            printer.AddRow(new InteractableConsoleRow(
                new Text("ClickMe"),
                (row, _) => printer.AddRow(new BasicConsoleRow(new Text("AddedRow" + ++counter2)))));

            printer.AddRow(new BasicConsoleRow(new Text("Text8")));
            printer.AddRow(new BasicConsoleRow(new Text("Text9")));

            printer.ReloadBuffer();
            printer.PrintBuffer();

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