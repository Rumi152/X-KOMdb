﻿using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.Models;

namespace XKOMapp.GUI.ConsoleRows.ProductSearching
{
    public class SearchContraintInputConsoleRow : ICustomKeystrokeListenerConsoleRow, ICustomCursorConsoleRow, IInteractableConsoleRow, IHoverConsoleRow
    {
        private readonly string markupPreText;
        private readonly ConsoleRowAction? onInteraction;
        private readonly ConsoleRowAction? onHoverEnd;
        private ConsolePrinter? owner;

        private bool isHovered = false;
        public string currentInput { get; private set; } = "";

        public SearchContraintInputConsoleRow(string markupPreText, ConsoleRowAction? onInteraction, ConsoleRowAction? onHoverEnd)
        {
            this.markupPreText = markupPreText;
            this.onInteraction = onInteraction;
            this.onHoverEnd = onHoverEnd;
        }

        public IRenderable GetRenderContent()
        {
            var end = (currentInput.Length < 32 && isHovered) ? "[blink]_[/]" : "";
            return new Markup(markupPreText + currentInput.EscapeMarkup() + end);
        }

        public void OnHoverEnd()
        {
            isHovered = false;
            onHoverEnd?.Invoke(this, owner);
        }

        public void OnHoverStart()
        {
            isHovered = true;
        }
        public void OnInteraction() => onInteraction?.Invoke(this, owner);

        public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            if (keystrokeInfo.Key == ConsoleKey.Backspace && currentInput.Length > 0)
                currentInput = currentInput[..^1];
            else if (currentInput.Length < 32 && char.IsLetterOrDigit(keystrokeInfo.KeyChar))
                currentInput += keystrokeInfo.KeyChar;
        }

        public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

        string ICustomCursorConsoleRow.GetCustomCursor() => "[blink]\u00BB[/]";
        string ICustomCursorConsoleRow.GetCustomCursorBackground() => " ";
    }
}
