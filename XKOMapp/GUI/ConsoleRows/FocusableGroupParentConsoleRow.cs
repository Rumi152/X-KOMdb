using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows
{
    public class FocusableGroupParentConsoleRow : BasicConsoleRow, IFocusableConsoleRow, ICustomCursorConsoleRow, IInteractableConsoleRow
    {
        private readonly List<IFocusableConsoleRow> children;
        private bool IsFocused = false;
        bool ISwitchableConsoleRow.IsActive { get => IsFocused; set => IsFocused = value; }

        public FocusableGroupParentConsoleRow(IRenderable renderable, List<IFocusableConsoleRow> children) : base(renderable)
        {
            this.children = children;
        }

        void ISwitchableConsoleRow.OnTurningOff()
        {
            children.ForEach(children => children.TurnOff());
        }

        void ISwitchableConsoleRow.OnTurningOn()
        {
            children.ForEach(row => row.TurnOn());
        }

        public string GetCustomCursor()
        {
            return IsFocused ? "X" : ">";
        }

        public string GetCustomCursorBackground()
        {
            return IsFocused ? "\u25A0" : " ";
        }

        public void OnInteraction()
        {
            ((ISwitchableConsoleRow)this).Switch();
        }
    }
}
