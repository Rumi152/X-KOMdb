using System;
using Spectre.Console;

public static class AnsiConsoleExtensions
{
    public static void StandardHeader()
    {
        AnsiConsole.Write(new Rule("[#b45ae0]X[/]-KOM"));
    }
}
