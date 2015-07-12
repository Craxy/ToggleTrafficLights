using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools.Visualization
{
    public class IntersectionHighlighting
    {
        //todo: do not include intersections outside area
        //todo: disable when none

        #region fields
        private static readonly GameObject[] EmptyGameObjectsArray = new GameObject[0];
        
        private Mesh _cylinderMesh = null;
        private GameObject[] _cylindersToHighlight = EmptyGameObjectsArray;

        private Material _hasTrafficLightsOvergroundMaterial = null;
        private Material _hasNoTrafficLightsOvergroundMaterial = null;
        private Material _hasTrafficLightsUndergroundMaterial = null;
        private Material _hasNoTrafficLightsUndergroundMaterial = null;
        #endregion

        #region properties

        public Options.GroundMode IntersectionsToHighlight
        {
            get { return Options.HighlightIntersections.IntersectionsToHighlight.Value; }
        }
        public float MarkerHeight
        {
            get { return Options.HighlightIntersections.MarkerHeight.Value; }
        }
        public float MarkerRadius
        {
            get { return Options.HighlightIntersections.MarkerRadius.Value; }
        }
        public int NumberOfMarkerSides
        {
            get { return Options.HighlightIntersections.NumberOfMarkerSides.Value; }
        }

        public Color HasTrafficLightsColor
        {
            get { return Options.HighlightIntersections.HasTrafficLightsColor.Value; }
        }
        public Color HasNoTrafficLightsColor
        {
            get { return Options.HighlightIntersections.HasNoTrafficLightsColor.Value; }
        }

        public ToggleTrafficLightsTool Tool { get; private set; }
        #endregion

        #region ctor

        public IntersectionHighlighting(ToggleTrafficLightsTool tool)
        {
            Tool = tool;
        }
        #endregion

        #region MonoBehaviour
        public void Awake()
        {
            CreateMaterials();
            CreateMesh();
        }
        public void OnEnable()
        {
            UpdateIntersectionsToHighlight(true, true);
            SubscribeEvents();
        }

        public void OnDisable()
        {
            UnsubscribeEvents();
            DestroyCylindersToHighlight();
        }

        public void OnDestroy()
        {
            UnsubscribeEvents();
            DestroyCylindersToHighlight();

        }
        #endregion

        #region Initialization

        private void CreateMesh()
        {
            _cylinderMesh = MeshHelper.CreateCylinder(MarkerHeight, MarkerRadius, NumberOfMarkerSides);
        }

        //todo: new Material(Shader) is obsolete. What to use instead?
        // Shaders: http://docs.unity3d.com/Manual/SL-Shader.html
        private const string UndergroundShader = @"
Shader ""Underground Intersection Shader"" 
{
    Properties 
    { 
            _Color (""Main Color"", Color) = (1.0,1.0,1.0,1.0) 
    }
    SubShader 
    {
        // The Pass block causes the geometry of an object to be rendered once.
        Pass 
        {
            // don’t render polygons facing away from the viewer
            Cull Back
            // depth buffer. off for semitransparent effects
            ZWrite Off
            // ? rec by some nvdia presentation
            ZTest Always

            // Lighting for underground
            Lighting On
            Color [_Color]
            Material { Ambient [_Color] }
            //brighten up color: double: not bright enough
            SetTexture [_Dummy] { combine primary quad }
        }
    }
}";

        private void CreateMaterials()
        {
            //Shader.Find("Diffuse")
            _hasTrafficLightsOvergroundMaterial = new Material(Shader.Find("Diffuse"))
            {
                color = HasTrafficLightsColor,
            };
            _hasNoTrafficLightsOvergroundMaterial = new Material(Shader.Find("Diffuse"))
            {
                color = HasNoTrafficLightsColor,
            };
            _hasTrafficLightsUndergroundMaterial = new Material(UndergroundShader)
            {
                color = HasTrafficLightsColor,
            };
            _hasNoTrafficLightsUndergroundMaterial = new Material(UndergroundShader)
            {
                color = HasNoTrafficLightsColor,
            };
        }

        private void UpdateMaterialColors()
        {
            if (_hasTrafficLightsOvergroundMaterial == null)
            {
                CreateMaterials();
            }
            else
            {
                _hasNoTrafficLightsOvergroundMaterial.color = HasTrafficLightsColor;
                _hasNoTrafficLightsOvergroundMaterial.color = HasNoTrafficLightsColor;
                _hasTrafficLightsUndergroundMaterial.color = HasTrafficLightsColor;
                _hasNoTrafficLightsUndergroundMaterial.color = HasNoTrafficLightsColor;
            }
        }

        #region Events
        private void SubscribeEvents()
        {
            Options.ToggleTrafficLightsTool.GroundMode.ValueChanged += OnGroundModeChanged;
            Options.HighlightIntersections.IntersectionsToHighlight.ValueChanged += OnIntersectionsToHighlightChanged;
            Options.HighlightIntersections.HasTrafficLightsColor.ValueChanged += OnColorChanged;
            Options.HighlightIntersections.HasNoTrafficLightsColor.ValueChanged += OnColorChanged;
            Options.HighlightIntersections.MarkerHeight.ValueChanged += OnMarkerSizeChanged;
            Options.HighlightIntersections.MarkerRadius.ValueChanged += OnMarkerSizeChanged;
            Options.HighlightIntersections.NumberOfMarkerSides.ValueChanged += OnMarkerChanged;
        }

        private void UnsubscribeEvents()
        {
            Options.ToggleTrafficLightsTool.GroundMode.ValueChanged -= OnGroundModeChanged;
            Options.HighlightIntersections.IntersectionsToHighlight.ValueChanged -= OnIntersectionsToHighlightChanged;
            Options.HighlightIntersections.HasTrafficLightsColor.ValueChanged -= OnColorChanged;
            Options.HighlightIntersections.HasNoTrafficLightsColor.ValueChanged -= OnColorChanged;
            Options.HighlightIntersections.MarkerHeight.ValueChanged -= OnMarkerSizeChanged;
            Options.HighlightIntersections.MarkerRadius.ValueChanged -= OnMarkerSizeChanged;
            Options.HighlightIntersections.NumberOfMarkerSides.ValueChanged -= OnMarkerChanged;
        }
        private void OnGroundModeChanged(Options.GroundMode oldValue, Options.GroundMode newValue)
        {
            UpdateIntersectionsToHighlight(false, false);
        }
        private void OnIntersectionsToHighlightChanged(Options.GroundMode oldValue, Options.GroundMode newValue)
        {
            UpdateIntersectionsToHighlight(false, false);
        }
        private void OnColorChanged(Color oldValue, Color newValue)
        {
            UpdateIntersectionsToHighlight(true, false);
        }
        private void OnMarkerSizeChanged(float oldValue, float newValue)
        {
            UpdateIntersectionsToHighlight(false, true);
        }
        private void OnMarkerChanged(int oldValue, int newValue)
        {
            UpdateIntersectionsToHighlight(false, true);
        }
        #endregion
        #endregion

        #region Highlighting

        private void DestroyCylindersToHighlight()
        {
            if (_cylindersToHighlight != null)
            {
                foreach (var go in _cylindersToHighlight)
                {
                    Object.Destroy(go);
                }
                _cylindersToHighlight = EmptyGameObjectsArray;
            }
        }

        private void UpdateIntersectionsToHighlight(bool updateMaterial, bool updateMesh)
        {
            if (IntersectionsToHighlight == Options.GroundMode.None)
            {
                _cylindersToHighlight = EmptyGameObjectsArray;
                return;
            }

            DebugLog.Info("IntersectionHighlighting: Updating intersections...");

            DestroyCylindersToHighlight();
            if (updateMaterial)
            {
                UpdateMaterialColors();
            }
            if (updateMesh)
            {
                CreateMesh();
            }
            PlaceCylindersToHighlight();

            DebugLog.Info("IntersectionHighlighting: Intersections updated: {0}", _cylindersToHighlight.Length);
        }

        private void PlaceCylindersToHighlight()
        {
            var intersectionsToHighlight = IntersectionsToHighlight;
            _cylindersToHighlight = intersectionsToHighlight == Options.GroundMode.None 
                ? EmptyGameObjectsArray 
                : IterateIntersectionsToHighlight(intersectionsToHighlight).ToArray();
        }

        private IEnumerable<GameObject> IterateIntersectionsToHighlight(Options.GroundMode intersectionsToHighlight)
        {
            var nm = Singleton<NetManager>.instance;
            for (ushort i = 0; i < nm.m_nodes.m_size; i++)
            {
                var node = nm.m_nodes.m_buffer[i];

                //test for highlighting
                if (node.m_flags.IsFlagSet(Tool.GetNodeIgnoreFlags())
                    || !node.m_flags.IsFlagSet(Tool.GetNodeIncludeFlags())
                    || !ToggleTrafficLightsTool.IsValidRoadNode(node)
                   )
                {
                    continue;
                }

                var isUnderground = node.Info.m_netAI.IsUnderground();
                if ((isUnderground && intersectionsToHighlight.IsFlagSet(Options.GroundMode.Underground)
                     && Options.ToggleTrafficLightsTool.GroundMode.Value.IsFlagSet(Options.GroundMode.Underground))
                    ||
                    (!isUnderground && intersectionsToHighlight.IsFlagSet(Options.GroundMode.Overground)
                     && Options.ToggleTrafficLightsTool.GroundMode.Value.IsFlagSet(Options.GroundMode.Overground)))
                {
                    yield return CreateGameObject(node.m_position, isUnderground, ToggleTrafficLightsTool.HasTrafficLights(node.m_flags));
                }
            }
        }

        private GameObject CreateGameObject(Vector3 position, bool isUnderground, bool hasTrafficLights)
        {
            var material = GetMaterial(isUnderground, hasTrafficLights);

            var go = new GameObject("IntersectionHighlighting");

            var meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = ShadowCastingMode.Off;

            var filter = go.AddComponent<MeshFilter>();
            filter.mesh = _cylinderMesh;

            go.transform.localPosition = position;

            go.GetComponent<Renderer>().material = material;

            return go;
        }

        private Material GetMaterial(bool isUnderground, bool hasTrafficLights)
        {
            return isUnderground
                ? (hasTrafficLights ? _hasTrafficLightsUndergroundMaterial : _hasNoTrafficLightsUndergroundMaterial)
                : (hasTrafficLights ? _hasTrafficLightsOvergroundMaterial : _hasNoTrafficLightsOvergroundMaterial);
        }
        #endregion

        public void OnTrafficLightsToggled(int nodeId)
        {
            if (_cylindersToHighlight == null)
            {
                return;
            }

            var node = ToggleTrafficLightsTool.GetNetNode(nodeId);
            var position = node.m_position;

            var go = _cylindersToHighlight.SingleOrDefault(g => g.transform.localPosition == position);
            if (go != null)
            {
                var material = GetMaterial(node.Info.m_netAI.IsUnderground(), ToggleTrafficLightsTool.HasTrafficLights(node.m_flags));
                go.GetComponent<Renderer>().material = material;
            }
        }
    }
}