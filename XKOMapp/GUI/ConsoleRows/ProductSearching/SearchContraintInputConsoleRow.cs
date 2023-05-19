using Spectre.Console;
using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows.ProductSearching
{
    public class SearchContraintInputConsoleRow : ICustomKeystrokeListenerConsoleRow, ICustomCursorConsoleRow, IInteractableConsoleRow, IHoverConsoleRow
    {
        private readonly string markupPreText;
        private readonly int inputMaxLength;
        private readonly Action onInteraction;
        private readonly Action onHoverEnd;
        private ConsolePrinter owner = null!;

        private bool isHovered = false;
        public string currentInput { get; private set; } = "";

        public SearchContraintInputConsoleRow(string markupPreText, int inputMaxLength, Action onInteraction, Action onHoverEnd)
        {
            this.markupPreText = markupPreText;
            this.inputMaxLength = inputMaxLength;
            this.onInteraction = onInteraction;
            this.onHoverEnd = onHoverEnd;
        }

        public IRenderable GetRenderContent()
        {
            string end = isHovered ? "[blink]_[/]" : "";
            return new Markup(markupPreText + currentInput.EscapeMarkup() + end);
        }

        public void OnHoverEnd()
        {
            isHovered = false;
            onHoverEnd();
        }

        public void OnHoverStart()
        {
            isHovered = true;
        }
        public void OnInteraction() => onInteraction();

        public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            owner.SetBufferDirty();

            if (keystrokeInfo.Key == ConsoleKey.Backspace && currentInput.Length > 0)
                currentInput = currentInput[..^1];
            else if (currentInput.Length < inputMaxLength && (char.IsLetterOrDigit(keystrokeInfo.KeyChar) || keystrokeInfo.Key == ConsoleKey.Spacebar))
                currentInput += keystrokeInfo.KeyChar;
        }

        public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

        string ICustomCursorConsoleRow.GetCustomCursor() => "[blink]\u00BB[/]";
        string ICustomCursorConsoleRow.GetCustomCursorBackground() => " ";

        public void ResetInput()
        {
            currentInput = "";
            owner.SetBufferDirty();
        }
    }
}
