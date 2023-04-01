using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows
{
    public class HoveredStylizationConsoleRow : IHoverConsoleRow
    {
        private IRenderable unhoveredRenderContent;
        private IRenderable hoveredRenderContent;
        private bool hovered = false;

        public HoveredStylizationConsoleRow(IRenderable unhoveredRenderContent, IRenderable hoveredRenderContent)
        {
            this.unhoveredRenderContent = unhoveredRenderContent;
            this.hoveredRenderContent = hoveredRenderContent;
        }

        public IRenderable GetRenderContent()
        {
            return  hovered ? hoveredRenderContent : unhoveredRenderContent;
        }

        public void OnHoverEnd()
        {
            hovered = false;
        }

        public void OnHoverStart()
        {
            hovered = true;
        }
        public void SetHoveredRenderContent(IRenderable renderContent) => this.hoveredRenderContent = renderContent;
        public void SetUnhoveredRenderContent(IRenderable renderContent) => this.unhoveredRenderContent = renderContent;
    }
}
