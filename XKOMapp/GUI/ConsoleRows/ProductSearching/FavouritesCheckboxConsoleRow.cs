using Spectre.Console;
using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows.ProductSearching;

internal class FavouritesCheckboxConsoleRow : IInteractableConsoleRow, ICustomCursorConsoleRow
{
    private readonly string preMarkupLabel;
    private readonly Action onInteraction;
    private ConsolePrinter owner = null!;

    public bool IsChecked { get; private set; } = false;

    public FavouritesCheckboxConsoleRow(string preMarkupLabel, Action onInteraction)
    {
        this.preMarkupLabel = preMarkupLabel;
        this.onInteraction = onInteraction;
    }

    public IRenderable GetRenderContent()
    {
        string checkbox = IsChecked ? $"[{StandardRenderables.GoldColorHex}][[X]][/]" : "[[]]";
        return new Markup($"{preMarkupLabel}{checkbox}");
    }

    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

    public string GetCustomCursor() => "»";
    public string GetCustomCursorBackground() => " ";

    public void OnInteraction()
    {
        owner.SetBufferDirty();
        IsChecked = !IsChecked;
        onInteraction();
    }

    public void Uncheck()
    {
        owner.SetBufferDirty();
        IsChecked = false;
    }
}
