using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows
{
    public class FocusableConsoleRow : BasicConsoleRow, IFocusableConsoleRow, ICustomCursorConsoleRow
    {
        public FocusableConsoleRow(IRenderable renderable) : base(renderable)
        {
        }

        private bool isFocused = false;
        bool ISwitchableConsoleRow.IsActive { get => isFocused; set => isFocused = value; }

        void ISwitchableConsoleRow.OnTurningOff()
        {

        }

        void ISwitchableConsoleRow.OnTurningOn()
        {

        }

        public string GetCustomCursor()
        {
            return isFocused ? "X" : ">";
        }

        public string GetCustomCursorBackground()
        {
            return isFocused ? "|" : " ";
        }
    }
}
