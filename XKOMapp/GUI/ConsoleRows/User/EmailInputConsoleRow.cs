using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows.User
{
    internal class EmailInputConsoleRow : ICustomCursorConsoleRow, ICustomKeystrokeListenerConsoleRow, IHoverConsoleRow, IInteractableConsoleRow
    {
        private readonly string markupLabel;
        private readonly int maxLength;
        ConsolePrinter owner = null!;

        public string CurrentInput { get; private set; } = "";
        private bool isHovered;

        public EmailInputConsoleRow(string markupLabel, int maxLength)
        {
            this.markupLabel = markupLabel;
            this.maxLength = maxLength;
        }

        public IRenderable GetRenderContent() => new Markup($"{markupLabel}{CurrentInput.Substring(Math.Max(CurrentInput.Length - 64, 0)).EscapeMarkup()}{(isHovered ? "[blink]_[/]" : "")}");
        public void SetOwnership(ConsolePrinter owner) => this.owner = owner;


        public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            owner.SetBufferDirty();

            if (keystrokeInfo.Key == ConsoleKey.Backspace)
            {
                if (CurrentInput.Length > 0)
                    CurrentInput = CurrentInput[..^1];
                return;
            }

            if (CurrentInput.Length >= maxLength)
                return;

            if (keystrokeInfo.KeyChar == '@')
            {
                if (!CurrentInput.Contains('@'))
                    CurrentInput += keystrokeInfo.KeyChar;

                return;
            }

            if (char.IsLetterOrDigit(keystrokeInfo.KeyChar))
            {
                CurrentInput += keystrokeInfo.KeyChar;
                return;
            }

            if (new List<char>() { '_', '.', '-', '%', '+' }.Contains(keystrokeInfo.KeyChar))
            {
                CurrentInput += keystrokeInfo.KeyChar;
                return;
            }
        }


        public string GetCustomCursor() => "[blink]\u00BB[/]";
        public string GetCustomCursorBackground() => " ";

        public void OnHoverStart() => isHovered = true;
        public void OnHoverEnd() => isHovered = false;

        public void OnInteraction() => owner.CursorDown();
    }
}
