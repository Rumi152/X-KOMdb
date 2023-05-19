using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows;

public class InteractableConsoleRow : BasicConsoleRow, IInteractableConsoleRow
{
    private readonly ConsoleRowAction interactionAction;

    public void OnInteraction() => interactionAction?.Invoke(this, owner);


    public InteractableConsoleRow(IRenderable renderContent, ConsoleRowAction interactionAction) : base(renderContent)
    {
        this.interactionAction = interactionAction;
    }
}
