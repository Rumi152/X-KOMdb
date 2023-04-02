using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows;

public class BasicConsoleRow : IConsoleRow
{
    private readonly IRenderable renderContent;

    public BasicConsoleRow(IRenderable renderable) => this.renderContent = renderable;

    public IRenderable GetRenderContent() => renderContent;
}
