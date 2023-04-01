using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows;

public class InteractableConsoleRow : IInteractableConsoleRow
{
    private IRenderable renderContent;
    private readonly ConsoleRowAction interactionAction;
    private ConsolePrinter printer;

    public IRenderable GetRenderContent() => renderContent;
    public void OnInteraction(ConsolePrinter printer) => interactionAction?.Invoke(this, printer);


    /// <summary>
    /// Creates row with interaction
    /// </summary>
    /// <param name="renderContent">Content rendered to console</param>
    /// <param name="interactionAction">Action on interaction</param>
    public InteractableConsoleRow(IRenderable renderContent, ConsoleRowAction interactionAction)
    {
        this.renderContent = renderContent;
        this.interactionAction = interactionAction;
    }

    public void SetRenderContent(IRenderable renderContent) => this.renderContent = renderContent;
}
