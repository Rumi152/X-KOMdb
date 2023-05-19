using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows.ProductDetails;
internal class ReviewDescriptionInputConsoleRow : IDeactivableConsoleRow, ICustomCursorConsoleRow, IHoverConsoleRow, ICustomKeystrokeListenerConsoleRow
{
    private readonly IRenderable renderable;
    private readonly bool isFirst;
    private readonly bool isLast;
    private readonly Func<bool> isDescriptionHoveredGetter;
    private readonly Func<string> descriptionGetter;
    private readonly Action<string> descriptionSetter;
    private readonly Action onCharacterWrite;

    public bool IsHovered { get; private set; } = false;

    private ConsolePrinter owner = null!;

    public ReviewDescriptionInputConsoleRow(IRenderable renderable, bool isFirst, bool isLast, Func<bool> isDescriptionHoveredGetter, Func<string> descriptionGetter, Action<string> descriptionSetter, Action onCharacterWrite)
    {
        this.renderable = renderable;
        this.isFirst = isFirst;
        this.isLast = isLast;
        this.isDescriptionHoveredGetter = isDescriptionHoveredGetter;
        this.descriptionGetter = descriptionGetter;
        this.descriptionSetter = descriptionSetter;
        this.onCharacterWrite = onCharacterWrite;
    }


    bool ISwitchableConsoleRow.IsActive { get => isFirst; set => throw new InvalidOperationException(); }
    void ISwitchableConsoleRow.OnTurningOff() => throw new InvalidOperationException();
    void ISwitchableConsoleRow.OnTurningOn() => throw new InvalidOperationException();

    public IRenderable GetRenderContent() => renderable;
    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

    public string GetCustomCursor()
    {
        string content;
        if (isFirst && isLast)
            content = "[[";
        else if (isFirst)
            content = "┌";
        else if (isLast)
            content = "└";
        else
            content = "│";

        content = (isDescriptionHoveredGetter() ? "[blink]" : "[dim]") + content + "[/]";

        return content;
    }
    public string GetCustomCursorBackground() => GetCustomCursor();

    public void OnHoverStart() => IsHovered = true;
    public void OnHoverEnd() => IsHovered = false;

    public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        string description = descriptionGetter();

        bool changed = false;

        if (char.IsLetterOrDigit(keystrokeInfo.KeyChar))
        {
            if (description.Length < 256)
            {
                description += keystrokeInfo.KeyChar;
                changed = true;
            }
        }
        else if (new List<char>() { ' ', '@', ',', '.', ')', '(', '!', '#', '*', '&', '+', '-', '?' }.Contains(keystrokeInfo.KeyChar))
        {
            if (description.Length < 256)
            {
                description += keystrokeInfo.KeyChar;
                changed = true;
            }
        }
        else if (keystrokeInfo.Key == ConsoleKey.Backspace)
        {
            if (description.Length > 0)
            {
                description = description[..^1];
                changed = true;
            }
        }

        if (!changed)
            return;

        owner.SetBufferDirty();
        descriptionSetter(description);
        onCharacterWrite();
    }
}
