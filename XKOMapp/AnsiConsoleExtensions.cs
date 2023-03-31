using System;
using Spectre.Console;
using Spectre.Console.Rendering;
using XKOMapp.GUI;

public static class AnsiConsoleExtensions
{
    public static IRenderable StandardHeader => new Rule("[#b45ae0]X[/]-KOM");
    public static IRenderable StandardLine => new Rule().HeavyBorder().RuleStyle(Style.Parse("#0e8f75"));

    public static ConsoleRow ToConsoleRow(this IRenderable renderable)
    {
        return new ConsoleRow(renderable);
    }
}
