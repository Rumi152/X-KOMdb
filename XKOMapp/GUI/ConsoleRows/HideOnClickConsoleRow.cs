using Spectre.Console.Rendering;

namespace XKOMapp.GUI.ConsoleRows
{
    public class HideOnClickConsoleRow : IInteractableConsoleRow, IHideableConsoleRow
    {
        private IRenderable renderContent;
        private readonly ConsoleRowAction interactionAction;
        bool isHidden = true;
        bool ISwitchableConsoleRow.IsActive { get => isHidden; set => isHidden = value; }

        public IRenderable GetRenderContent() => renderContent;
        public void SetRenderContent(IRenderable renderContent) => this.renderContent = renderContent;

        public void OnInteraction(ConsolePrinter printer) => interactionAction?.Invoke(this, printer);

        public HideOnClickConsoleRow(IRenderable renderContent)
        {
            this.renderContent = renderContent;
            this.interactionAction = (row, printer) =>
            {
                ((IHideableConsoleRow)this).TurnOff();
                printer.ReloadBuffer();
                printer.PrintBuffer();
            };
        }

        void ISwitchableConsoleRow.OnTurningOff()
        {

        }

        void ISwitchableConsoleRow.OnTurningOn()
        {
            throw new NotImplementedException();
        }
    }
}
