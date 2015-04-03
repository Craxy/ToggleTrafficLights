using System.Collections.Generic;
using System.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine
{
    public sealed class TrafficLightsMachine : StateMachine
    {
        #region fields

        private bool _firstUpdate = true;
        #endregion

        public TrafficLightsMachine()
            : base(initialState: State.Hidden)
        {
            Transitions = new List<Transition>
            {
                new Transition(State.Hidden, Command.DisplayRoadsPanel, State.Deactivated),
                new Transition(State.Hidden, Command.PressShortcut, State.HiddenToActivated),
                new Transition(State.HiddenToActivated, Command.DisplayRoadsPanel, State.Activated),
                new Transition(State.Deactivated, Command.HideRoadsPanel, State.Hidden),
                new Transition(State.Deactivated, Command.PressShortcut, State.Activated),
                new Transition(State.Deactivated, Command.ClickToolButton, State.Activated),
                new Transition(State.Activated, Command.PressShortcut, State.Hidden),
                new Transition(State.Activated, Command.HideRoadsPanel, State.Hidden),
                new Transition(State.Activated, Command.ClickToolModeTab, State.Deactivated),
                new Transition(State.Activated, Command.ActivateOtherTool, State.Deactivated),
            };
            States = new List<IState>
            {
                new HiddenState(),
                new Deactivated(),
                new ActivatedState(),
                new HiddenToActivatedState(),
            };
        }

        public IList<IState> States { get; private set; }
        public IState GetCurrentState()
        {
            return States.Single(s => s.State == CurrentState);
        }

        public void OnUpdate()
        {
            var state = GetCurrentState();

            if (_firstUpdate)
            {
                DebugLog.Info("Initial state: {0}", CurrentState);
                state.OnEntry();
                _firstUpdate = false;
            }

//            Command? command = null;
//            while ((command = state.CheckCommand()).HasValue)
//            {
//                //move to next state
//                var transition = MoveNext(command.Value);
//
//                DebugLog.Info("Transition: {0}", transition);
//
//                //revert old state
//                state.OnExit();
//
//                state = GetCurrentState();
//                //activate new state
//                state.OnEntry();
//            }

            var command = state.CheckCommand();
            if (command.HasValue)
            {
                //move to next state
                var transition = MoveNext(command.Value);
                
                DebugLog.Info("Transition: {0}", transition);
                
                //revert old state
                state.OnExit();
                
                state = GetCurrentState();
                //activate new state
                state.OnEntry();
            }
            else
            {
                state.OnUpdate();
            }
        }

        public void Destroy()
        {
            if (!_firstUpdate)
            {
                var state = GetCurrentState();
                state.OnExit();
            }
        }
    }
}