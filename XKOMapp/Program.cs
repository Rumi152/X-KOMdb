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

        //TEMP
        //private static void FirstTest()
        //{
        //    bool end = false;

        //    printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow()); //creates row with standard header
        //    printer.AddRow(new BasicConsoleRow(new Text("Info\n")));
        //    printer.AddRow(new BasicConsoleRow(new Text("Info2\n")));
        //    printer.AddRow(StandardRenderables.StandardLine.ToBasicConsoleRow()); //creates row with standard separator line
            
        //    printer.StartContent(); //starts interactible content zone

        //    //row that is different when hovered
        //    printer.AddRow(new HoveredStylizationConsoleRow(
        //        new Text("White"),
        //        new Markup("[yellow]Yellow[/]")
        //    ));
        //    printer.AddRow(new BasicConsoleRow(new Text("Text1")));
        //    printer.AddRow(new BasicConsoleRow(new Markup("[red]Text2[/]"))); //stylized text
        //    printer.AddRow(new BasicConsoleRow(new Text("Text3")));
        //    printer.AddRow(new BasicConsoleRow(new Panel("TextInPanel").HeavyBorder().Header("Panel"))); //panel
        //    printer.AddRow(new BasicConsoleRow(new Text("Text5")));
        //    printer.AddRow(new HoveredStylizationConsoleRow(
        //        new Text("Text6"),
        //        new Text("No one expected the spanish inquisition")
        //    ));

        //    //row with interaction on clicked enter
        //    printer.AddRow(new InteractableConsoleRow(
        //        new Panel("PanelNr2").RoundedBorder().Header("Click me"),
        //        (row, _) => printer.AddRow(new BasicConsoleRow(new Text("Dynamiaclly added row")))));

        //    printer.AddRow(new InteractableConsoleRow(
        //            new Text("Click to end Test1"),
        //            (row, _) => end = true
        //        ));

        //    printer.ReloadBuffer(); //reloading buffers from rows list
        //    printer.PrintBuffer(); //rendering from buffers

        //    //checking for input in loop
        //    while (true)
        //    {
        //        if (end)
        //        {
        //            printer.ClearMemory();
        //            return;
        //        }

        //        if (Console.KeyAvailable)
        //        {
        //            var key = Console.ReadKey(true).Key;

        //            switch (key)
        //            {
        //                case ConsoleKey.DownArrow:
        //                    printer.CursorDown();
        //                    break;

        //                case ConsoleKey.UpArrow:
        //                    printer.CursorUp();
        //                    break;

        //                case ConsoleKey.Enter:
        //                    printer.Interract();
        //                    break;

        //                default:
        //                    break;
        //            }

        //            //refreshing screen
        //            printer.ClearScreen();
        //            printer.ReloadBuffer();
        //            printer.PrintBuffer();
        //        }
        //    }
        //}

        //TEMP
        //private static void SecondTest()
        //{
        //    printer.AddRow(StandardRenderables.StandardHeader.ToBasicConsoleRow()); //creates row with standard header
        //    printer.AddRow(new BasicConsoleRow(new Text("Info\n")));
        //    printer.AddRow(StandardRenderables.StandardLine.ToBasicConsoleRow()); //creates row with standard separator line

        //    printer.StartContent(); //starts interactible content zone

        //    printer.AddRow(new HideOnClickConsoleRow(new Text("Click to hide")));
        //    //row that is different when hovered
        //    printer.AddRow(new HoveredStylizationConsoleRow(
        //        new Text("Walter"),
        //        new Markup("White")
        //    ));
        //    printer.AddRow(new BasicConsoleRow(new Text("Text1")));
        //    printer.AddRow(new HideOnClickConsoleRow(new Text("Click to hide")));
        //    printer.AddRow(new BasicConsoleRow(new Text("Text1")));
        //    printer.AddRow(new BasicConsoleRow(new Text("Text1")));

        //    //row with interaction on clicked enter
        //    int counter = 0;
        //    printer.AddRow(new InteractableConsoleRow(
        //            new Panel("PanelNr2").RoundedBorder().Header("Click me"),
        //            (row, _) =>
        //            {
        //                counter++;
        //                ((InteractableConsoleRow)row).SetRenderContent(new Text(counter.ToString()));
        //                printer.ReloadBuffer();
        //                printer.PrintBuffer();
        //            }
        //        ));

        //    printer.AddRow(new BasicConsoleRow(new Text("Text2")));

        //    printer.AddRow(new HideOnClickConsoleRow(new Text("Click to hide")));

        //    printer.ReloadBuffer(); //reloading buffers from rows list
        //    printer.PrintBuffer(); //rendering from buffers

        //    //checking for input in loop
        //    while (true)
        //    {
        //        if (Console.KeyAvailable)
        //        {
        //            var key = Console.ReadKey(true).Key;

        //            switch (key)
        //            {
        //                case ConsoleKey.DownArrow:
        //                    printer.CursorDown();
        //                    break;

        //                case ConsoleKey.UpArrow:
        //                    printer.CursorUp();
        //                    break;

        //                case ConsoleKey.Enter:
        //                    printer.Interract();
        //                    break;

        //                default:
        //                    break;
        //            }

        //            //refreshing screen
        //            printer.ClearScreen();
        //            printer.ReloadBuffer();
        //            printer.PrintBuffer();
        //        }
        //    }
        //}

        //TEMP
        private static void ScrollTest()
        {
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
            printer.AddRow(new MultiLineConsoleRow(new Text("Text5\n\tParagraph1\n\tParagraph2\n\tParagrapgh3"), 4)); //row that takes more than 1 line
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