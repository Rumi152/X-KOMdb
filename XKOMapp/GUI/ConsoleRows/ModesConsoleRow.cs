using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows
{
    public class ModesConsoleRow : IModesConsoleRow, ICustomKeystrokeListenerConsoleRow, ICustomCursorConsoleRow
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

        public string GetCustomCursor()
        {
            return "\u00BB";
        }

        public string GetCustomCursorBackground()
        {
            return " ";
        }

        public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            switch (keystrokeInfo.Key)
            {
                case ConsoleKey.RightArrow:
                    ((IModesConsoleRow)this).IncrementModeIndex();
                    break;
                case ConsoleKey.LeftArrow:
                    ((IModesConsoleRow)this).DecrementModeIndex();
                    break;
            }
        }
    }
}
