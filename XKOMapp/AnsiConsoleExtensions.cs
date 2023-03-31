using System;
using Spectre.Console;

public static class AnsiConsoleExtensions
{
    public static void StandardHeader()
    {
        AnsiConsole.Write(new Rule("[#b45ae0]X[/]-KOM"));
    }

    public static void StandardRule()
    {
        AnsiConsole.Write(new Rule().HeavyBorder().RuleStyle(Style.Parse("#0e8f75")));
    }
}
