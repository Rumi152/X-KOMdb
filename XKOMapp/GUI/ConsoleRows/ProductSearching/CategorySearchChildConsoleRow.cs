using Spectre.Console;
using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows.ProductSearching
{
    public class ChoiceMenuChildConsoleRow : IHideableConsoleRow, IFocusableConsoleRow, ICustomCursorConsoleRow, IStandardKeystrokeOverrideConsoleRow, ICustomKeystrokeListenerConsoleRow
    {
        public readonly string category;
        private ConsolePrinter owner = null!;
        private ChoiceMenuParentConsoleRow parent = null!;

        private bool isActive = false;
        bool ISwitchableConsoleRow.IsActive { get => isActive; set => isActive = value; }


        public ChoiceMenuChildConsoleRow(string category) => this.category = category;


        public IRenderable GetRenderContent() => new Text(category);
        public void SetOwnership(ConsolePrinter owner) => this.owner = owner;
        public void SetParent(ChoiceMenuParentConsoleRow parent) => this.parent = parent;


        void ISwitchableConsoleRow.OnTurningOff() { }
        void ISwitchableConsoleRow.OnTurningOn() { }

        public string GetCustomCursor() => "X";
        public string GetCustomCursorBackground() => "|";


        public bool ProcessStandardKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            parent.ProcessChildrenStandardKeystroke(keystrokeInfo);
            return false;
        }
        public void ProcessCustomKeystroke(ConsoleKeyInfo keystrokeInfo)
        {
            parent.ProcessChildrenCustomKeystroke(keystrokeInfo);
        }
    }
}
