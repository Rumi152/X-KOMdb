using Spectre.Console;
using System.Diagnostics;
using System.Reflection;
using XKOMapp.GUI;
using XKOMapp.GUI.ConsoleRows;

namespace XKOMapp
{
    internal class Program
    {
        static ConsolePrinter printer = null!;

        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            printer = new ConsolePrinter();
        }
    }
}