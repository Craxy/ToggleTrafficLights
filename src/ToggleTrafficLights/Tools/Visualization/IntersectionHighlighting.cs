using System;
using System.Linq;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools.Visualization
{
    [Obsolete]
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
            OnInfoTypeChanged(HighlightingModes.InfoType.None);

            Enabled = false;
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

            if (m == HighlightingModes.IntersectionsToHighlight.AllIntersections)
            {
                return true;
            }

            var state = ToggleTrafficLightsTool.HasTrafficLights(node.m_flags);
            if (m == HighlightingModes.IntersectionsToHighlight.IntersectionsWithTrafficLights
                && state)
            {
                return true;
            }
            if (m == HighlightingModes.IntersectionsToHighlight.IntersectionsWithoutTrafficLights
                && !state)
            {
                return true;
            }


            if (m == HighlightingModes.IntersectionsToHighlight.IntersectionsWithDefaultState
                || m == HighlightingModes.IntersectionsToHighlight.IntersectionsWithoutDefaultState)
            {
                var def = ToggleTrafficLightsTool.WantTrafficLights(nodeId, node);
                if (m == HighlightingModes.IntersectionsToHighlight.IntersectionsWithDefaultState
                    && def == state)
                {
                    return true;
                }
                if (m == HighlightingModes.IntersectionsToHighlight.IntersectionsWithoutDefaultState
                    && def != state)
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
//                        {
//                            var mainLight = im.GetNonPublicField<InfoManager, Light>("m_mainLight");
//                            mainLight.color = im.m_properties.m_lightColor;
//                            mainLight.intensity = im.m_properties.m_lightIntensity;
//                        }
//                        RenderSettings.ambientSkyColor = im.m_properties.m_ambientColor;
                        Shader.SetGlobalColor("_InfoCurrentColor", im.m_properties.m_modeProperties[(int)InfoManager.InfoMode.Traffic].m_activeColor.linear);
                        Shader.SetGlobalColor("_InfoCurrentColorB", im.m_properties.m_modeProperties[(int)InfoManager.InfoMode.Traffic].m_activeColorB.linear);
//
//                        Shader.EnableKeyword("INFOMODE_ON");
//                        Shader.DisableKeyword("INFOMODE_OFF");

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
            public class NameAttribute : Attribute
            {
                public string Name { get; set; }

                public NameAttribute(string name)
                {
                    Name = name;
                }
            }

            public class IgnoreAttribute : Attribute
            {
                
            }

            public static string GetEnumName<TEnum>()
            {
                var na = Attribute.GetCustomAttribute(typeof (TEnum), typeof (NameAttribute)) as NameAttribute;
                return na != null ? na.Name : string.Empty;
            }

            public static string GetEnumValueName<TEnum>(TEnum value)
            {
                var type = typeof (TEnum);
                var memInfo = type.GetMember(value.ToString());
                var attributes = memInfo[0].GetCustomAttributes(typeof (NameAttribute), false);
                return ((NameAttribute)attributes.Last()).Name;
            }

            [Name("Info View")]
            public enum InfoType
            {
                [Name("None")]
                None,
                [Name("Traffic densitiy")]
                Traffic,
                [Name("Monocolored traffic density")]
                MonocolorTraffic,
            }

            [Ignore]
            public enum HighlightingType
            {
                Circle,
                Cylinder,
            }

            [Name("Highlighting Mode")]
            public enum HighlightingMode
            {
                [Name("Existence of Lights")]
                TrafficLights,
                [Name("Default state")]
                Default,
                [Name("Difference to default")]
                DifferenceToDefault,
            }

            [Name("Intersections to hightlight")]
            public enum IntersectionsToHighlight
            {
                [Name("with lights")]
                IntersectionsWithTrafficLights,
                [Name("without lights")]
                IntersectionsWithoutTrafficLights,
                [Name("all intersections")]
                AllIntersections,
                [Name("with default state")]
                IntersectionsWithDefaultState,
                [Name("without default state")]
                IntersectionsWithoutDefaultState,
            }
        }

    }
}