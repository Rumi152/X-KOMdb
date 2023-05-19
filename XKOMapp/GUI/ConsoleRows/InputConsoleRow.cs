using Spectre.Console;
using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows
{
    public class InputConsoleRow : ICustomKeystrokeListenerConsoleRow
    {
        public string CurrentInput { get; private set; } = "";

        private ConsolePrinter? owner;

        public IRenderable GetRenderContent()
        {
            return new Text(CurrentInput);
        }

        public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            char letter = keystrokeInfo.KeyChar;
            if (char.IsLetterOrDigit(letter))
                CurrentInput += letter;

            if (keystrokeInfo.Key == ConsoleKey.Backspace && CurrentInput.Length > 0)
                CurrentInput = CurrentInput.Remove(CurrentInput.Length - 1);
        }

        public void SetOwnership(ConsolePrinter owner)
        {
            this.owner = owner;
        }
    }
}
