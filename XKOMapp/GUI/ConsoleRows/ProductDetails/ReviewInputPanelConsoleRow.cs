using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows.ProductDetails
{
    internal class ReviewInputPanelConsoleRow : ICustomCursorConsoleRow, ICustomKeystrokeListenerConsoleRow, ICustomLineSpanConsoleRow
    {
        public int StarRating { get; private set; } = 0;
        public string Description { get; private set; } = "";

        private ConsolePrinter owner = null!;

        public string GetCustomCursor() => "\u00BB";
        public string GetCustomCursorBackground() => " ";

        public IRenderable GetRenderContent()
        {
            var stars = $"[yellow]{new string('*', StarRating)}[/][dim]{new string('*', 6 - StarRating)}[/]";

            int descriptionHeight;
            string panelText;
            if (Description.Length == 0)
            {
                descriptionHeight = 1;
                panelText = "[dim]Write something about product[/]";
            }
            else
            {
                descriptionHeight = (int)Math.Ceiling(Description.Length / (Console.WindowWidth - 10f));
                panelText = Description;
            }

            var panel = new Panel(panelText).HeavyBorder().Header($"| {stars} |");
            panel.Height = descriptionHeight + 2;
            panel.Width = 64;

            return panel;
        }

        public int GetRenderHeight()
        {
            if (Description.Length == 0)
                return 3;

            return (int)(Math.Ceiling(Description.Length / (Console.WindowWidth - 10f)) + 2);
        }

        public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            owner.SetBufferDirty();

            if (keystrokeInfo.Key == ConsoleKey.RightArrow)
            {
                if (StarRating < 6)
                    StarRating++;

                return;
            }

            if (keystrokeInfo.Key == ConsoleKey.LeftArrow)
            {
                if (StarRating > 1)
                    StarRating--;

                return;
            }

            if (char.IsLetterOrDigit(keystrokeInfo.KeyChar))
            {
                if (Description.Length < 256)
                    Description += keystrokeInfo.KeyChar;

                return;
            }

            if (new List<char>() { ' ', '@', ',', '.', ')', '(', '!', '#', '*', '&', '+', '-', '?'}.Contains(keystrokeInfo.KeyChar))
            {
                if (Description.Length < 256)
                    Description += keystrokeInfo.KeyChar;

                return;
            }

            if (keystrokeInfo.Key == ConsoleKey.Backspace)
            {
                if (Description.Length > 0)
                    Description = Description[..^1];

                return;
            }
        }

        public void SetOwnership(ConsolePrinter owner) => this.owner = owner;
    }
}
