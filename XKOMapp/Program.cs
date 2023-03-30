using Spectre.Console;

namespace XKOMapp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AnsiConsole.Write(new FigletText("X-KOM"));
            AnsiConsole.WriteLine();            

            AnsiConsole.MarkupLine("[green]I'm hungry![/]");
            AnsiConsole.MarkupLine("[yellow]Hello hungry, I'm dad[/]");

            Console.ReadLine();
        }
    }
}