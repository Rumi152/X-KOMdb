using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows.ProductDetails;

[Obsolete("Shall be deleted", true)]
internal class ReviewInputDescriptionConsoleRow : IDeactivableConsoleRow, ICustomCursorConsoleRow, ICustomKeystrokeListenerConsoleRow
{
    private readonly bool canMouseOver;
    private readonly Func<bool> isMouseOverDescriptionGetter;
    private readonly Action<string> descriptionSetter;
    private readonly Func<string> descriptionGetter;

    private ConsolePrinter owner = null!;

    public ReviewInputDescriptionConsoleRow(bool canMouseOver, Func<bool> isMouseOverDescriptionGetter, Action<string> descriptionSetter, Func<string> descriptionGetter)
    {
        this.canMouseOver = canMouseOver;
        this.isMouseOverDescriptionGetter = isMouseOverDescriptionGetter;
        this.descriptionSetter = descriptionSetter;
        this.descriptionGetter = descriptionGetter;
    }


    bool ISwitchableConsoleRow.IsActive { get => canMouseOver; set => throw new NotImplementedException(); }

    public string GetCustomCursor() => isMouseOverDescriptionGetter() ? "[[" : " ";
    public string GetCustomCursorBackground() => isMouseOverDescriptionGetter() ? "[[" : " ";
    public IRenderable GetRenderContent() => throw new NotImplementedException();
    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

    void ISwitchableConsoleRow.OnTurningOff() => throw new NotSupportedException();
    void ISwitchableConsoleRow.OnTurningOn() => throw new NotSupportedException();

    public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        var description = descriptionGetter();

        owner.SetBufferDirty();

        if (char.IsLetterOrDigit(keystrokeInfo.KeyChar))
        {
            if (description.Length < 256)
                description += keystrokeInfo.KeyChar;
        }
        else if (new List<char>() { ' ', '@', ',', '.', ')', '(', '!', '#', '*', '&', '+', '-', '?' }.Contains(keystrokeInfo.KeyChar))
        {
            if (description.Length < 256)
                description += keystrokeInfo.KeyChar;
        }
        else if (keystrokeInfo.Key == ConsoleKey.Backspace)
        {
            if (description.Length > 0)
                description = description[..^1];
        }

        descriptionSetter(description);
    }
}
