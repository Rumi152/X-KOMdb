using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows.User
{
    internal class PasswordInputConsoleRow : ICustomCursorConsoleRow, ICustomKeystrokeListenerConsoleRow, IHoverConsoleRow, IInteractableConsoleRow
    {
        private readonly string markupLabel;
        private readonly int maxLength;
        ConsolePrinter owner = null!;

        public string CurrentInput { get; private set; } = "";
        private bool isHovered;

        public PasswordInputConsoleRow(string markupLabel, int maxLength)
        {
            this.markupLabel = markupLabel;
            this.maxLength = maxLength;
        }

        public IRenderable GetRenderContent() => new Markup($"{markupLabel}{new string('*', CurrentInput.Length)}{(isHovered ? "[blink]_[/]" : "")}");
        public void SetOwnership(ConsolePrinter owner) => this.owner = owner;


        public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            if(keystrokeInfo.Key == ConsoleKey.Backspace)
            {
                if (CurrentInput.Length > 0)
                    CurrentInput = CurrentInput[..^1];
                return;
            }

            if (CurrentInput.Length >= maxLength)
                return;

            if (char.IsWhiteSpace(keystrokeInfo.KeyChar))
                return;

            if (!char.IsAscii(keystrokeInfo.KeyChar))
                return;

            CurrentInput += keystrokeInfo.KeyChar;
        }


        public string GetCustomCursor() => "\u00BB";
        public string GetCustomCursorBackground() => " ";

        public void OnHoverStart() => isHovered = true;
        public void OnHoverEnd() => isHovered = false;

        public void OnInteraction() => owner.CursorDown();
    }
}
