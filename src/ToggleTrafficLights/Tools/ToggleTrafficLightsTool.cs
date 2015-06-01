using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools
{
    public class ToggleTrafficLightsTool : DefaultToolWithNetNodeDetection
    {
        #region Start/End

        [UsedImplicitly]
        private void Start()
        {
            DebugLog.Message("ToggleTrafficLightsTool start");
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();

            var ump = UndergroundModePanel.Get();
            if (ump != null)
            {
                Destroy(ump.gameObject);
            }

            DebugLog.Message("ToggleTrafficLightsTool destroyed");
        }

        protected override void OnEnable()
        {
            Options.Ensure();

            base.OnEnable();

            ActivateGroundMode(Options.instance.UsedGroundMode);
            UndergroundModePanel.GetOrCreate().Show(true);
            Options.instance.GroundModeChanged += ActivateGroundMode;

            OnOnEnabledChanged(true);

            DebugLog.Message("ToggleTrafficLightsTool enabled");
        }

        protected override void OnDisable()
        {
            Options.instance.GroundModeChanged -= ActivateGroundMode;
            UndergroundModePanel.GetOrCreate().Hide();
            ActivateGroundMode(Options.GroundMode.Ground);

            base.OnDisable();

            OnOnEnabledChanged(false);

            DebugLog.Message("ToggleTrafficLightsTool disabled");
        }
        #endregion

        #region game loop
        protected override void OnToolUpdate()
        {
            base.OnToolUpdate();

            var nodeId = GetCurrentNetNodeId();

            if (!m_toolController.IsInsideUI && Cursor.visible && IsValidRoadNode(nodeId))
            {
                var node = GetNetNode(nodeId);
                var hasTrafficLight = CitiesHelper.HasTrafficLights(node.m_flags);
                var txt = string.Join("\n", new[]
                    {
                        string.Format("Traffic lights: {0}", hasTrafficLight),
                        string.Format("      Original: {0}", WantTrafficLights(nodeId)),

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

            var nodeId = GetCurrentNetNodeId();

            if (!m_toolController.IsInsideUI && Cursor.visible && IsValidRoadNode(nodeId))
            {
                var current = Event.current;
                //button=0 -> left click
                if (current.type == EventType.MouseDown && current.button == 0)
                {
                    ToggleTrafficLights(nodeId);
                }
                else if (current.type == EventType.MouseDown && current.button == 1)
                {
                    var wantLights = WantTrafficLights(nodeId);
                    var hasLights = HasTrafficLights(GetNetNode(nodeId).m_flags);

                    if (hasLights != wantLights)
                    {
                        ToggleTrafficLights(nodeId);
                    }
                }
            }

            HandleGroundModeKeys();
        }

        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            var nodeId = GetCurrentNetNodeId();

//            if (Options.HighlightAllIntersections)
//            {
//                HighlightAllIntersections();
//            }

            if (OnRenderOverlay != null)
            {
                foreach (var action in OnRenderOverlay)
                {
                    action(cameraInfo);
                }
            }


            if (IsValidRoadNode(nodeId))
            {
                HighlightNode(cameraInfo, nodeId);
            }
        }


        #endregion

        #region overlays
        private void HighlightNode(RenderManager.CameraInfo cameraInfo, int nodeId)
        {
            var node = GetNetNode(nodeId);
            var position = node.m_position;

            var info = node.Info;

            var color = GetToolColor(false, false);
            //http://paletton.com/#uid=13r0u0k++++qKZWAF+V+VAFZWqK
            color = HasTrafficLights(node.m_flags) 
                    ? new Color(0.2f, 0.749f, 0.988f, color.a) 
                    : new Color(0.0f, 0.369f, 0.525f, color.a);

            ++Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls;
            Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, color, position, info.m_halfWidth * 2, -1f, 1280f, false, false);
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

        #region Actions

        private readonly List<Action<RenderManager.CameraInfo>> _onRenderOverlay = Enumerable.Empty<Action<RenderManager.CameraInfo>>().ToList();

        public IEnumerable<Action<RenderManager.CameraInfo>> OnRenderOverlay
        {
            get { return _onRenderOverlay; }
        }

        public void AddRenderOverlay(Action<RenderManager.CameraInfo> onRenderOverlay)
        {
            _onRenderOverlay.Add(onRenderOverlay);
        }

        public void RemoveRenderOverlay(Action<RenderManager.CameraInfo> onRenderOverlay)
        {
            _onRenderOverlay.Remove(onRenderOverlay);
        }

        public void ClearRenderOverlay()
        {
            _onRenderOverlay.Clear();
        }
        #endregion

        #region Underground
        private void HandleGroundModeKeys()
        {
            var os = Options.instance;
            if (os.ElevationUp.IsPressed() && os.ElevationDown.IsPressed())
            {
                ChangeGroundMode(Options.GroundMode.All);
                DebugLog.Info("ToggleTrafficLightsTool: Changed ground mode to {0}", Options.GroundMode.All);
            }
            else if(os.ElevationUp.IsPressed())
            {
                ChangeGroundMode(Options.GroundMode.Ground);
                DebugLog.Info("ToggleTrafficLightsTool: Changed ground mode to {0}", Options.GroundMode.Ground);
            }
            else if (os.ElevationDown.IsPressed())
            {
                ChangeGroundMode(Options.GroundMode.Underground);
                DebugLog.Info("ToggleTrafficLightsTool: Changed ground mode to {0}", Options.GroundMode.Underground);
            }

        }

        public void ChangeGroundMode(Options.GroundMode groundMode)
        {
            if (groundMode == Options.instance.UsedGroundMode)
            {
                return;
            }

            Options.instance.UsedGroundMode = groundMode;
            ActivateGroundMode(groundMode);
        }

        private void ActivateGroundMode(Options.GroundMode groundMode)
        {
            switch (groundMode)
            {
                case Options.GroundMode.Ground:
                    _nodeLayerIncludeFlags = ItemClass.Layer.Default;
                    ActivateGroundInfoView();
                    break;
                case Options.GroundMode.Underground:
                    _nodeLayerIncludeFlags = ItemClass.Layer.MetroTunnels;
                    ActivateUndergroundInfoView();
                    break;
                case Options.GroundMode.All:
                    _nodeLayerIncludeFlags = ItemClass.Layer.Default | ItemClass.Layer.MetroTunnels;
                    ActivateAllInfoView();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("groundMode");
            }
        }

        private static void ActivateGroundInfoView()
        {
            var im = Singleton<InfoManager>.instance;
            im.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
        }
        private static void ActivateUndergroundInfoView()
        {
            var im = Singleton<InfoManager>.instance;
            im.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            im.CallNonPublicMethod("SetMode", InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
        }
        private static void ActivateAllInfoView()
        {
            var im = Singleton<InfoManager>.instance;
            im.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            im.CallNonPublicMethod("SetMode", InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
        }

        #region Overrides of DefaultToolWithNetNodeDetection

        private ItemClass.Layer _nodeLayerIncludeFlags = ItemClass.Layer.Default;
        public override ItemClass.Layer GetNodeLayerIncludeFlags()
        {
            return _nodeLayerIncludeFlags;
        }

        #endregion

        #endregion

        #region Node

        private static bool IsValidRoadNode(int nodeId)
        {
            if (nodeId == 0)
            {
                return false;
            }

            var node = GetNetNode(nodeId);
            return IsValidRoadNode(node);

        }

        public static bool IsValidRoadNode(NetNode node)
        {
            var info = node.Info;
            var ai = info.GetAI();

            return ai is RoadBaseAI;
        }

        private int GetCurrentNetNodeId()
        {
            return m_hoverInstance.NetNode;
        }

        private static NetNode GetNetNode(int nodeId)
        {
            if (nodeId == 0)
            {
                throw new InvalidOperationException("Not a valid NetNode");
            }

            return Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId];
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
        private static void ToggleTrafficLights(int nodeId)
        {
            var node = GetNetNode(nodeId);

            node.m_flags = ToggleTrafficLights(node.m_flags);

            Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId] = node;
        }

        private static bool WantTrafficLights(int nodeId)
        {
            return WantTrafficLights((ushort) nodeId, GetNetNode(nodeId));
        }
        public static bool WantTrafficLights(ushort nodeId, NetNode node)
        {
            //Source: RoadBaseAI.UpdateNodeFlags
            //seems pretty......human unfriendly....
            var flags1 = node.m_flags;
            var num1 = 0U;
            var num2 = 0;
            var instance = Singleton<NetManager>.instance;
            var num3 = 0;
            var num4 = 0;
            var num5 = 0;
            var flag = ((RoadBaseAI)node.Info.GetAI()).WantTrafficLights();
            for (var index = 0; index < 8; ++index)
            {
                var segment = node.GetSegment(index);
                if (segment != 0)
                {
                    var info = instance.m_segments.m_buffer[segment].Info;
                    var num6 = 1U << (int)(info.m_class.m_level & (ItemClass.Level)31);
                    if (((int)num1 & (int)num6) == 0)
                    {
                        num1 |= num6;
                        ++num2;
                    }
                    if (info.m_netAI.WantTrafficLights())
                        flag = true;
                    var forward = 0;
                    var backward = 0;
                    instance.m_segments.m_buffer[segment].CountLanes(segment, NetInfo.LaneType.Vehicle, VehicleInfo.VehicleType.Car, ref forward, ref backward);
                    if (instance.m_segments.m_buffer[segment].m_endNode == nodeId)
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
            var flags2 = num2 < 2 ? flags1 & ~NetNode.Flags.Transition : flags1 | NetNode.Flags.Transition;
            var flags3 = !flag ||
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