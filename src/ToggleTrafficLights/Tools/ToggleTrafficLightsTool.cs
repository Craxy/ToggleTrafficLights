using System;
using System.Collections.Generic;
using System.Linq;
using ColossalFramework;
using ColossalFramework.Math;
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
                        string.Format("     Elevation: {0}", node.m_elevation),
                        string.Format("   Underground: {0}", node.Info.m_netAI.IsUnderground()),
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
        public override void RenderGeometry(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderGeometry(cameraInfo);

            var nodeId = GetCurrentNetNodeId();

            if (!m_toolController.IsInsideUI && Cursor.visible && IsValidRoadNode(nodeId) && _controlPointSet)
            {
                var node = GetNetNode(nodeId);
                var info = node.Info;

                NetTool.ControlPoint controlPoint = _controlPoint;
                controlPoint.m_direction = Vector3.forward;

                ushort n;
                ushort segment;
                int cost;
                int productionRate;
                NetTool.CreateNode(info, controlPoint, controlPoint, controlPoint, new FastList<NetTool.NodePosition>() {  }, 0, false, true, true, false, false, false, (ushort)0, out n, out segment, out cost, out productionRate);

            }
        }

        private bool _controlPointSet = false;
        private NetTool.ControlPoint _controlPoint;
        public override void SimulationStep()
        {
            base.SimulationStep();

            var nodeId = GetCurrentNetNodeId();

            if (m_mouseRayValid && IsValidRoadNode(nodeId))
            {
                var node = GetNetNode(nodeId);
                var info = node.Info;

                NetTool.ControlPoint p;
                if (NetTool.MakeControlPoint(this.m_mouseRay, this.m_mouseRayLength, info, false, GetNodeIgnoreFlags(),
                    GetSegmentIgnoreFlags(), GetBuildingIgnoreFlags(), node.m_elevation, info.m_netAI.IsUnderground(), out p))
                {
                    _controlPoint = p;
                    _controlPointSet = true;
                }
                else
                {
                    _controlPointSet = false;
                }

            }
            else
            {
                _controlPointSet = false;
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

            //if underground: circle is projected on terrain surface -> draw another circle on road
            //elevation is always >= 0 -- even for underground roads
//            if (node.m_elevation < 0)
            if(info.m_netAI.IsUnderground())
            {
////                p.m_position.y = NetSegment.SampleTerrainHeight(info1, p.m_position, false, p.m_elevation);
//                position.y = position.y - node.m_elevation;

//                ++Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls;
//                Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(cameraInfo, Color.yellow, position, info.m_halfWidth * 2, -1f, 1280f, false, false);

//                var nt = Singleton<NetTool>.instance;
//                nt.CallNonPublicStaticMethod("RenderNode", info, position, Vector3.zero);


//                NetManager instance = Singleton<NetManager>.instance;
//                Quaternion identity = Quaternion.identity;
//                for (int index = 0; index < info.m_nodes.Length; ++index)
//                {
//                    NetInfo.Node n = info.m_nodes[index];
//                    if (n.CheckFlags(NetNode.Flags.None))
//                    {
//                        ++Singleton<ToolManager>.instance.m_drawCallData.m_defaultCalls;
//                        Graphics.DrawMesh(n.m_nodeMesh, position, identity, n.m_nodeMaterial, n.m_layer, null, 0, instance.m_materialBlock);
//                    }
//                }


//                bool isHoveringSegment = false;
//                int segmentIndex;
//                NetInfo newPrefab = null;
//                NetSegment segment;
//                NetTool.ControlPoint startPoint;
//                NetTool.ControlPoint middlePoint;
//                NetTool.ControlPoint endPoint;
//                ushort n;
//                ushort outSegment;
//                int cost;
//                int productionRate;
//                // Initializes colliding arrays
//                ToolErrors errors = NetTool.CreateNode(newPrefab != null ? newPrefab : segment.Info, startPoint, middlePoint, endPoint, 
//                    NetTool.m_nodePositionsSimulation, 1000, true, false, true, false, false, false, 0, out n, out outSegment, out cost, out productionRate);

//                NetTool.RenderOverlay(cameraInfo, , Color.yellow, Color.red);
                


//                m_toolController.RenderColliding(cameraInfo, Color.yellow, Color.red, Color.green, Color.gray, 0, 0);




//                NetTool.ControlPoint p;
//                if (NetTool.MakeControlPoint(m_mouseRay, m_mouseRayLength, info, true, GetNodeIgnoreFlags(), GetSegmentIgnoreFlags(), GetBuildingIgnoreFlags(), node.m_elevation, true, out p))
//                {
//
//                    var startPoint = p;
//                    var middlePoint = p;
//                    var endPoint = p;
//
////                    Singleton<NetTool>.instance.CallNonPublicMethod("RenderOverlay", cameraInfo, info, Color.yellow, p, p, p);
//
//
//                    Singleton<NetTool>.instance.CallNonPublicMethod("RenderOverlay", cameraInfo, info, Color.yellow, startPoint, middlePoint, endPoint);
//                    var arr = new object[] {info, startPoint, middlePoint, endPoint};
//                    if (!Singleton<NetTool>.instance.CallNonPublicStaticFunction<NetTool, bool>("GetSecondaryControlPoints", arr))
//                        return;
//
//                    startPoint = (NetTool.ControlPoint) arr[1];
//                    middlePoint = (NetTool.ControlPoint) arr[2];
//                    endPoint = (NetTool.ControlPoint) arr[3];
//
//
//                    Singleton<NetTool>.instance.CallNonPublicMethod("RenderOverlay", cameraInfo, info, Color.yellow, startPoint, middlePoint, endPoint);
//
//                }


//                GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("UndergroundView");
//                var undergroundCamera = gameObjectWithTag.GetComponent<Camera>();
////                var layerMask = undergroundCamera.cullingMask;
//                ++Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls;
//                Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(undergroundCamera., Color.yellow, position, info.m_halfWidth * 2, -1f, 1280f, false, false);


//                NetTool.ControlPoint p;
//                if (NetTool.MakeControlPoint(m_mouseRay, m_mouseRayLength, info, true, GetNodeIgnoreFlags(),
//                    GetSegmentIgnoreFlags(), GetBuildingIgnoreFlags(), node.m_elevation, true, out p))
//                {
//                    p.m_direction = Vector3.forward;
//                    ushort n;
//                    ushort segment;
//                    int cost;
//                    int productionRate;
//
//                    int num = (int)NetTool.CreateNode(info, p, p, p, NetTool.m_nodePositionsMain, 1000, false, true, true, false, false, false, (ushort)0,
//                                out n, out segment, out cost, out productionRate);
//                }

//                var direction = Vector3.forward;
//                Singleton<NetTool>.instance.CallNonPublicStaticMethod("RenderNode", info, position, direction);

//                NetTool.ControlPoint p;
//                if (NetTool.MakeControlPoint(m_mouseRay, m_mouseRayLength, info, true, GetNodeIgnoreFlags(), GetSegmentIgnoreFlags(), GetBuildingIgnoreFlags(), node.m_elevation, true, out p))
//                {
//                    p.m_elevation = -p.m_elevation;
//
//                    var startPoint = p;
//                    var middlePoint = p;
//                    var endPoint = p;
//
//                    ushort n;
//                    ushort segment;
//                    int cost;
//                    int productionRate;
//
////                    Singleton<NetTool>.instance.CallNonPublicStaticMethod("RenderNode", info, position, Vector3.forward);
//                    NetTool.CreateNode(info, startPoint, middlePoint, endPoint, NetTool.m_nodePositionsMain, 1000, false, true, true, false, false, false, (ushort)0, out n, out segment, out cost, out productionRate);
//
//                    var objs = new object[] {info, startPoint, middlePoint, endPoint,};
//                    if (Singleton<NetTool>.instance.CallNonPublicStaticFunction<NetTool, bool>("GetSecondaryControlPoints", objs))
//                    {
//                        startPoint = (NetTool.ControlPoint) objs[1];
//                        middlePoint = (NetTool.ControlPoint) objs[2];
//                        endPoint = (NetTool.ControlPoint) objs[3];
//
////                        Singleton<NetTool>.instance.CallNonPublicStaticMethod("RenderNode", info, position, Vector3.forward);
//                        NetTool.CreateNode(info, startPoint, middlePoint, endPoint, NetTool.m_nodePositionsMain, 1000, false, true, true, false, false, false, (ushort)0, out n, out segment, out cost, out productionRate);
//                    }
//                }


//                Singleton<NetTool>.instance.CallNonPublicStaticMethod("RenderSegment", info, position, position,  Vector3.forward, Vector3.forward, false, false);

//                Singleton<NetTool>.instance.CallNonPublicStaticMethod("RenderNode", info, position, Vector3.forward);

//                NetTool.ControlPoint p;
//                if (NetTool.MakeControlPoint(m_mouseRay, m_mouseRayLength, info, true, GetNodeIgnoreFlags(),
//                    GetSegmentIgnoreFlags(), GetBuildingIgnoreFlags(), node.m_elevation, true, out p))
//                {
//
//                    DebugLog.Info("ControlPoint");
//
//                    this.m_toolController.RenderCollidingNotifications(cameraInfo, (ushort) 0, (ushort) 0);
//                    NetTool.ControlPoint controlPoint = p;
//                    controlPoint.m_direction = Vector3.forward;
//                    ushort n;
//                    ushort segment;
//                    int cost;
//                    int productionRate;
//                    int num =
//                        (int)
//                            NetTool.CreateNode(info, controlPoint, controlPoint, controlPoint,
//                                NetTool.m_nodePositionsMain, 0, false, true, true, false, false, false, (ushort) 0,
//                                out n, out segment, out cost, out productionRate);
//
//                }




                // both together do highlight a circle..or something like that....
//                RenderNode(info, position, Vector3.back);
//                RenderNode(info, position, Vector3.forward);

//                ++Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls;
//                DrawCircleUnderground(cameraInfo, Color.yellow, position, info.m_halfWidth * 2, -1f, 1280f, false, false, node);

            }

//            {
//                Bezier3 bezier;
//                var segment = Singleton<NetManager>.instance.m_segments.m_buffer[node.m_segment0];
//
//                bezier.a = node.m_position;
//                bezier.d = node.m_position;
//
//                NetSegment.CalculateMiddlePoints(bezier.a, segment.m_startDirection, bezier.d, segment.m_endDirection, false, false, out bezier.b, out bezier.c);
//
//                ++Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls;
//                Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(cameraInfo, color, bezier, info.m_halfWidth * 2f, info.m_halfWidth, info.m_halfWidth, -1f, 1280f, false, false);
//
//            }


//            ++Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls;
//            DrawCircleUnderground(cameraInfo, Color.yellow, position, info.m_halfWidth * 2, -1f, 1280f, false, false, node);

        }

        private static void DrawCircleUnderground(RenderManager.CameraInfo cameraInfo, Color color, Vector3 center,
            float size, float minY, float maxY, bool renderLimits, bool alphaBlend, NetNode node)
        {
            var oe = Singleton<RenderManager>.instance.OverlayEffect;

            //OverlayEffect.DrawCircle
            float num = (float)((double)Vector2.Distance((Vector2)cameraInfo.m_position, (Vector2)center) * (1.0 / 1000.0) + 1.0);
            Vector4 vector1 = new Vector4(center.x, center.z, size * -0.5f, size * 0.5f);
            Vector4 vector2 = !renderLimits ? new Vector4(-100000f, minY, maxY, 100000f) : new Vector4(minY, -100000f, 100000f, maxY);
            Vector3 vector3_1 = center - new Vector3(size * 0.5f, 0.0f, size * 0.5f);
            Vector3 vector3_2 = center + new Vector3(size * 0.5f, 0.0f, size * 0.5f);
            vector3_1.y = Mathf.Min(vector3_1.y, minY);
            vector3_2.y = Mathf.Max(vector3_2.y, maxY);
            Bounds bounds = new Bounds();
            Vector3 vector3_3 = new Vector3(num, num, num);
            bounds.SetMinMax(vector3_1 - vector3_3, vector3_2 + vector3_3);
            if (!bounds.Intersects(cameraInfo.m_bounds))
            {
                return;
            }
            Material material = !alphaBlend ? oe.GetNonPublicField<OverlayEffect, Material>("m_shapeMaterial") : oe.GetNonPublicField<OverlayEffect, Material>("m_shapeMaterialBlend");
            material.color = color.linear;
            material.SetVector(oe.GetNonPublicField<OverlayEffect, int>("ID_Cen terPos"), vector1);
            material.SetVector(oe.GetNonPublicField<OverlayEffect, int>("ID_LimitsY"), vector2);

            //OverlayEffect.DrawEffect
            //this.DrawEffect(cameraInfo, material, 1, bounds);
            GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("UndergroundView");
            var undergroundCamera = gameObjectWithTag.GetComponent<Camera>();
            
            var pass = 1;

            if (!material.SetPass(pass))
                return;

            Matrix4x4 matrix = new Matrix4x4();
//            if (bounds.Intersects(cameraInfo.m_nearBounds))
//            {
//                matrix.SetTRS(cameraInfo.m_position + cameraInfo.m_forward * (cameraInfo.m_near + 1f), cameraInfo.m_rotation, new Vector3(100f, 100f, 1f));
//                DebugLog.Info("A");
//            }
//            else
//            {
//                matrix.SetTRS(bounds.center, Quaternion.identity, bounds.size);
//                DebugLog.Info("B");
//            }
            matrix.SetTRS(bounds.center, Quaternion.identity, bounds.size);


            var mesh = oe.GetNonPublicField<OverlayEffect, Mesh>("m_boxMesh");
            ++Singleton<ToolManager>.instance.m_drawCallData.m_defaultCalls;
            Graphics.DrawMeshNow(mesh, matrix);

//            Graphics.DrawMesh(mesh, matrix, material, node.Info.m_netLayers, undergroundCamera);
//            Graphics.DrawMeshNow(mesh, matrix);

//            Graphics.DrawMesh(mesh, matrix, material, node.Info.m_prefabDataLayer);

//            var info = node.Info;
//            for (int index = 0; index < info.m_nodes.Length; ++index)
//            {
//                NetInfo.Node n = info.m_nodes[index];
//                ++Singleton<ToolManager>.instance.m_drawCallData.m_defaultCalls;
//                Graphics.DrawMesh(mesh, node.m_position, Quaternion.identity, material, n.m_layer, (Camera)null);
//            }

//            Graphics.DrawMesh(mesh, node.m_position, Quaternion.identity, material, node.Info.m_prefabDataLayer, undergroundCamera);



//            if (bounds.Intersects(cameraInfo.m_nearBounds))
//            {
//                if (!material.SetPass(pass))
//                    return;
//                matrix.SetTRS(cameraInfo.m_position + cameraInfo.m_forward * (cameraInfo.m_near + 1f), cameraInfo.m_rotation, new Vector3(100f, 100f, 1f));
//                Graphics.DrawMeshNow(oe.GetNonPublicField<OverlayEffect, Mesh>("m_boxMesh"), matrix);
//
////                Graphics.DrawMesh(oe.GetNonPublicField<OverlayEffect, Mesh>("m_boxMesh"), matrix, material, node.Info.m_netLayers, undergroundCamera);
//
////                Graphics.DrawMesh(node.m_nodeMesh, position, identity, node.m_nodeMaterial, node.m_layer, undergroundCamera, 0, instance.m_materialBlock);
//
//            }
//            else
//            {
//                if (!material.SetPass(pass))
//                    return;
//                Matrix4x4 matrix = new Matrix4x4();
//                matrix.SetTRS(bounds.center, Quaternion.identity, bounds.size);
//                Graphics.DrawMeshNow(oe.GetNonPublicField<OverlayEffect, Mesh>("m_boxMesh"), matrix);
//
////                Graphics.DrawMesh(oe.GetNonPublicField<OverlayEffect, Mesh>("m_boxMesh"), matrix, material, node.Info.m_netLayers, undergroundCamera);
//
//            }
        }

        private static void RenderNode(NetInfo info, Vector3 position, Vector3 direction)
        {
            if (info.m_nodes == null)
                return;
            NetManager instance = Singleton<NetManager>.instance;
            position.y += 0.15f;
            Quaternion identity = Quaternion.identity;
            float vScale = 0.05f;
            Vector3 vector3_1 = new Vector3(direction.z, 0.0f, -direction.x) * info.m_halfWidth;
            Vector3 vector3_2 = position + vector3_1;
            Vector3 vector3_3 = position - vector3_1;
            Vector3 vector3_4 = vector3_3;
            Vector3 vector3_5 = vector3_2;
            float num = Mathf.Min(info.m_halfWidth * 1.333333f, 16f);
            Vector3 vector3_6 = vector3_2 - direction * num;
            Vector3 vector3_7 = vector3_4 - direction * num;
            Vector3 vector3_8 = vector3_3 - direction * num;
            Vector3 vector3_9 = vector3_5 - direction * num;
            Vector3 vector3_10 = vector3_2 + direction * num;
            Vector3 vector3_11 = vector3_4 + direction * num;
            Vector3 vector3_12 = vector3_3 + direction * num;
            Vector3 vector3_13 = vector3_5 + direction * num;
            Matrix4x4 matrix4x4_1 = NetSegment.CalculateControlMatrix(vector3_2, vector3_6, vector3_7, vector3_4, vector3_2, vector3_6, vector3_7, vector3_4, position, vScale);
            Matrix4x4 matrix4x4_2 = NetSegment.CalculateControlMatrix(vector3_3, vector3_12, vector3_13, vector3_5, vector3_3, vector3_12, vector3_13, vector3_5, position, vScale);
            Matrix4x4 matrix4x4_3 = NetSegment.CalculateControlMatrix(vector3_2, vector3_10, vector3_11, vector3_4, vector3_2, vector3_10, vector3_11, vector3_4, position, vScale);
            Matrix4x4 matrix4x4_4 = NetSegment.CalculateControlMatrix(vector3_3, vector3_8, vector3_9, vector3_5, vector3_3, vector3_8, vector3_9, vector3_5, position, vScale);
            matrix4x4_1.SetRow(3, matrix4x4_1.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            matrix4x4_2.SetRow(3, matrix4x4_2.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            matrix4x4_3.SetRow(3, matrix4x4_3.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            matrix4x4_4.SetRow(3, matrix4x4_4.GetRow(3) + new Vector4(0.2f, 0.2f, 0.2f, 0.2f));
            Vector4 vector4_1 = new Vector4(0.5f / info.m_halfWidth, 1f / info.m_segmentLength, (float)(0.5 - (double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5), (float)((double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5));
            Vector4 zero = Vector4.zero;
            zero.w = (float)(((double)matrix4x4_1.m33 + (double)matrix4x4_2.m33 + (double)matrix4x4_3.m33 + (double)matrix4x4_4.m33) * 0.25);
            Vector4 vector4_2 = new Vector4((float)((double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5), 1f, (float)((double)info.m_pavementWidth / (double)info.m_halfWidth * 0.5), 1f);
            instance.m_materialBlock.Clear();
            instance.m_materialBlock.AddMatrix(instance.ID_LeftMatrix, matrix4x4_1);
            instance.m_materialBlock.AddMatrix(instance.ID_RightMatrix, matrix4x4_2);
            instance.m_materialBlock.AddMatrix(instance.ID_LeftMatrixB, matrix4x4_3);
            instance.m_materialBlock.AddMatrix(instance.ID_RightMatrixB, matrix4x4_4);
            instance.m_materialBlock.AddVector(instance.ID_MeshScale, vector4_1);
            instance.m_materialBlock.AddVector(instance.ID_CenterPos, zero);
            instance.m_materialBlock.AddVector(instance.ID_SideScale, vector4_2);
            instance.m_materialBlock.AddColor(instance.ID_Color, Color.yellow);

            GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("UndergroundView");
            var undergroundCamera = gameObjectWithTag.GetComponent<Camera>();
            
            for (int index = 0; index < info.m_nodes.Length; ++index)
            {
                NetInfo.Node node = info.m_nodes[index];
                if (node.CheckFlags(NetNode.Flags.None))
                {
                    ++Singleton<ToolManager>.instance.m_drawCallData.m_defaultCalls;
                    Graphics.DrawMesh(node.m_nodeMesh, position, identity, node.m_nodeMaterial, node.m_layer, undergroundCamera, 0, instance.m_materialBlock);
                }
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