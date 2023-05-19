using Spectre.Console;
using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows.ProductSearching;

public class ChoiceMenuParentConsoleRow : ICustomCursorConsoleRow, ISwitchableConsoleRow, IInteractableConsoleRow
{
    private readonly int childrenDisplaySize;
    private readonly int childrenStickyStart;
    private readonly Action? onAccept;
    private readonly Action? preEnter;
    private ConsolePrinter owner = null!;
    private readonly string markupPreText;
    private List<ChoiceMenuChildConsoleRow> children = new();

    private bool isActive = false;
    bool ISwitchableConsoleRow.IsActive { get => isActive; set => isActive = value; }

    private int choiceIndex = 0;
    private int appliedChoiceIndex = 0;


    public ChoiceMenuParentConsoleRow(string markupPreText, int childrenDisplaySize, int childrenStickyStart, Action? onAccept, Action? preEnter)
    {
        this.markupPreText = markupPreText;
        this.childrenDisplaySize = childrenDisplaySize;
        this.childrenStickyStart = childrenStickyStart;
        this.onAccept = onAccept;
        this.preEnter = preEnter;
    }

    public void SetChildren(List<ChoiceMenuChildConsoleRow> newChildren)
    {
        children = newChildren;
        children.ForEach(x => x.SetParent(this));
        choiceIndex = Math.Clamp(choiceIndex, 0, children.Count);
    }
    public string GetCurrentCategory()
    {
        return children.ElementAtOrDefault(appliedChoiceIndex)?.category ?? "All";
    }


    public string GetCustomCursor() => "\u00BB";
    public string GetCustomCursorBackground() => isActive ? "_" : " ";


    public IRenderable GetRenderContent() => new Markup(markupPreText + GetCurrentCategory());
    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;


    public void OnInteraction()
    {
        preEnter?.Invoke();

        if (children.Count > 0)
        {
            ((ISwitchableConsoleRow)this).TurnOn();
        }
    }
    public void ProcessChildrenStandardKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        owner.SetBufferDirty();

        if (keystrokeInfo.Key == owner.InteractionKey)
        {
            appliedChoiceIndex = choiceIndex;
            ((ISwitchableConsoleRow)this).TurnOff();
            onAccept?.Invoke();
            return;
        }

        if (keystrokeInfo.Key == owner.UpKey && choiceIndex > 0)
        {
            owner.CursorUp();
            choiceIndex--;
        }
        else if (keystrokeInfo.Key == owner.DownKey && choiceIndex < children.Count - 1)
        {
            owner.CursorDown();
            choiceIndex++;
        }

        RefreshNeededDisplay();
    }
    public void ProcessChildrenCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        owner.SetBufferDirty();

        if (!char.IsLetterOrDigit(keystrokeInfo.KeyChar))
            return;

        int x = children.FindIndex(x => x.category.StartsWith(keystrokeInfo.KeyChar.ToString(), true, null));

        if (x == -1)
            return;

        if (x == choiceIndex)
            return;

        int temp = choiceIndex;
        for (int i = x; i < temp; i++)
        {
            owner.CursorUp();
            choiceIndex--;
            RefreshNeededDisplay();
        }

        for (int i = x; i > temp; i--)
        {
            owner.CursorDown();
            choiceIndex++;
            RefreshNeededDisplay();
        }

        choiceIndex = x;
        RefreshNeededDisplay();
    }


    void ISwitchableConsoleRow.OnTurningOff()
    {
        children.ForEach(x => ((IHideableConsoleRow)x).TurnOff());
        choiceIndex = 0;

        owner.SetBufferDirty();
    }
    void ISwitchableConsoleRow.OnTurningOn()
    {
        RefreshNeededDisplay();

        owner.SetBufferDirty();
    }

    private void RefreshNeededDisplay()
    {
        owner.SetBufferDirty();

        children.ForEach(x => ((IHideableConsoleRow)x).TurnOff());

        int firstIndex = Enumerable.Range(0, children.Count)
            .ToList()
            .FindIndex(index => index > choiceIndex - childrenStickyStart || index > children.Count - childrenDisplaySize);

        children
            .Skip(Math.Clamp(firstIndex - 1, 0, children.Count))
            .Take(childrenDisplaySize)
            .ToList()
            .ForEach(x => ((IHideableConsoleRow)x).TurnOn());
    }

    public void ResetCategory()
    {
        choiceIndex = 0;
        appliedChoiceIndex = 0;
        owner.SetBufferDirty();
    }
}
