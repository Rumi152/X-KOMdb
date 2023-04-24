using System;
using Spectre.Console;
using Spectre.Console.Rendering;
using XKOMapp.GUI.ConsoleRows;

namespace XKOMapp.GUI;

public static class StandardRenderables
{
    /// <summary>
    /// Standard header IRenderable
    /// </summary>
    public static IRenderable StandardHeader => new Rule("[#b45ae0]X[/]-KOM");

    /// <summary>
    /// Standard line separator IRenderable
    /// </summary>
    public static IRenderable StandardSeparator => new Rule().HeavyBorder().RuleStyle(Style.Parse(AquamarineColorHex));

    /// <summary>
    /// Converts IRenderable to simplest form of Console row
    /// </summary>
    /// <param name="renderable">IRenderable to convert</param>
    /// <returns>ConsoleRow without interaction and onHover stylization</returns>
    public static IConsoleRow ToBasicConsoleRow(this IRenderable renderable)
    {
        return new BasicConsoleRow(renderable);
    }

    public static Color GoldColor => new Color(0xFF, 0xA6, 0x00);
    public static string GoldColorHex => "#FFA600";

    public static Color AquamarineColor => new Color(0x0E, 0x8F, 0x75);
    public static string AquamarineColorHex => "#0E8F75";
}
