using System;
using ColossalFramework;
using ColossalFramework.Steamworks;
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

            if (!m_toolController.IsInsideUI && Cursor.visible && IsValidRoadNode())
            {
                var node = GetNetNode();
                var hasTrafficLight = CitiesHelper.HasTrafficLights(node.m_flags);
                var txt = string.Join("\n", new[]
                    {
                        string.Format("Traffic lights: {0}", hasTrafficLight),
                        string.Format("      Original: {0}", WantTrafficLights()),

#if DEBUG
                        string.Format("          Node: {0}", m_hoverInstance.NetNode),
#endif
                    });

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

            if (!m_toolController.IsInsideUI && Cursor.visible && IsValidRoadNode())
            {
                var current = Event.current;
                //button=0 -> left click
                if (current.type == EventType.MouseDown && current.button == 0)
                {
                    ToggleTrafficLights();
                }
                else if (current.type == EventType.MouseDown && current.button == 1)
                {
                    var wantLights = WantTrafficLights();
                    var hasLights = HasTrafficLights(GetNetNode().m_flags);

                    if (hasLights != wantLights)
                    {
                        ToggleTrafficLights();
                    }
                }
            }
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            if (IsValidRoadNode())
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

        public event EventHandler<EventArgs<bool>> EnabledChanged;
        protected virtual void OnOnEnabledChanged(bool isEnabled)
        {
            var handler = EnabledChanged;
            if (handler != null)
            {
                handler(this, new EventArgs<bool>(isEnabled));
            }
        }

        #endregion

        #region Node
        private bool IsValidRoadNode()
        {
            if (m_hoverInstance.NetNode == 0)
            {
                return false;
            }

            //detect only road intersections
            var node = GetNetNode();
            return IsValidRoadNode(node);
        }

        public static bool IsValidRoadNode(NetNode node)
        {
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

        public static NetNode.Flags ToggleTrafficLights(NetNode.Flags flags)
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

        public static NetNode.Flags SetTrafficLights(NetNode.Flags flags)
        {
            return flags | NetNode.Flags.TrafficLights;
        }

        public static NetNode.Flags UnsetTrafficLights(NetNode.Flags flags)
        {
            return flags & ~NetNode.Flags.TrafficLights;
        }
        private void ToggleTrafficLights()
        {
            var node = GetNetNode();

            node.m_flags = ToggleTrafficLights(node.m_flags);

            Singleton<NetManager>.instance.m_nodes.m_buffer[GetNetNodeId()] = node;
        }

        private bool WantTrafficLights()
        {
            return WantTrafficLights((ushort) GetNetNodeId(), GetNetNode());
        }
        public static bool WantTrafficLights(ushort nodeId, NetNode node)
        {
            //Source: RoadBaseAI.UpdateNodeFlags
            //seems pretty......human unfriendly....
            NetNode.Flags flags1 = node.m_flags;
            uint num1 = 0U;
            int num2 = 0;
            NetManager instance = Singleton<NetManager>.instance;
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            bool flag = ((RoadBaseAI)node.Info.GetAI()).WantTrafficLights();
            for (int index = 0; index < 8; ++index)
            {
                ushort segment = node.GetSegment(index);
                if ((int)segment != 0)
                {
                    NetInfo info = instance.m_segments.m_buffer[(int)segment].Info;
                    uint num6 = 1U << (int)(info.m_class.m_level & (ItemClass.Level)31);
                    if (((int)num1 & (int)num6) == 0)
                    {
                        num1 |= num6;
                        ++num2;
                    }
                    if (info.m_netAI.WantTrafficLights())
                        flag = true;
                    int forward = 0;
                    int backward = 0;
                    instance.m_segments.m_buffer[(int)segment].CountLanes(segment, NetInfo.LaneType.Vehicle, VehicleInfo.VehicleType.Car, ref forward, ref backward);
                    if ((int)instance.m_segments.m_buffer[(int)segment].m_endNode == (int)nodeId)
                    {
                        if (forward != 0)
                        {
                            ++num3;
                            num4 += forward;
                        }
                    }
                    else if (backward != 0)
                    {
                        ++num3;
                        num4 += backward;
                    }
                    if (forward != 0 || backward != 0)
                        ++num5;
                }
            }
            NetNode.Flags flags2 = num2 < 2 ? flags1 & ~NetNode.Flags.Transition : flags1 | NetNode.Flags.Transition;
            NetNode.Flags flags3 = !flag ||
                num3 <= 2 && (num3 < 2 || num5 < 3 || num4 <= 6) ||
                (flags2 & NetNode.Flags.Junction) == NetNode.Flags.None ? flags2 & ~NetNode.Flags.TrafficLights : flags2 | NetNode.Flags.TrafficLights;

            return (flags3 & NetNode.Flags.TrafficLights) == NetNode.Flags.TrafficLights;
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