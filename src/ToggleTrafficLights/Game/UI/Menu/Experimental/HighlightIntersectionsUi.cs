using System;
using System.Reflection;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components;
using Craxy.CitiesSkylines.ToggleTrafficLights.ModTools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using JetBrains.Annotations;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Experimental
{
    public class HighlightIntersectionsUi : MonoBehaviour
    {

        #region static
        private static HighlightIntersectionsUi _ui = null;
        public static void ToggleShow()
        {
            DebugLog.Info("HighlightIntersectionsUi: ToggleShow");

            if (_ui == null)
            {
                //gets enabled automatically
                var toolControl = Singleton<ToolManager>.instance;
                _ui = toolControl.gameObject.AddComponent<HighlightIntersectionsUi>();
                _ui.enabled = true;
            }
            else
            {
                _ui.enabled = !_ui.enabled;
            }
        }

        #endregion

        #region fields
        private Vector2 _scrollPosition;
        private Texture2D _backgroundTexture = null;
        private ColorPicker _colorPicker = null;
        private Monocolor _monocolor = null;
        public ServiceSetting _serviceSetting = null;
        #endregion

        #region MonoBehavior

        [UsedImplicitly]
        private void Start()
        {
            DebugLog.Info("HighlightIntersectionsUi: Start");

            _backgroundTexture = new Texture2D(1, 1);
            _backgroundTexture.SetPixel(0, 0, new Color(0.321f, 0.321f, 0.321f, 1.0f));
            _backgroundTexture.Apply();

            _colorPicker = gameObject.AddComponent<ColorPicker>();

            _monocolor = new Monocolor(_colorPicker);
            _serviceSetting = new ServiceSetting();

            name = "HighlightIntersectionsUI";
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            DebugLog.Info("HighlightIntersectionsUi: OnDestroy");

            if (_backgroundTexture != null)
            {
                Destroy(_backgroundTexture);
            }
            if (_colorPicker != null)
            {
                Destroy(_colorPicker);
            }
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            DebugLog.Info("HighlightIntersectionsUi: OnEnable");
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            DebugLog.Info("HighlightIntersectionsUi: OnDisable");
        }
        #endregion

        #region UI

        [UsedImplicitly]
        private void OnGUI()
        {

            var left = 0f;
            var top = 50f;
            var width = 250f;
            var height = 550f;
            var padding = 5f;

            GUILayout.BeginArea(new Rect(left, top, width, height));
            {
                GUI.Box(new Rect(0f, 0f, width, height), _backgroundTexture);
                GUILayout.BeginArea(new Rect(padding, padding, width - 2 * padding, height - 2 * padding));
                {
                    GUILayout.BeginVertical();
                    {
                        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
                        {
                            ShowGuiContent();
                        }
                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndArea();
            }
            GUILayout.EndArea();
        }

        private void ShowGuiContent()
        {
            GUILayout.Label("<size=15><b>Intersection Highlighting</b></size>");
            GUILayout.Space(10f);
            _monocolor.ShowGui();
            GUILayout.Space(5f);
            _serviceSetting.ShowGui();
        }

        #endregion

        public sealed class Monocolor
        {
            #region fields

            public bool UseMonocolor = false;
            public ColorSetting[] ColorSettings = null;
            #endregion

            public Monocolor(ColorPicker colorPicker)
            {
                var im = Singleton<InfoManager>.instance;

                ColorSettings = new[]
                {
                    new ColorSetting("_InfoCurrentColor", im.m_properties.m_neutralColor.linear, colorPicker)
                    {
                        OnColorChanged = c => Shader.SetGlobalColor("_InfoCurrentColor", c.linear),
                        ShowResetButton = true,
                    },
                    new ColorSetting("_InfoCurrentColorB", im.m_properties.m_neutralColor.linear, colorPicker)
                    {
                        OnColorChanged = c => Shader.SetGlobalColor("_InfoCurrentColorB", c.linear),
                        ShowResetButton = true,
                    },
//                    new ColorSetting("ambientSkyColor", im.m_properties.m_ambientColor, colorPicker)
//                    {
//                        OnColorChanged = c => RenderSettings.ambientSkyColor = c,
//                        ShowResetButton = true,
//                    },
//                    new ColorSetting("ambientEquatorColor", RenderSettings.ambientEquatorColor, colorPicker)
//                    {
//                        OnColorChanged = c => RenderSettings.ambientEquatorColor = c,
//                        ShowResetButton = true,
//                    },
//                    new ColorSetting("ambientGroundColor", RenderSettings.ambientGroundColor, colorPicker)
//                    {
//                        OnColorChanged = c => RenderSettings.ambientGroundColor = c,
//                        ShowResetButton = true,
//                    },
//                    new ColorSetting("ambientLight", RenderSettings.ambientLight, colorPicker)
//                    {
//                        OnColorChanged = c => RenderSettings.ambientLight = c,
//                        ShowResetButton = true,
//                    },
//                    new ColorSetting("fogColor", RenderSettings.fogColor, colorPicker)
//                    {
//                        OnColorChanged = c => RenderSettings.fogColor = c,
//                        ShowResetButton = true,
//                    },
                };

            }

            public void ShowGui()
            {
                var pre = UseMonocolor;
                UseMonocolor = GUILayout.Toggle(UseMonocolor, "<b>moncolor</b>");

                if (pre != UseMonocolor)
                {
                    var im = Singleton<InfoManager>.instance;

                    if (UseMonocolor)
                    {
                        //enable
//                        RenderSettings.ambientSkyColor = im.m_properties.m_ambientColor;
//                        Shader.SetGlobalColor("_InfoCurrentColor", im.m_properties.m_modeProperties[(int)InfoManager.InfoMode.None].m_activeColor.linear);
//                        Shader.SetGlobalColor("_InfoCurrentColorB", im.m_properties.m_modeProperties[(int)InfoManager.InfoMode.None].m_activeColorB.linear);

                        var rm = Singleton<RenderManager>.instance;
                        var cm = rm.m_objectColorMap;
                        for (int x = 0; x < cm.width; x++)
                        {
                            for (int y = 0; y < cm.height; y++)
                            {
                                cm.SetPixel(x, y, im.m_properties.m_neutralColor);
                            }
                        }

                        Shader.EnableKeyword("INFOMODE_ON");
                        Shader.DisableKeyword("INFOMODE_OFF");
                    }
                    else
                    {
                        //disable
//                        RenderSettings.ambientSkyColor = Singleton<RenderManager>.instance.m_properties.m_ambientLight;
//                        Shader.SetGlobalColor("_InfoCurrentColor", im.m_properties.m_neutralColor.linear);
//                        Shader.SetGlobalColor("_InfoCurrentColorB", im.m_properties.m_neutralColor.linear);

                        Shader.EnableKeyword("INFOMODE_OFF");
                        Shader.DisableKeyword("INFOMODE_ON");
                    }

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

//                if (UseMonocolor)
//                {
//                    ShowOptionsGui();
//                }
            }

            private void ShowOptionsGui()
            {
                using (Layout.Vertical())
                {
                    //colors
                    GUILayout.Label("InfoMode Colors:");
                    foreach (var cs in ColorSettings)
                    {
                        cs.ShowGui();
                    }
                }
            }
        }

        public class ColorSetting
        {
            private Color _color;
            public Func<Color> GetResetColor { get; set; }
            public bool ShowResetButton { get; set; }
            public Color Color
            {
                get { return _color; }
                set
                {
                    if (value != _color)
                    {
                        _color = value;
                        _OnColorChanged(value);                        
                    }
                }
            }

            public Action<Color> OnColorChanged { get; set; }
            public string Title { get; set; }
            public ColorPicker ColorPicker { get; set; }

            public ColorSetting(string title, Color initialColor, ColorPicker colorPicker)
            {
                _color = initialColor;
                Title = title;
                ColorPicker = colorPicker;
                GetResetColor = () => initialColor;
                OnColorChanged = color => { };
                ShowResetButton = true;
            }

            public void Reset()
            {
                Color = GetResetColor();
            }

            public void ShowGui()
            {
                using (Layout.Horizontal())
                {
                    GuiControls.ColorField(Title, Title, _color, ColorPicker, _OnColorChanged);

                    if (ShowResetButton)
                    {
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Reset"))
                        {
                            Reset();
                        }
                    }
                }
            }

            private void _OnColorChanged(Color color)
            {
                _color = color;
                OnColorChanged(color);
            }
        }

        public class ServiceSetting
        {
            public ItemClass.Service Service = ItemClass.Service.None;
            public ItemClass.SubService SubService = ItemClass.SubService.None;
            public ItemClass.Level Level = ItemClass.Level.None;
            public float FadeLength = 300f;
            public bool InvertDirection = false;
            private static readonly string[] services = Enum.GetNames(typeof (ItemClass.Service));
            private static readonly string[] subServices = Enum.GetNames(typeof(ItemClass.SubService));
            private static readonly string[] levels = Enum.GetNames(typeof(ItemClass.Level));
            public bool ShowTrafficCoverage = false;

            public void ShowGui()
            {
                using (Layout.Vertical())
                {
                    GUILayout.Label("<b>Service</b>");

                    var pre = ShowTrafficCoverage;
                    ShowTrafficCoverage = GUILayout.Toggle(ShowTrafficCoverage, "Traffic Coverage");
                    if (pre != ShowTrafficCoverage)
                    {
                        var im = Singleton<InfoManager>.instance;

//                        var mainLight = im.GetNonPublicField<InfoManager, Light>("m_mainLight");
//                        var cameraController = im.GetNonPublicField<InfoManager, CameraController>("m_cameraController");
                        if (ShowTrafficCoverage)
                        {
                            //enable
                            im.SetNonPublicField("m_currentMode", InfoManager.InfoMode.Traffic);

//                            mainLight.color = im.m_properties.m_lightColor;
//                            mainLight.intensity = im.m_properties.m_lightIntensity;
//
//                            cameraController.SetViewMode(CameraController.ViewMode.Info);

                            RenderSettings.ambientSkyColor = im.m_properties.m_ambientColor;
                            Shader.SetGlobalColor("_InfoCurrentColor", im.m_properties.m_modeProperties[(int)InfoManager.InfoMode.Traffic].m_activeColor.linear);
                            Shader.SetGlobalColor("_InfoCurrentColorB", im.m_properties.m_modeProperties[(int)InfoManager.InfoMode.Traffic].m_activeColorB.linear);
                        }
                        else
                        {
                            //disable
                            im.SetNonPublicField("m_currentMode", InfoManager.InfoMode.None);

//                            mainLight.color = im.GetNonPublicField<InfoManager, Color>("m_wasLightColor");
//                            mainLight.intensity = im.GetNonPublicField<InfoManager, float>("m_wasLightIntensity");
//
//                            cameraController.SetViewMode(CameraController.ViewMode.Normal);

                            RenderSettings.ambientSkyColor = Singleton<RenderManager>.instance.m_properties.m_ambientLight;
                            Shader.SetGlobalColor("_InfoCurrentColor", im.m_properties.m_neutralColor.linear);
                            Shader.SetGlobalColor("_InfoCurrentColorB", im.m_properties.m_neutralColor.linear);
                        }

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
                }

//                using (Layout.Vertical())
//                {
//                    GUILayout.Label("<b>Service</b>");
//                    GuiControls.ComboBox("Service", "Service", services, (int) Service, i => _OnSelectedServiceChanged((ItemClass.Service) i));
//                    GuiControls.ComboBox("SubServices", "SubServices", subServices, (int)SubService, i => _OnSelectedSubServiceChanged((ItemClass.SubService)i));
//                    GuiControls.ComboBox("Level", "Level", levels, (int)Level + 1, i => _OnSelectedLevelChanged((ItemClass.Level)(i-1)));
//                    GuiControls.InputField("FadeLength", "FadeLength", FadeLength, _OnFadeLengthChanged, Parser.ParseFloat, 65f);
//                }
            }

            private void _OnSelectedServiceChanged(ItemClass.Service service)
            {
                Service = service;
                UpdateMode();
            }
            private void _OnSelectedSubServiceChanged(ItemClass.SubService subService)
            {
                SubService = subService;
                UpdateMode();
            }
            private void _OnSelectedLevelChanged(ItemClass.Level level)
            {
                Level = level;
                UpdateMode();
            }
            private void _OnFadeLengthChanged(float fadeLength)
            {
                FadeLength = fadeLength;
                UpdateMode();
            }

            public void UpdateMode()
            {
                Singleton<CoverageManager>.instance.SetMode(Service, SubService, Level, FadeLength, InvertDirection);
            }
        }
    }
}