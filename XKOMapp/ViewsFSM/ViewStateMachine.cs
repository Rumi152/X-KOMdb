namespace XKOMapp.ViewsFSM
{
    public class ViewStateMachine
    {
        private ViewState? currentState;

        public void Checkout(string stateID)
        {
            currentState?.OnExit();
            currentState = states[stateID];
            currentState.OnEnter();
        }

        public void PassKeystroke(ConsoleKeyInfo info) => currentState?.PassKeystroke(info);

        public void AddState(string stateID, ViewState state) => states.Add(stateID, state);

        private readonly Dictionary<string, ViewState> states = new();
    }
}
