using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows.ProductDetails
{
    internal class DynamicRenderableConsoleRow : IConsoleRow
    {
        private readonly Func<IRenderable> getRenderable;
        private ConsolePrinter owner = null!;

        public DynamicRenderableConsoleRow(Func<IRenderable> getRenderable) => this.getRenderable = getRenderable;

        public IRenderable GetRenderContent() => getRenderable();

        public void SetOwnership(ConsolePrinter owner) => this.owner = owner;
    }
}
