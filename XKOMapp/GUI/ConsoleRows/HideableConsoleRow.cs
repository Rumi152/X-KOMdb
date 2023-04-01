using Spectre.Console.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XKOMapp.GUI.ConsoleRows
{
    internal class HideableConsoleRow : IHideableConsoleRow
    {
        private IRenderable renderContent;
        bool isHidden;
        bool IHideableConsoleRow.IsHidden { get => isHidden; set => isHidden = value; }

        public HideableConsoleRow(IRenderable renderable) => this.renderContent = renderable;

        public IRenderable GetRenderContent() => renderContent;
        public void SetRenderContent(IRenderable renderContent) => this.renderContent = renderContent;

        void IHideableConsoleRow.OnHide()
        {

        }

        void IHideableConsoleRow.OnShow()
        {

        }
    }
}
