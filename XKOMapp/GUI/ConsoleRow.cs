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
    public IRenderable renderContent;
    public IRenderable hoveredRenderContent;
    private readonly ConsoleRowAction interactionAction;

    public ConsoleRow(IRenderable renderContent, ConsoleRowAction interactionAction)
    {
        this.renderContent = renderContent;
        this.hoveredRenderContent = renderContent;
        this.interactionAction = interactionAction;
    }

    public ConsoleRow(IRenderable renderContent, IRenderable hoveredRenderContent, ConsoleRowAction interactionAction)
    {
        this.renderContent = renderContent;
        this.hoveredRenderContent = hoveredRenderContent;
        this.interactionAction = interactionAction;
    }

    public ConsoleRow(IRenderable renderContent)
    {
        this.renderContent = renderContent;
        this.hoveredRenderContent = renderContent;
        interactionAction = null;
    }

    public ConsoleRow(IRenderable renderContent, IRenderable hoveredRenderContent)
    {
        this.renderContent = renderContent;
        this.hoveredRenderContent = hoveredRenderContent;
        interactionAction = null;
    }

    public IRenderable GetRenderContent(bool hovered = false) => hovered ? hoveredRenderContent : renderContent;
    public void OnInteraction() => interactionAction?.Invoke(this);
}

public delegate void ConsoleRowAction(ConsoleRow row);
