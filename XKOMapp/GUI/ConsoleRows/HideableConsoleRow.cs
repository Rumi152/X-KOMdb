using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows
{
    public class HideableConsoleRow : BasicConsoleRow, IHideableConsoleRow
    {
        private bool isActive = true;
        bool ISwitchableConsoleRow.IsActive { get => isActive; set => isActive = value; }

        public HideableConsoleRow(IRenderable renderable) : base(renderable)
        {
        }

        void ISwitchableConsoleRow.OnTurningOff()
        {

        }

        void ISwitchableConsoleRow.OnTurningOn()
        {

        }
    }
}
