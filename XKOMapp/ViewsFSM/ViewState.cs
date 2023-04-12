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
        protected ConsolePrinter? printer = null;
        protected ViewStateMachine stateMachine { get; private set; }

        protected ViewState(ViewStateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }

        public virtual void Print() { }
        public virtual void PassKeystroke(ConsoleKeyInfo info) { }
    }
}
