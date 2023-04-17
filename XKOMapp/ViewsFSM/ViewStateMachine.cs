namespace XKOMapp.ViewsFSM
{
    public class ViewStateMachine
    {
        private readonly Dictionary<string, ViewState> states = new();
        private ViewState? currentState;

        public void Checkout(string stateID)
        {
            Checkout(states[stateID]);
        }

        public void Checkout(ViewState newState)
        {
            currentState?.OnExit();
            currentState = newState;
            currentState.OnEnter();
        }


        public void PassKeystroke(ConsoleKeyInfo info) => currentState?.PassKeystroke(info);

        public void SaveState(string stateID, ViewState state) => states.Add(stateID, state);
    }
}
