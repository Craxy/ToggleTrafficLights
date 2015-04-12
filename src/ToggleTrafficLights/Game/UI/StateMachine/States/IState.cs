namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public interface IState
    {
        State State { get; }

        void OnEntry();
        void OnExit();

        void OnUpdate();
        Command? CheckCommand();

        void Destroy();
    }
}