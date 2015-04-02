using System.Collections.Generic;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine
{
    public class TrafficLightsMachine : StateMachine
    {
        public TrafficLightsMachine()
        {
            Transitions = new List<Transition>()
            {
                new Transition(State.Hidden, Command.DisplayRoadsPanel, State.Deactivated),
                new Transition(State.Hidden, Command.PressShortcut, State.Activated),
                new Transition(State.Deactivated, Command.HideRoadsPanel, State.Hidden),
                new Transition(State.Deactivated, Command.PressShortcut, State.Activated),
                new Transition(State.Deactivated, Command.ClickToolButton, State.Activated),
                new Transition(State.Activated, Command.PressShortcut, State.Hidden),
                new Transition(State.Activated, Command.HideRoadsPanel, State.Hidden),
                new Transition(State.Activated, Command.ClickToolModeTab, State.Deactivated),
                new Transition(State.Activated, Command.ActivateOtherTool, State.Deactivated),
            };
        }
        public static readonly Dictionary<State, IState> States = new Dictionary<State, IState>();




        public void OnUpdate()
        {
            IState state = null;
            
            state.OnUpdate();

            var command = state.CheckCommand();
            if (command.HasValue)
            {
                //move to next state
                var transition = MoveNext(command.Value);

                state.OnExit(transition);
                state.OnEntry(transition);
            }
        }
    }
}