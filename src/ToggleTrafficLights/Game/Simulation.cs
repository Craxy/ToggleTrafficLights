using System;
using System.Diagnostics;
using System.Reflection;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
    //Order of Events in Unity: https://i.imgur.com/NJage5W.png

    //C:S does not work with a class implementing two Interfaces at once:
    // it creates for each Interface one instance
    // therefore ILoadingExtension AND IThreadingExtension can not live together in the same instance

    public sealed class Loading : LoadingExtensionBase
    {
        #region Overrides of LoadingExtensionBase

        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            Simulation.OnCreated(loading);
        }

        public override void OnReleased()
        {
            base.OnReleased();

            Simulation.OnReleased();
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            Simulation.OnLevelLoaded(mode);
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            Simulation.OnLevelUnloading();
        }

        #endregion

        public Simulation Simulation
        {
            get { return SimulationInstance.Simulation; }
        }
    }

    public sealed class Threading : ThreadingExtensionBase
    {
        public Simulation Simulation
        {
            get { return SimulationInstance.Simulation; }
        }

        #region Overrides of ThreadingExtensionBase

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            Simulation.OnUpdate(realTimeDelta, simulationTimeDelta);

//#if DEBUG
//            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.P))
//            {
////                MyHighlightTestTool.ToggleActivated();
//                HighlightIntersectionsUi.ToggleShow();
//            }
//#endif
        }

        #endregion
    }

    // I don't want to add an static Instance property to the Simulation class...
    public static class SimulationInstance
    {
        public static readonly Simulation Simulation = new Simulation();
    }

    public sealed class Simulation
    {
        #region members
        private TrafficLightsMachine _stateMachine = null;
        #endregion

        #region properties
        public ILoading LoadingManager { get; private set; }
        public IManagers Managers
        {
            get
            {
                return LoadingManager.managers;
            }
        }

        #endregion

        #region Implementation of IThreadingExtension

        public void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            if (IsLoading() || !IsGameMode())
            {
                return;
            }

#if DEBUG
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.O))
            {
                DebugLog.Info("Current State: {0}", _stateMachine.CurrentState);
                DebugLog.Info("Current tool: {0}", ToolsModifierControl.toolController.CurrentTool);
                //DebugLog.Info("States:");
                //foreach (var s in _stateMachine.States)
                //{
                //    DebugLog.Info("      {0} -- enabled={1}", s.State, s.enabled);
                //}
            }
#endif

            //state machine is update automatically because monobehaviour
            //_stateMachine.OnUpdate();
        }

        #endregion

        #region Overrides of LoadingExtensionBase

        public void OnCreated(ILoading loading)
        {
            LoadingManager = loading;

            DebugLog.Message("Created v.{0} at {1}", Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now);
        }

        public void OnReleased()
        {
            if (_stateMachine != null)
            {
                _stateMachine.enabled = false;
                Object.Destroy(_stateMachine.gameObject);
            }
            _stateMachine = null;
                        
            DebugLog.Message("Released v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        public void OnLevelLoaded(LoadMode mode)
        {

            if (IsGameMode())
            {
                var go = new GameObject("StateMachineGameObject");
                _stateMachine = go.AddComponent<TrafficLightsMachine>();
                _stateMachine.enabled = true;
                DebugLog.Message("Level loaded");
            }
            else
            {
                DebugLog.Message("In Editor -> mod is disabled");
            }
        }

        public void OnLevelUnloading()
        {
            if (_stateMachine != null)
            {
                _stateMachine.enabled = false;
                Object.Destroy(_stateMachine.gameObject);
            }
            _stateMachine = null;
            DebugLog.Message("Level unloaded");
        }

        #endregion

        #region helpers
        private bool IsGameMode()
        {
            if (LoadingManager != null)
            {
                return LoadingManager.IsGameMode();
            }
            //don't know -> go on
            DebugLog.Warning("IsGameMode: unknown -- default to true");
            var st = new StackTrace();
            DebugLog.Warning(st.ToString());
            return true;
        }

        private bool IsLoading()
        {
            return _stateMachine == null;
        }
        #endregion

    }
}