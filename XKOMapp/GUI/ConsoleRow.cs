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
    private IRenderable renderContent;
    private IRenderable hoveredRenderContent;
    private readonly ConsoleRowAction interactionAction;

    /// <summary>
    /// Creates row with interaction
    /// </summary>
    /// <param name="renderContent">Content rendered to console</param>
    /// <param name="interactionAction">Action on interaction</param>
    public ConsoleRow(IRenderable renderContent, ConsoleRowAction interactionAction)
    {
        this.renderContent = renderContent;
        this.hoveredRenderContent = renderContent;
        this.interactionAction = interactionAction;
    }

    /// <summary>
    /// Creates row with interaction and onHover stylization
    /// </summary>
    /// <param name="renderContent">Content rendered to console</param>
    /// <param name="hoveredRenderContent">Content rendered to console when hovered</param>
    /// <param name="interactionAction">Action on interaction</param>
    public ConsoleRow(IRenderable renderContent, IRenderable hoveredRenderContent, ConsoleRowAction interactionAction)
    {
        this.renderContent = renderContent;
        this.hoveredRenderContent = hoveredRenderContent;
        this.interactionAction = interactionAction;
    }

    /// <summary>
    /// Creates row
    /// </summary>
    /// <param name="renderContent">Content rendered to console</param>
    public ConsoleRow(IRenderable renderContent)
    {
        this.renderContent = renderContent;
        this.hoveredRenderContent = renderContent;
        interactionAction = null;
    }

    /// <summary>
    /// Creates row with onHover stylization
    /// </summary>
    /// <param name="renderContent">Content rendered to console</param>
    /// <param name="hoveredRenderContent">Content rendered to console when hovered</param>
    public ConsoleRow(IRenderable renderContent, IRenderable hoveredRenderContent)
    {
        this.renderContent = renderContent;
        this.hoveredRenderContent = hoveredRenderContent;
        interactionAction = null;
    }

    /// <summary>
    /// Gets content to render
    /// </summary>
    /// <param name="hovered">Is row hovered on right now</param>
    /// <returns>IRenderable to write to console</returns>
    public IRenderable GetRenderContent(bool hovered = false) => hovered ? hoveredRenderContent : renderContent;

    /// <summary>
    /// Method to ivoke when interacting with row
    /// </summary>
    public void OnInteraction() => interactionAction?.Invoke(this);
}

public delegate void ConsoleRowAction(ConsoleRow row);
