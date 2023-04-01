using System;
using Spectre.Console;
using Spectre.Console.Rendering;
using XKOMapp.GUI;

public static class AnsiConsoleExtensions
{
    /// <summary>
    /// Standard header IRenderable
    /// </summary>
    public static IRenderable StandardHeader => new Rule("[#b45ae0]X[/]-KOM");

    /// <summary>
    /// Standard line separator IRenderable
    /// </summary>
    public static IRenderable StandardLine => new Rule().HeavyBorder().RuleStyle(Style.Parse("#0e8f75"));

    /// <summary>
    /// Converts IRenderable to simplest form of Console row
    /// </summary>
    /// <param name="renderable">IRenderable to convert</param>
    /// <returns>ConsoleRow without interaction and onHover stylization</returns>
    public static ConsoleRow ToConsoleRow(this IRenderable renderable)
    {
        return new ConsoleRow(renderable);
    }

    public static void StandardRule()
    {
        AnsiConsole.Write(new Rule().HeavyBorder().RuleStyle(Style.Parse("#0e8f75")));
    }
}
