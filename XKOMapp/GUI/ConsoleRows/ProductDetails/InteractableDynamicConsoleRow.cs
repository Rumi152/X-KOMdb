using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows.ProductDetails
{
    internal class InteractableDynamicConsoleRow : IInteractableConsoleRow, ICustomCursorConsoleRow
    {
        private string markupText;
        private ConsolePrinter owner = null!;

        private readonly ConsoleRowAction onInteraction;

        public InteractableDynamicConsoleRow(string markup, ConsoleRowAction onInteraction)
        {
            markupText = markup;
            this.onInteraction = onInteraction;
        }

        public IRenderable GetRenderContent() => new Markup(markupText);
        public void SetMarkupText(string markup) => markupText = markup;

        public void OnInteraction() => onInteraction(this, owner);
        public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

        public string GetCustomCursor() => "\u00BB";
        public string GetCustomCursorBackground() => " ";
    }
}
