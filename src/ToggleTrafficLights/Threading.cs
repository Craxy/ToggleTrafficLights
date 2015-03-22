using System;
using System.Reflection;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights
{
    public sealed class ThreadingExtension : ThreadingExtensionBase
    {
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            //TODO: is not mode dependent
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
            {
                DebugLog.Message("Enabling ToggleTrafficLightsTool");
                if(LoadingExtension.Instance == null)
                {
                    DebugLog.Message("LoadingExtension.Instance is null");
                    return;
                }

                LoadingExtension.Instance.EnableTool();
            }
        }
    }

    public sealed class LoadingExtension : LoadingExtensionBase
    {
        public static LoadingExtension Instance = null;


        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            if (Instance == null)
            {
                Instance = this;
            }

            DebugLog.Warning("Created v." + Assembly.GetExecutingAssembly().GetName().Version + " at " + DateTime.Now);
        }
        public override void OnReleased()
        {
            base.OnReleased();

            if (_tool != null)
            {
                Object.Destroy(_tool);
                _tool = null;
            }

            DebugLog.Warning("Released v." + Assembly.GetExecutingAssembly().GetName().Version);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            switch (mode)
            {
                case LoadMode.NewGame:
                case LoadMode.LoadGame:
                    OnLoaded();
                    DebugLog.Warning("Level loaded v." + Assembly.GetExecutingAssembly().GetName().Version);
                    break;
                default:
                    break;
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            DebugLog.Warning("Level unloaded v." + Assembly.GetExecutingAssembly().GetName().Version);
        }


        private ToggleTrafficLightsTool _tool = null;
        private void OnLoaded()
        {
            
        }

        public void EnableTool()
        {
            if (_tool == null)
            {
                CreateTool();
            }

            ToolsModifierControl.toolController.CurrentTool = _tool;
        }
        private void CreateTool()
        {
            if (_tool == null)
            {
                _tool = ToolsModifierControl.toolController.gameObject.GetComponent<ToggleTrafficLightsTool>() ??
                        ToolsModifierControl.toolController.gameObject.AddComponent<ToggleTrafficLightsTool>();

                DebugLog.Message("Tool created");
            }
            else
            {
                DebugLog.Message("Tool already created");
            }
        }
    }
}