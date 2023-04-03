using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows
{
    public class ModesConsoleRow : IModesConsoleRow, IXAxisInteractableConsoleRow, ICustomCursorConsoleRow, IInteractableConsoleRow
    {
        public struct ConsoleRowModeData
        {
            public IRenderable renderContent;
            public ConsoleRowAction action;
        }

        private readonly ConsoleRowModeData[] data;
        private readonly int modesCount;
        public int ModesCount => modesCount;

        private ConsolePrinter? owner;
        private int modeIndex = 0;
        int IModesConsoleRow.ModeIndex { get => modeIndex; set => modeIndex = value; }

        public ModesConsoleRow(params ConsoleRowModeData[] data)
        {
            modesCount = data.Length;
            this.data = data;
        }

        public IRenderable GetRenderContent() => data[modeIndex].renderContent;
        public void SetOwnership(ConsolePrinter owner) => this.owner = owner;

        void IModesConsoleRow.OnModeChange()
        {

        }

        public void MoveRight() => ((IModesConsoleRow)this).IncrementModeIndex();
        public void MoveLeft() => ((IModesConsoleRow)this).DecrementModeIndex();

        public string GetCustomCursor()
        {
            return "\u00BB";
        }

        public string GetCustomCursorBackground()
        {
            return " ";
        }

        public void OnInteraction() => data[modeIndex].action?.Invoke(this, owner);
    }
}
