using System;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
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

            DebugLog.Message("ToggleTrafficLightsTool enabled");
        }

        protected override void OnDisable()
        {
            base.OnDisable();

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
            }
        }
        #endregion

        #region Node
        private bool IsValidNetNode()
        {
            return m_hoverInstance.NetNode != 0;
        }

        private int GetNetNodeId()
        {
            return m_hoverInstance.NetNode;
        }

        private NetNode GetNetNode()
        {
            if (!IsValidNetNode())
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
//            if (HasTrafficLights(node.m_flags))
//            {
//                node.m_flags &= ~NetNode.Flags.TrafficLights;
//                DebugLog.Message("Traffic lights disabled");
//            }
//            else
//            {
//                node.m_flags |= NetNode.Flags.TrafficLights;
//                DebugLog.Message("Traffic lights enabled");
//            }

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