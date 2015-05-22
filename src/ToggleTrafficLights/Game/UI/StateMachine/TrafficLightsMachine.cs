using System.Collections.Generic;
using System.Linq;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools.Visualization;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine
{
    public sealed class TrafficLightsMachine : StateMachine
    {
        #region fields

        private bool _firstUpdate = true;
        private IntersectionHighlighting _intersectionHighlighting = null;
        #endregion

        public TrafficLightsMachine()
            : base(initialState: State.Hidden)
        {
            Transitions = new List<Transition>
            {
                new Transition(State.Hidden, Command.DisplayRoadsPanel, State.Deactivated),
                new Transition(State.Hidden, Command.PressShortcut, State.HiddenToActivated),
                new Transition(State.Hidden, Command.PressInvisibleShortcut, State.HiddenActivatedState),

                new Transition(State.HiddenToActivated, Command.DisplayRoadsPanel, State.Activated),
                
                new Transition(State.Deactivated, Command.HideRoadsPanel, State.Hidden),
                new Transition(State.Deactivated, Command.LeftClickOnToolButton, State.Activated),
                new Transition(State.Deactivated, Command.RightClickToolButton, State.ActivatedUiState),
                new Transition(State.Deactivated, Command.PressShortcut, State.Activated),
                new Transition(State.Deactivated, Command.PressInvisibleShortcut, State.Activated),
                

                new Transition(State.Activated, Command.HideRoadsPanel, State.Hidden),
                new Transition(State.Activated, Command.RightClickToolButton, State.ActivatedUiState),
                new Transition(State.Activated, Command.ClickToolModeTab, State.Deactivated),
                new Transition(State.Activated, Command.ActivateOtherTool, State.Deactivated),
                new Transition(State.Activated, Command.PressShortcut, State.ActivatedToHidden),
                new Transition(State.Activated, Command.PressInvisibleShortcut, State.Deactivated),
                
                new Transition(State.ActivatedToHidden, Command.HideRoadsPanel, State.Hidden),

                new Transition(State.HiddenActivatedState, Command.PressInvisibleShortcut, State.Hidden),
                new Transition(State.HiddenActivatedState, Command.PressShortcut, State.Hidden),
                new Transition(State.HiddenActivatedState, Command.ActivateOtherTool, State.Hidden),

                new Transition(State.ActivatedUiState, Command.HideRoadsPanel, State.Hidden),
                new Transition(State.ActivatedUiState, Command.LeftClickOnToolButton, State.Activated),
                new Transition(State.ActivatedUiState, Command.ClickToolModeTab, State.Deactivated),
                new Transition(State.ActivatedUiState, Command.ActivateOtherTool, State.Deactivated),
                new Transition(State.ActivatedUiState, Command.PressShortcut, State.ActivatedToHidden),
                new Transition(State.ActivatedUiState, Command.PressInvisibleShortcut, State.Deactivated),
            };
            _intersectionHighlighting = new IntersectionHighlighting();
            States = new List<IState>
            {
                new HiddenState(),
                new Deactivated(),
                new ActivatedState(_intersectionHighlighting),
                new HiddenToActivatedState(),
                new ActivatedToHiddenState(),
                new HiddenActivatedState(),
                new ActivatedUiState(_intersectionHighlighting),
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

            foreach (var s in States)
            {
                s.Destroy();
            }

            if (_intersectionHighlighting != null && _intersectionHighlighting.Enabled)
            {
                _intersectionHighlighting.Deactivate();
            }
            _intersectionHighlighting = null;
        }
    }
}