using Spectre.Console.Rendering;

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
