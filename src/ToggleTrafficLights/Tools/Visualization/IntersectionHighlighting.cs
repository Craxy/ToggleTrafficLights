using System;
using System.Linq;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools.Visualization
{
    public class IntersectionHighlighting
    {
        #region fields
        private HighlightingModes.InfoType _infoType;
        #endregion

        public IntersectionHighlighting()
        {
            Enabled = false;
            InfoType = HighlightingModes.InfoType.None;
            HighlightingType = HighlightingModes.HighlightingType.Circle;
            HighlightingMode = HighlightingModes.HighlightingMode.TrafficLights;
            IntersectionsToHighlight = HighlightingModes.IntersectionsToHighlight.AllIntersections;

            HasTrafficLightsColor = new Color(0.004f, 0.125f, 0.569f, 0.75f);
            DoesNotHaveTrafficLightsColor = new Color(0.835f, 0.384f, 0.0f, 0.75f);
        }

        #region Settings

        public bool Enabled { get; private set; }

        public HighlightingModes.InfoType InfoType
        {
            get { return _infoType; }
            set
            {
                if (_infoType != value)
                {
                    _infoType = value;
                    OnInfoTypeChanged(value);
                }
            }
        }

        public HighlightingModes.HighlightingType HighlightingType { get; set; }
        public HighlightingModes.HighlightingMode HighlightingMode { get; set; }
        public HighlightingModes.IntersectionsToHighlight IntersectionsToHighlight { get; set; }
        public Color HasTrafficLightsColor { get; set; }
        public Color DoesNotHaveTrafficLightsColor { get; set; }

        #endregion

        #region base controls
        public void Activate()
        {
            Enabled = true;

            OnInfoTypeChanged(InfoType);
        }

        public void Deactivate()
        {
            Enabled = false;

            OnInfoTypeChanged(HighlightingModes.InfoType.None);
        }
        #endregion

        #region game loop

        public void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            if (!Enabled)
            {
                return;
            }

            //InfoType must be set extra (when enabled or changed)


            //highlighting
            var nm = Singleton<NetManager>.instance;
            for (ushort i = 0; i < nm.m_nodes.m_size; i++)
            {
                var node = nm.m_nodes.m_buffer[i];

                if (!IsNodeToHighlight(i, node))
                {
                    continue;
                }

                var color = GetColor(i, node);

                ++Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls;
                Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, color, node.m_position, node.Info.m_halfWidth * 2, -1f, 1280f, false, false);
            }
        }
        #endregion

        #region helper
        private bool IsNodeToHighlight(ushort nodeId, NetNode node)
        {
            if (node.m_flags.IsFlagSet(NetNode.Flags.None)
                || !node.m_flags.IsFlagSet(NetNode.Flags.Junction)
                || !ToggleTrafficLightsTool.IsValidRoadNode(node))
            {
                return false;
            }

            var m = IntersectionsToHighlight;

            if (m.IsFlagSet(HighlightingModes.IntersectionsToHighlight.AllIntersections))
            {
                return true;
            }

            var state = ToggleTrafficLightsTool.HasTrafficLights(node.m_flags);
            if (m.IsFlagSet(HighlightingModes.IntersectionsToHighlight.IntersectionsWithTrafficLights)
                && state)
            {
                return true;
            }
            if (m.IsFlagSet(HighlightingModes.IntersectionsToHighlight.IntersectionsWithoutTrafficLights)
                && !state)
            {
                return true;
            }


            if (m.IsFlagSet(HighlightingModes.IntersectionsToHighlight.IntersectionsWithDefaultState)
                || m.IsFlagSet(HighlightingModes.IntersectionsToHighlight.IntersectionsWithoutDefaultState))
            {
                var def = ToggleTrafficLightsTool.WantTrafficLights(nodeId, node);
                if (m.IsFlagSet(HighlightingModes.IntersectionsToHighlight.IntersectionsWithDefaultState)
                    && def)
                {
                    return true;
                }
                if (m.IsFlagSet(HighlightingModes.IntersectionsToHighlight.IntersectionsWithoutDefaultState)
                    && !def)
                {
                    return true;
                }
            }

            return false;
        }

        private Color GetColor(ushort nodeId, NetNode node)
        {
            switch (HighlightingMode)
            {
                case HighlightingModes.HighlightingMode.TrafficLights:
                    {
                        if (ToggleTrafficLightsTool.HasTrafficLights(node.m_flags))
                        {
                            return HasTrafficLightsColor;
                        }
                        else
                        {
                            return DoesNotHaveTrafficLightsColor;
                        }
                    }
                    break;
                case HighlightingModes.HighlightingMode.Default:
                    {
                        if (ToggleTrafficLightsTool.WantTrafficLights(nodeId, node))
                        {
                            return HasTrafficLightsColor;
                        }
                        else
                        {
                            return DoesNotHaveTrafficLightsColor;
                        }
                    }
                    break;
                case HighlightingModes.HighlightingMode.DifferenceToDefault:
                    {
                        var want = ToggleTrafficLightsTool.WantTrafficLights(nodeId, node);
                        var has = ToggleTrafficLightsTool.HasTrafficLights(node.m_flags);

                        if (want == has)
                        {
                            return HasTrafficLightsColor;
                        }
                        else
                        {
                            return DoesNotHaveTrafficLightsColor;
                        }
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion

        #region specific controls
        private void OnInfoTypeChanged(HighlightingModes.InfoType infoType)
        {
            if (!Enabled)
            {
                return;
            }

            var im = Singleton<InfoManager>.instance;
            switch (infoType)
            {
                case HighlightingModes.InfoType.None:
                    {
                        im.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
                    }
                    break;
                case HighlightingModes.InfoType.Traffic:
                    {
                        im.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);

                        im.SetNonPublicField("m_currentMode", InfoManager.InfoMode.Traffic);
                        {
                            var mainLight = im.GetNonPublicField<InfoManager, Light>("m_mainLight");
                            mainLight.color = im.m_properties.m_lightColor;
                            mainLight.intensity = im.m_properties.m_lightIntensity;
                        }
                        RenderSettings.ambientSkyColor = im.m_properties.m_ambientColor;
                        Shader.SetGlobalColor("_InfoCurrentColor", im.m_properties.m_modeProperties[(int)InfoManager.InfoMode.Traffic].m_activeColor.linear);
                        Shader.SetGlobalColor("_InfoCurrentColorB", im.m_properties.m_modeProperties[(int)InfoManager.InfoMode.Traffic].m_activeColorB.linear);

                        Shader.EnableKeyword("INFOMODE_ON");
                        Shader.DisableKeyword("INFOMODE_OFF");

                        Singleton<CoverageManager>.instance.SetMode(ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None, 300f, false);
                        Singleton<ImmaterialResourceManager>.instance.ResourceMapVisible = ImmaterialResourceManager.Resource.None;
                        Singleton<ElectricityManager>.instance.ElectricityMapVisible = false;
                        Singleton<WaterManager>.instance.WaterMapVisible = false;
                        Singleton<DistrictManager>.instance.DistrictsInfoVisible = false;
                        Singleton<TransportManager>.instance.LinesVisible = false;
                        Singleton<WindManager>.instance.WindMapVisible = false;
                        Singleton<TerrainManager>.instance.TransparentWater = false;
                        Singleton<BuildingManager>.instance.UpdateBuildingColors();
                        Singleton<NetManager>.instance.UpdateSegmentColors();
                        Singleton<NetManager>.instance.UpdateNodeColors();
                    }
                    break;
                case HighlightingModes.InfoType.MonocolorTraffic:
                    {
                        im.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
                        im.CallNonPublicMethod("SetMode", InfoManager.InfoMode.Traffic, InfoManager.SubInfoMode.Default);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("infoType");
            }
        }
        #endregion

        public static class HighlightingModes
        {
            public enum InfoType
            {
                None = 0,
                Traffic = 1,
                MonocolorTraffic = 2,
            }

            public enum HighlightingType
            {
                Circle = 0,
                Cylinder = 1,
            }

            public enum HighlightingMode
            {
                TrafficLights = 0,
                DifferenceToDefault = 1,
                Default = 2,
            }

            [Flags]
            public enum IntersectionsToHighlight
            {
                IntersectionsWithTrafficLights = 0,
                IntersectionsWithoutTrafficLights = 1,
                AllIntersections = IntersectionsWithTrafficLights | IntersectionsWithoutTrafficLights,
                IntersectionsWithDefaultState = 2,
                IntersectionsWithoutDefaultState = 4,
            }
        }

    }
}