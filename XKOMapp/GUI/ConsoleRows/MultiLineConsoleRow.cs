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
        private readonly Func<IConsoleRow, int> heightFunc;

        public MultiLineConsoleRow(IRenderable renderable, int height) : base(renderable)
        {
            heightFunc = (_) => height;
        }

        public MultiLineConsoleRow(IRenderable renderable, Func<IConsoleRow, int> height) : base(renderable)
        {
            heightFunc = height;
        }

        public int GetRenderHeight()
        {
            return heightFunc.Invoke(this);
        }
    }
}
