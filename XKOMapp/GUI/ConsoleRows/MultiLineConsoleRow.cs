using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows
{
    internal class MultiLineConsoleRow : BasicConsoleRow, ICustomLineSpanConsoleRow
    {
        private int height;

        public MultiLineConsoleRow(IRenderable renderable, int height) : base(renderable)
        {
            this.height = height;
        }

        public int GetRenderHeight()
        {
            return height;
        }
    }
}
