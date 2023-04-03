using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows;

public class BasicConsoleRow : IConsoleRow
{
    protected readonly IRenderable renderContent;
    protected ConsolePrinter? owner;

    public BasicConsoleRow(IRenderable renderable) => this.renderContent = renderable;

    public IRenderable GetRenderContent() => renderContent;

    public void SetOwnership(ConsolePrinter owner)
    {
        this.owner = owner;
    }
}
