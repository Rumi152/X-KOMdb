using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI;

namespace XKOMapp.ViewsFSM
{
    public abstract class ViewState
    {
        protected ConsolePrinter printer = null!;
        protected ViewStateMachine fsm { get; private set; }
        protected bool isActiveState { get; private set; }

        protected ViewState(ViewStateMachine stateMachine)
        {
            this.fsm = stateMachine;
        }

        public virtual void OnEnter() => isActiveState = true;
        public virtual void OnExit() => isActiveState = false;

        protected virtual void Display()
        {
            ConsolePrinter.ClearScreen();
            printer.PrintMemory();
        }

        protected virtual void OnKeystrokePassed(ConsoleKeyInfo info) { }
        protected virtual void OnKeystrokePassedFinally(ConsoleKeyInfo info) { }

        public void PassKeystroke(ConsoleKeyInfo info)
        {
            OnKeystrokePassed(info);
            if (isActiveState)
                OnKeystrokePassedFinally(info);
        }
    }
}
