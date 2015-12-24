using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public abstract class StateBase : MonoBehaviour, IState
    {
        #region fields
        protected UIPanel RoadsPanel = null;
        #endregion


        #region ctor
        protected StateBase()
        {
            name = "State {0}".Format(State);
        }
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

        public virtual void Awake()
        {
            //"hack" to add the state-monobehaviours disabled
            // else the OnEnable is called once when GameObject.AddComponent<StateBase>
            Disable();
        }

        public void Enable()
        {
            enabled = true;
        }
        public void Disable()
        {
            enabled = false;
        }

        public virtual void OnEnable()
        {
            DebugLog.Info("OnEntry: State {0}", State);

            SetRoadsPanel();
        }

        public virtual void OnDisable()
        {
            DebugLog.Info("OnExit: State {0}", State);

            RoadsPanel = null;
        }

        public virtual void Update()
        {
            SetRoadsPanel();
        }

        public abstract Command? CheckCommand();
        public virtual void OnDestroy()
        {
            RoadsPanel = null;
        }
        #endregion
    }
}