namespace XKOMapp.ViewsFSM
{
    public class ViewStateMachine
    {
        private ViewState? currentState;

        public void Checkout(ViewState newState)
        {
            currentState?.OnExit();
            currentState = newState;
            currentState?.OnEnter();
        }
    }
}
