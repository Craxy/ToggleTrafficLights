using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public abstract class StateBase : IState
    {
        #region fields
        protected UIPanel RoadsPanel = null;
        #endregion


        #region Helper
        protected void SetRoadsPanel()
        {
            if (RoadsPanel == null)
            {
                RoadsPanel = UIView.Find<UIPanel>("RoadsPanel");
            }
        }

        #endregion

        #region Implementation of IState

        public abstract State State { get; }

        public virtual void OnEntry()
        {
            DebugLog.Info("OnEntry: State {0}", State);

            SetRoadsPanel();
        }

        public virtual void OnExit()
        {
            DebugLog.Info("OnExit: State {0}", State);

            RoadsPanel = null;
        }

        public virtual void OnUpdate()
        {
            SetRoadsPanel();
        }

        public abstract Command? CheckCommand();

        #endregion
    }
}