namespace XKOMapp.ViewsFSM
{
    public class ViewStateMachine
    {
        private readonly Dictionary<string, ViewState> states = new();
        private readonly Stack<ViewState> history = new Stack<ViewState>();
        private ViewState? currentState;

        public void PassKeystroke(ConsoleKeyInfo info) => currentState?.PassKeystroke(info);

        public void SaveState(string stateID, ViewState state) => states.Add(stateID, state);


        public void RollbackOrDefault(ViewState defaultState)
        {
            if (history.TryPop(out var result))
                Checkout(result);
            else
                Checkout(defaultState);
        }

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
            if (currentState is not null)
            {
                currentState.OnExit();
                history.Push(currentState);
            }

            currentState = newState;
            currentState.OnEnter();
        }
    }
}
