using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows
{
    public class HideableConsoleRow : BasicConsoleRow, IHideableConsoleRow
    {
        bool isActive = true;
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
