using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.Models;

namespace XKOMapp.GUI.ConsoleRows.ProductSearching;

public class CategorySearchParentConsoleRow : ICustomCursorConsoleRow, ISwitchableConsoleRow, IInteractableConsoleRow
{
    private int childrenDisplaySize;
    private int childrenStickyStart;
    private readonly ConsoleRowAction? onAccept;
    private readonly ConsoleRowAction? preEnter;
    private ConsolePrinter owner = null!;
    private readonly string markupPreText;
    private List<CategorySearchChildConsoleRow> children = new List<CategorySearchChildConsoleRow>();

    private bool isActive = false;
    bool ISwitchableConsoleRow.IsActive { get => isActive; set => isActive = value; }

    private int choiceIndex = 0;
    private int appliedChoiceIndex = 0;


    public CategorySearchParentConsoleRow(string markupPreText, int childrenDisplaySize, int childrenStickyStart, ConsoleRowAction? onAccept, ConsoleRowAction? preEnter)
    {
        this.markupPreText = markupPreText;
        this.childrenDisplaySize = childrenDisplaySize;
        this.childrenStickyStart = childrenStickyStart;
        this.onAccept = onAccept;
        this.preEnter = preEnter;
    }

    public void SetChildren(List<CategorySearchChildConsoleRow> newChildren)
    {
        this.children = newChildren;
        children.ForEach(x => x.SetParent(this));
        choiceIndex = Math.Clamp(choiceIndex, 0, children.Count);
    }
    public string GetCurrentCategory()
    {
        return children.ElementAtOrDefault(appliedChoiceIndex)?.category ?? "All";
    }


    public string GetCustomCursor() => isActive ? ">" : ">";
    public string GetCustomCursorBackground() => isActive ? "_" : " ";


    public IRenderable GetRenderContent() => new Markup(markupPreText + GetCurrentCategory());
    public void SetOwnership(ConsolePrinter owner) => this.owner = owner;


    public void OnInteraction()
    {
        preEnter?.Invoke(this, owner);

        if (children.Count > 0)
        {
            ((ISwitchableConsoleRow)this).TurnOn();
        }
    }
    public bool ProcessChildrenStandardKeystroke(ConsoleKeyInfo keystrokeInfo)
    {
        if (keystrokeInfo.Key == owner.InteractionKey)
        {
            appliedChoiceIndex = choiceIndex;
            ((ISwitchableConsoleRow)this).TurnOff();
            onAccept?.Invoke(this, owner);
            return false;
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

        return false;
    }


    void ISwitchableConsoleRow.OnTurningOff()
    {
        children.ForEach(x => ((IHideableConsoleRow)x).TurnOff());
        choiceIndex = 0;
    }
    void ISwitchableConsoleRow.OnTurningOn()
    {
        RefreshNeededDisplay();
    }

    private void RefreshNeededDisplay()
    {
        children.ForEach(x => ((IHideableConsoleRow)x).TurnOff());

        var firstIndex = Enumerable.Range(0, children.Count)
            .ToList()
            .FindIndex(index => index > choiceIndex - childrenStickyStart || index > children.Count - childrenDisplaySize);

        children
            .Skip(Math.Clamp(firstIndex - 1, 0, children.Count))
            .Take(childrenDisplaySize)
            .ToList()
            .ForEach(x => ((IHideableConsoleRow)x).TurnOn());
    }
}
