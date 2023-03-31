using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI;

public class ConsoleRow
{
    private readonly IRenderable renderContent;
    private readonly ConsoleRowInteractionAction interactionAction;

    public ConsoleRow(IRenderable renderContent, ConsoleRowInteractionAction interactionAction)
    {
        this.renderContent = renderContent;
        this.interactionAction = interactionAction;
    }

    public ConsoleRow(IRenderable renderContent)
    {
        this.renderContent = renderContent;
        this.interactionAction = null;
    }

    public IRenderable GetRenderContent() => renderContent;
    public void OnInteraction() => interactionAction?.Invoke();
}

public delegate void ConsoleRowInteractionAction();
