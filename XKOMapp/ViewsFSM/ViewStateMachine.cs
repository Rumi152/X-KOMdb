namespace XKOMapp.ViewsFSM
{
    public class ViewStateMachine
    {
        public ViewState? CurrentState { get; private set; }

        public void Checkout(string stateID)
        {
            CurrentState?.OnExit();
            CurrentState = states[stateID];
            CurrentState.OnEnter();
        }

        public void AddState(string stateID, ViewState state) => states.Add(stateID, state);

        private readonly Dictionary<string, ViewState> states = new();
    }
}
