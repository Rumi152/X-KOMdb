﻿using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows
{
    public class HoveredStylizationConsoleRow : IHoverConsoleRow
    {
        private IRenderable unhoveredRenderContent;
        private IRenderable hoveredRenderContent;
        private bool hovered = false;
        private ConsolePrinter? owner;

        public HoveredStylizationConsoleRow(IRenderable unhoveredRenderContent, IRenderable hoveredRenderContent)
        {
            this.unhoveredRenderContent = unhoveredRenderContent;
            this.hoveredRenderContent = hoveredRenderContent;
        }

        public IRenderable GetRenderContent()
        {
            return hovered ? hoveredRenderContent : unhoveredRenderContent;
        }

        public void OnHoverEnd()
        {
            hovered = false;
        }

        public void OnHoverStart()
        {
            hovered = true;
        }

        public void SetOwnership(ConsolePrinter owner)
        {
            this.owner = owner;
        }
    }
}
