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
            var ttltool = GetTrafficLightsTool();

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

        private ToolBase GetTrafficLightsTool()
        {
            var tool = ToolsModifierControl.toolController.gameObject.GetComponent<Tools.ToggleTrafficLightsTool>();
            if (tool == null)
            {
                tool = ToolsModifierControl.toolController.gameObject.AddComponent<Tools.ToggleTrafficLightsTool>();
                DebugLog.Message("Simulation: ToggleTrafficLightsTool created");
            }

            System.Diagnostics.Debug.Assert(tool != null);

            return tool;
        }
    }
}