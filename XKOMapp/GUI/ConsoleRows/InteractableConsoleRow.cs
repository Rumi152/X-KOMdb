using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
