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

        protected ViewState(ViewStateMachine stateMachine)
        {
            this.fsm = stateMachine;
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }

        protected abstract void Display();
        public abstract void PassKeystroke(ConsoleKeyInfo info);
    }
}
