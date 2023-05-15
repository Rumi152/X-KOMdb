using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XKOMapp.GUI;
using XKOMapp.Models;

namespace XKOMapp.ViewsFSM
{
    public abstract class ViewState
    {
        protected ConsolePrinter? printer = null;
        protected ViewStateMachine fsm { get; private set; }
        protected bool isActiveState { get; private set; }

        protected ViewState(ViewStateMachine stateMachine)
        {
            this.fsm = stateMachine;
        }

        public virtual void OnEnter()
        {
            if (printer is not null)
            printer.OnBufferReload += Display;
            isActiveState = true;
            printer?.SetBufferDirty();
        }

        public virtual void OnExit()
        {
            if (printer is not null)
            printer.OnBufferReload -= Display;
            isActiveState = false;
        }

        protected virtual void Display()
        {
            ConsolePrinter.ClearScreen();
            printer?.PrintBuffer();
        }

        public virtual void Tick()
        {
            printer?.Tick();
        }

        public virtual void PassKeystroke(ConsoleKeyInfo info)
        {
            printer?.PassKeystroke(info);
        }
    }
}
