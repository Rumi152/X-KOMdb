using XKOMapp.GUI;

namespace XKOMapp.ViewsFSM
{
    public abstract class ViewState
    {
        protected readonly ConsolePrinter printer = new();
        protected readonly ViewStateMachine fsm;
        protected bool IsActiveState { get; private set; }

        private bool printerBuilt = false;

        protected ViewState(ViewStateMachine stateMachine)
        {
            fsm = stateMachine;
        }

        protected abstract void InitialPrinterBuild(ConsolePrinter printer);

        public virtual void OnEnter()
        {
            IsActiveState = true;

            if (!printerBuilt)
            {
                InitialPrinterBuild(printer);
                printerBuilt = true;
            }

            printer.OnBufferReload += Display;
            printer.SetBufferDirty();
        }
        public virtual void OnExit()
        {
            printer.OnBufferReload -= Display;
            IsActiveState = false;
        }

        protected void Display()
        {
            ConsolePrinter.ClearScreen();
            printer.PrintBuffer();
        }

        public virtual void Tick() => printer.Tick();
        public virtual void PassKeystroke(ConsoleKeyInfo info) => printer.PassKeystroke(info);
    }
}
