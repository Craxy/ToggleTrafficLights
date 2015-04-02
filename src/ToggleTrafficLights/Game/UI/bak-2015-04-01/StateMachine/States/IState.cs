namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public interface IState
    {
        State State { get; }

        void OnEntry(Transition transition);
        void OnExit(Transition transition);

        void OnUpdate();
        Command? CheckCommand();
    }
}