using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows
{
    public class HideOnClickConsoleRow : IInteractableConsoleRow, IHideableConsoleRow
    {
        private IRenderable renderContent;
        private readonly ConsoleRowAction interactionAction;
        bool isHidden;
        bool IHideableConsoleRow.IsHidden { get => isHidden; set => isHidden = value; }

        public IRenderable GetRenderContent() => renderContent;
        public void SetRenderContent(IRenderable renderContent) => this.renderContent = renderContent;

        public void OnInteraction(ConsolePrinter printer) => interactionAction?.Invoke(this, printer);

        public HideOnClickConsoleRow(IRenderable renderContent)
        {
            this.renderContent = renderContent;
            this.interactionAction = (row, printer) =>
            {
                ((IHideableConsoleRow)this).Hide();
                printer.ReloadBuffer();
                printer.PrintBuffer();
            };
        }

        void IHideableConsoleRow.OnHide()
        {

        }

        void IHideableConsoleRow.OnShow()
        {

        }
    }
}
