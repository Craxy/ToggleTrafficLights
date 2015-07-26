namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public interface IState
    {
        State State { get; }

        void Enable();
        void Disable();

        void OnEnable();
        void OnDisable();

        void Update();
        Command? CheckCommand();

        void OnDestroy();
    }
}