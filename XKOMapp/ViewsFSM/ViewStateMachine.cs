namespace XKOMapp.ViewsFSM
{
    public class ViewStateMachine
    {
        private readonly Dictionary<string, ViewState> states = new();
        private readonly Stack<ViewState> history = new Stack<ViewState>();
        public ViewState? CurrentState { get; private set; } = null;

        public void PassKeystroke(ConsoleKeyInfo info) => CurrentState?.PassKeystroke(info);
        public void Tick() => CurrentState?.Tick();

        public void SaveState(string stateID, ViewState state) => states.Add(stateID, state);


        public void RollbackOrDefault(ViewState defaultState)
        {
            if (history.TryPop(out var result))
                Checkout(result);
            else
                Checkout(defaultState);
        }

        [Obsolete("This method may produce unexpected result and should replaced by Checkout with previously saved ViewState variable", false)]
        public void RollbackOrDefault(string defaultStateID)
        {
            RollbackOrDefault(states[defaultStateID]);
        }


        public void Checkout(string stateID)
        {
            Checkout(states[stateID]);
        }

        public void Checkout(ViewState newState)
        {
            if (CurrentState is not null)
            {
                CurrentState.OnExit();
                history.Push(CurrentState);
            }

            CurrentState = newState;
            CurrentState.OnEnter();
        }
    }
}
