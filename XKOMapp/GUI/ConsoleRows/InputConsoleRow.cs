using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows
{
    public class InputConsoleRow : IInputFieldConsoleRow
    {
        private string currentInput = "";
        public string CurrentInput { get => currentInput; set => currentInput = value; }

        private ConsolePrinter? owner;

        public IRenderable GetRenderContent()
        {
            return new Text(CurrentInput);
        }

        public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            var letter = keystrokeInfo.KeyChar;
            if (Char.IsLetterOrDigit(letter))
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
