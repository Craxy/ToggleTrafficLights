using System;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools
{
    public class ToggleTrafficLightsTool : DefaultToolWithNetNodeDetection
    {
        #region Start/End
        private void Start()
        {
            DebugLog.Message("ToggleTrafficLightsTool start");
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            DebugLog.Message("ToggleTrafficLightsTool destroyed");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            OnOnEnabledChanged(true);

            DebugLog.Message("ToggleTrafficLightsTool enabled");
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            OnOnEnabledChanged(false);

            DebugLog.Message("ToggleTrafficLightsTool disabled");
        }
        #endregion

        #region game loop
        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            if (!m_toolController.IsInsideUI && Cursor.visible && IsValidNetNode())
            {
                var node = GetNetNode();
                var hasTrafficLight = CitiesHelper.HasTrafficLights(node.m_flags);
                var txt = string.Format("Traffic lights: {0}", hasTrafficLight);
#if DEBUG
                txt = string.Format("{0}\nNode: {1}", txt, m_hoverInstance.NetNode);
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

            if (!m_toolController.IsInsideUI && Cursor.visible && IsValidNetNode())
            {
                var current = Event.current;
                //button=0 -> left click
                if (current.type == EventType.MouseDown && current.button == 0)
                {
                    ToggleTrafficLights();
                }
                else if (current.type == EventType.MouseDown && current.button == 1)
                {
                    //detect only road intersections
                    var node = GetNetNode();
                    var info = node.Info;
                    var ai = info.GetAI();

                    var isRoad = ai is RoadBaseAI;
                    DebugLog.Info("is road: {0}", isRoad);
                }
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (IsValidNetNode())
            {
                var node = GetNetNode();
                var position = node.m_position;

                var info = node.Info;

                var color = GetToolColor(false, false);
                //http://paletton.com/#uid=13r0u0k++++qKZWAF+V+VAFZWqK
                if (HasTrafficLights(node.m_flags))
                {
                    color = new Color(0.2f, 0.749f, 0.988f, color.a);
                }
                else
                {
                    color = new Color(0.0f, 0.369f, 0.525f, color.a);
                }

                ++Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls;
                Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, color, position, info.m_halfWidth * 2, -1f, 1280f, false, false);
            }
        }
        #endregion

        #region Events

        public event EventHandler<EventArgs<bool>> OnEnabledChanged;
        protected virtual void OnOnEnabledChanged(bool isEnabled)
        {
            var handler = OnEnabledChanged;
            if (handler != null)
            {
                handler(this, new EventArgs<bool>(isEnabled));
            }
        }

        #endregion

        #region Node
        private bool IsValidNetNode()
        {
            if (m_hoverInstance.NetNode == 0)
            {
                return false;
            }

            //detect only road intersections
            var node = GetNetNode();
            var info = node.Info;
            var ai = info.GetAI();

            return ai is RoadBaseAI;
        }

        private int GetNetNodeId()
        {
            return m_hoverInstance.NetNode;
        }

        private NetNode GetNetNode()
        {
            if (m_hoverInstance.NetNode == 0)
            {
                throw new InvalidOperationException("Not a valid NetNode");
            }

            return Singleton<NetManager>.instance.m_nodes.m_buffer[GetNetNodeId()];
        }
        #endregion

        #region Helper
        public static bool HasTrafficLights(NetNode.Flags flags)
        {
            return (flags & NetNode.Flags.TrafficLights) == NetNode.Flags.TrafficLights;
        }

        private static NetNode.Flags ToggleTrafficLights(NetNode.Flags flags)
        {
            if (HasTrafficLights(flags))
            {
                flags &= ~NetNode.Flags.TrafficLights;
                DebugLog.Message("Traffic lights disabled");
            }
            else
            {
                flags |= NetNode.Flags.TrafficLights;
                DebugLog.Message("Traffic lights enabled");
            }
            return flags;
        }
        private void ToggleTrafficLights()
        {
            var node = GetNetNode();

            node.m_flags = ToggleTrafficLights(node.m_flags);

            Singleton<NetManager>.instance.m_nodes.m_buffer[GetNetNodeId()] = node;
        }
        #endregion

        #region Ignore Flags

        public override NetNode.Flags GetNodeIncludeFlags()
        {
            return NetNode.Flags.Junction;
        }

        public override NetNode.Flags GetNodeIgnoreFlags()
        {
            //just ~Junction is not enough: roads have usually other flags to -> get ignored
            return NetNode.Flags.None;
        }

        public override NetSegment.Flags GetSegmentIgnoreFlags()
        {
            return NetSegment.Flags.All;
        }

        public override Building.Flags GetBuildingIgnoreFlags()
        {
            return Building.Flags.All;
        }

        public override TreeInstance.Flags GetTreeIgnoreFlags()
        {
            return TreeInstance.Flags.All;
        }

        public override PropInstance.Flags GetPropIgnoreFlags()
        {
            return PropInstance.Flags.All;
        }

        public override Vehicle.Flags GetVehicleIgnoreFlags()
        {
            return Vehicle.Flags.All;
        }

        public override VehicleParked.Flags GetParkedVehicleIgnoreFlags()
        {
            return VehicleParked.Flags.All;
        }

        public override CitizenInstance.Flags GetCitizenIgnoreFlags()
        {
            return CitizenInstance.Flags.All;
        }

        public override TransportLine.Flags GetTransportIgnoreFlags()
        {
            return TransportLine.Flags.All;
        }

        public override District.Flags GetDistrictIgnoreFlags()
        {
            return District.Flags.All;
        }

        public override bool GetTerrainIgnore()
        {
            return true;
        }
        #endregion
    }

}