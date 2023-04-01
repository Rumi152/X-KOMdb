using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows;

public class BasicConsoleRow : IConsoleRow
{
    private IRenderable renderContent;

    public BasicConsoleRow(IRenderable renderable) => this.renderContent = renderable;

    public IRenderable GetRenderContent() => renderContent;
    public void SetRenderContent(IRenderable renderContent) => this.renderContent = renderContent;
}
