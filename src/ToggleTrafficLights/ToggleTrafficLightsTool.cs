using ColossalFramework;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights
{
    public sealed class ToggleTrafficLightsTool : ToolBase //TODO: auf DefaultTool upgrade?
    {
        private ushort _currentNetNodeIdx;

        protected override void OnEnable()
        {
            base.OnEnable();

            Log.Message("ToggleTrafficLightsTool enabled");
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Log.Message("ToggleTrafficLightsTool disabled");
        }

        public override void SimulationStep()
        {
            base.SimulationStep();
            //DefaultTool does not detect NetNodes
            //-> I'm using code from DefaultTool but keep NetNode

            var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var mouseRayLength = Camera.main.farClipPlane;
            var rayRight = Camera.main.transform.TransformDirection(Vector3.right);
            var mouseRayValid = !UIView.IsInsideUI() && Cursor.visible;

            if (mouseRayValid)
            {
                var defaultService = new ToolBase.RaycastService(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Layer.Default);
                var input = new ToolBase.RaycastInput(mouseRay, mouseRayLength)
                {
                    m_rayRight = rayRight,
                    m_netService = defaultService,
                    m_ignoreNodeFlags = NetNode.Flags.None
                    //other flags and services unnecessary -- I'm only interested in NetNodes
                };
                RaycastOutput output;
                if (!RayCast(input, out output))
                {
                    DebugLog.Error("Not a valid Raycast!");
                    //TODO: Fehlerbehandlung?
                    _currentNetNodeIdx = 0;
                    return;
                }

                //test for intersection
                var node = GetNetNode(output.m_netNode);
                if ((node.m_flags & NetNode.Flags.Junction) == NetNode.Flags.Junction)
                {
                    _currentNetNodeIdx = output.m_netNode;
                }
                else
                {
                    _currentNetNodeIdx = 0;
                }

            }
        }

        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            if (!m_toolController.IsInsideUI && Cursor.visible && IsOverNetNode())
            {
                var node = GetCurrentNetNode();
                var txt = string.Format("Traffic lights: {0}", HasTrafficLights(node.m_flags));
#if DEBUG
                txt = string.Format("{0}\nNode: {1}", txt, _currentNetNodeIdx);
#endif
                ShowToolInfo(true, txt, node.m_position);
            }
            else
            {
                ShowToolInfo(false, null, Vector3.zero);
            }
        }

        protected override void OnToolGUI()
        {
            base.OnToolGUI();

            if (m_toolController.IsInsideUI)
            {
                return;
            }

            var current = Event.current;
            if (current.type == EventType.MouseDown)
            {
                if (current.button != 0)
                {
                    return;
                }

                if (!IsOverNetNode())
                {
                    return;
                }

                ToggleTrafficLights(_currentNetNodeIdx);
            }
        }

        #region Status

        private bool IsOverNetNode()
        {
            return _currentNetNodeIdx != 0;
        }
        #endregion

        #region Helper

        private NetNode GetCurrentNetNode()
        {
            return GetNetNode(_currentNetNodeIdx);
        }
        private static NetNode GetNetNode(ushort index)
        {
            return Singleton<NetManager>.instance.m_nodes.m_buffer[index];
        }

        private static void SetNetNode(ushort index, NetNode node)
        {
            Singleton<NetManager>.instance.m_nodes.m_buffer[index] = node;
        }

        public static bool HasTrafficLights(NetNode.Flags flags)
        {
            return (flags & NetNode.Flags.TrafficLights) == NetNode.Flags.TrafficLights;
        }

        private static void ToggleTrafficLights(ushort index)
        {
            var node = GetNetNode(index);

            if (HasTrafficLights(node.m_flags))
            {
                node.m_flags &= ~NetNode.Flags.TrafficLights;
                DebugLog.Message("Traffic lights enabled");
            }
            else
            {
                node.m_flags |= NetNode.Flags.TrafficLights;
                DebugLog.Message("Traffic lights disabled");
            }

            SetNetNode(index, node);
        }
        #endregion
    }
}