using System;
using System.Threading;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using ICities;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
    public class Simulation : ThreadingExtensionBase
    {
        #region members

        private ToolBase _previousTool = null;
        #endregion

        public override void OnCreated(IThreading threading)
        {
            base.OnCreated(threading);
        }

        public override void OnReleased()
        {
            base.OnReleased();
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            if (managers.loading.IsGameMode() && Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.T))
            {
                ToggleToggleTrafficLightsTool();
            }
        }

        private void ToggleToggleTrafficLightsTool()
        {
            var controller = ToolsModifierControl.toolController;

            var currentTool = controller.CurrentTool;
            var ttltool = GetTrafficLightsTool<ToggleTrafficLightsTool>();

            if (currentTool == ttltool)
            {
                //switch to previous tool
                if (_previousTool == null)
                {
                    //fallback: back to defaulttool (selection tool)
                    ToolsModifierControl.SetTool<DefaultTool>();
                }
                else
                {
                    controller.CurrentTool = _previousTool;
                }

                DebugLog.Message("Switched to previous tool: {0}", _previousTool == null ? "[DefaultTool]" : _previousTool.GetType().Name);

                _previousTool = null;
                System.Diagnostics.Debug.Assert(_previousTool == null);

            }
            else
            {
                //switch to traffic lights tool
                _previousTool = currentTool;
                controller.CurrentTool = ttltool;

                DebugLog.Message("Switched to ToggleTrafficLightsTool from {0}", _previousTool == null ? "[DefaultTool]" : _previousTool.GetType().Name);

                System.Diagnostics.Debug.Assert(_previousTool != null);
            }
        }


        private ToolBase GetTrafficLightsTool<T>() where T : ToolBase 
        {
            T tool = null;
            try
            {
                tool = ToolsModifierControl.toolController.gameObject.GetComponent<T>();
            }
            catch (Exception e)
            {
                Log.Warning("GetComponent null: {0}", e.ToString());
            }
            if (tool == null)
            {
                tool = ToolsModifierControl.toolController.gameObject.AddComponent<T>();
                DebugLog.Message("Simulation: ToggleTrafficLightsTool created");
            }

            if (tool == null)
            {
                Log.Error("Tool is still null...");
            }

            System.Diagnostics.Debug.Assert(tool != null);

            return tool;
        }
    }
}