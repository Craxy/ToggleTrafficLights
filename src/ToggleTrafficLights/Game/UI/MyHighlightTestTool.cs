using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    public class MyHighlightTestTool : ToolBase
    {
        #region Activation

        private static ToolBase _previousTool = null;
        private static MyHighlightTestTool _tool = null;
        public static bool ToggleActivated()
        {
            if (_tool == null || ToolsModifierControl.toolController.CurrentTool != _tool)
            {
                _tool = ToolsModifierControl.toolController.gameObject.GetComponent<MyHighlightTestTool>()
                        ?? ToolsModifierControl.toolController.gameObject.AddComponent<MyHighlightTestTool>();

                _previousTool = ToolsModifierControl.toolController.CurrentTool;
                ToolsModifierControl.toolController.CurrentTool = _tool;

                DebugLog.Info("MyHighlightTestTool: Activated");
                return true;
            }
            else
            {
                ToolsModifierControl.toolController.CurrentTool = _previousTool;
                _previousTool = null;
                _tool = null;

                DebugLog.Info("MyHighlightTestTool: Deactivated");
                return false;
            }
        }
        #endregion


        #region fields
        private Vector2 _scrollPosition;
        #endregion

        #region GUI fields
        private readonly ValueParser<float> _diameterMultiplier = ValueParser.Create(Parser.ParseFloat, 1.0f);

        private readonly ValueParser<float>[] _buildingSize = new[]
        {
            ValueParser.Create(Parser.ParseFloat, 1.0f),
            ValueParser.Create(Parser.ParseFloat, 1.0f),
            ValueParser.Create(Parser.ParseFloat, 1.0f),
            ValueParser.Create(Parser.ParseFloat, 1.0f),
        };

        private readonly ValueParser<int> _cylinderSides = ValueParser.Create(Parser.ParseInt, 5);
        #endregion


        #region MonoBehaviour

        protected void Start()
        {
            DebugLog.Info("MyHighlightTestTool: Start");
        }

        protected override void OnDestroy()
        {
            DebugLog.Info("MyHighlightTestTool: OnDestroy");
        }

        protected override void OnEnable()
        {
            DebugLog.Info("MyHighlightTestTool: OnEnable");
        }

        protected override void OnDisable()
        {
            DebugLog.Info("MyHighlightTestTool: OnDisable");
        }

        protected override void OnToolGUI()
        {
            base.OnToolGUI();

            const float left = 0f;
            const float top = 50f;
            const float width = 225f;
            const float height = 510f;
            const float padding = 5f;

            GUILayout.BeginArea(new Rect(left, top, width, height));
            
            var bgTexture = new Texture2D(1, 1);
            bgTexture.SetPixel(0, 0, new Color(0.321f, 0.321f, 0.321f, 1.0f));
            bgTexture.Apply();
            GUI.Box(new Rect(0f, 0f, width, height), bgTexture);
            {
                GUILayout.BeginArea(new Rect(padding, padding, width - 2 * padding, height - 2 * padding));
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            
                GUILayout.Label("<size=15><b>Highlight stuff</b></size>");
            
                GUILayout.Space(10f);

                GUILayout.Label("Cylinder:");
                ValueInput("dia mult", _diameterMultiplier);
//                ValueInput("x", _buildingSize[0]);
//                ValueInput("y", _buildingSize[1]);
//                ValueInput("z", _buildingSize[2]);
//                ValueInput("w", _buildingSize[3]);
                ValueInput("sides", _cylinderSides);
                



                GUILayout.EndScrollView();
                GUILayout.EndArea();
            }

            GUILayout.EndArea();
        }

        private static void ValueInput<TResult>(string title, ValueParser<TResult> vp)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label(title + ":");
            GUILayout.FlexibleSpace();

            {
                Color? pre = null;
                if (!vp.IsValidInput)
                {
                    pre = GUI.color;
                    GUI.color = Color.red;
                }
                vp.Input = GUILayout.TextArea(vp.Input);
                if (pre.HasValue)
                {
                    GUI.color = pre.Value;
                }
            }

            GUILayout.Label(string.Format("({0})", vp.Value));

            GUILayout.EndHorizontal();
        }

//        protected override void OnToolGUI()
//        {
//
//            const float left = 0f;
//            const float top = 50f;
//            const float width = 225f;
//            const float height = 510f;
//            const float padding = 5f;
//
//
//            GUILayout.BeginArea(new Rect(left, top, width, height));
//
//            var bgTexture = new Texture2D(1, 1);
//            bgTexture.SetPixel(0, 0, new Color(0.321f, 0.321f, 0.321f, 1.0f));
//            bgTexture.Apply();
//            GUI.Box(new Rect(0f, 0f, width, height), bgTexture);
//            {
//                GUILayout.BeginArea(new Rect(padding, padding, width - 2 * padding, height - 2 * padding));
//                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
//
//                GUILayout.Label("<size=15><b>Highlight stuff</b></size>");
//
//                GUILayout.Space(10f);
//
//
//
//
//
//                GUILayout.Space(10f);
//
//                GUILayout.Label("<b>Statistics</b>:");
//                GUILayout.Label("Number of");
//                if (_statistics == null || _updateStatisticsCounter++ >= UpdateStatisticsEveryNthUpdate)
//                {
//                    _statistics = Statistics.Collect();
//                    _updateStatisticsCounter = 0;
//                }
//                _statistics.DrawGuiTable();
//
//                GUILayout.Space(10f);
//
//                GUILayout.Label("<b>Batch Commands</b>:");
//                if (GUILayout.Button("Remove all Traffic Lights"))
//                {
//                    RemoveAllTrafficLights();
//                }
//                if (GUILayout.Button("Add all Traffic Lights"))
//                {
//                    AddAllTrafficLights();
//                }
//                if (GUILayout.Button("Reset all to default"))
//                {
//                    ResetAllTrafficLights();
//                }
//
//                GUILayout.Space(5f);
//                if (_changedStatistics != null && _updateChangedStatisticsCounter > 0)
//                {
//                    _changedStatistics.DrawGuiTable();
//                    _updateChangedStatisticsCounter--;
//                }
//
//                GUILayout.EndScrollView();
//                GUILayout.EndArea();
//            }
//
//            GUILayout.EndArea();
//        }


        public override void RenderOverlay(RenderManager.CameraInfo cameraInfo)
        {
            base.RenderOverlay(cameraInfo);

            HighlightAllIntersections();
        }

        #endregion



        #region Highlighting
        private void HighlightAllIntersections()
        {
            var nodes = Singleton<NetManager>.instance.m_nodes.m_buffer
                                .Where(n => !n.m_flags.IsFlagSet(NetNode.Flags.None))
                                .Where(n => n.m_flags.IsFlagSet(NetNode.Flags.Junction))
                                .Where(ToggleTrafficLightsTool.IsValidRoadNode);

            var bm = Singleton<BuildingManager>.instance;


            var material = new Material(bm.m_properties.m_highlightMaterial)
            {
                color = Color.red
            };
            var mesh = MeshHelper.CreateCylinder(_cylinderSides.Value);
            var id = Shader.PropertyToID("_BuildingSize");
//            var id = Shader.PropertyToID("_Highlight1");



            foreach (var node in nodes)
            {
                var info = node.Info;
                

                var d = info.m_halfWidth * 2 * _diameterMultiplier.Value;
                var position = node.m_position;

                //x & z: radius of ellipse (length, width)
                //y & w have no effect (because default shader?)
                material.SetVector(id, new Vector4(d, 0, d, 0));
//                Log.Info("d: {0}", d);
//                material.SetVector(id, new Vector4(_buildingSize[0].Value, _buildingSize[1].Value, _buildingSize[2].Value, _buildingSize[3].Value));
//                material.SetVector(Shader.PropertyToID("_HeightMapping"), node.);

                //shader does not seem to have an effect

                if (material.SetPass(0))
                {
                    ++Singleton<BuildingManager>.instance.m_drawCallData.m_overlayCalls;
                    Graphics.DrawMeshNow(mesh, position, Quaternion.AngleAxis(0, Vector3.down));
                }

            }
        }
        private Mesh CreateHighlightMesh()
        {
            return MeshHelper.CreateCylinder(9);

            
            //4 corners
            Vector3[] vector3Array1 = new Vector3[8];
            int[] numArray1 = new int[24];
            int num1 = 0;
            int num2 = 0;
            Vector3[] vector3Array2 = vector3Array1;
            int index1 = num1;
            int num3 = 1;
            int num4 = index1 + num3;
            vector3Array2[index1] = new Vector3(-0.5f, 0.0f, -0.5f);
            Vector3[] vector3Array3 = vector3Array1;
            int index2 = num4;
            int num5 = 1;
            int num6 = index2 + num5;
            vector3Array3[index2] = new Vector3(-0.5f, 0.0f, 0.5f);
            Vector3[] vector3Array4 = vector3Array1;
            int index3 = num6;
            int num7 = 1;
            int num8 = index3 + num7;
            vector3Array4[index3] = new Vector3(0.5f, 0.0f, 0.5f);
            Vector3[] vector3Array5 = vector3Array1;
            int index4 = num8;
            int num9 = 1;
            int num10 = index4 + num9;
            vector3Array5[index4] = new Vector3(0.5f, 0.0f, -0.5f);
            Vector3[] vector3Array6 = vector3Array1;
            int index5 = num10;
            int num11 = 1;
            int num12 = index5 + num11;
            vector3Array6[index5] = new Vector3(-0.5f, 1f, -0.5f);
            Vector3[] vector3Array7 = vector3Array1;
            int index6 = num12;
            int num13 = 1;
            int num14 = index6 + num13;
            vector3Array7[index6] = new Vector3(-0.5f, 1f, 0.5f);
            Vector3[] vector3Array8 = vector3Array1;
            int index7 = num14;
            int num15 = 1;
            int num16 = index7 + num15;
            vector3Array8[index7] = new Vector3(0.5f, 1f, 0.5f);
            Vector3[] vector3Array9 = vector3Array1;
            int index8 = num16;
            int num17 = 1;
            int num18 = index8 + num17;
            vector3Array9[index8] = new Vector3(0.5f, 1f, -0.5f);
            int[] numArray2 = numArray1;
            int index9 = num2;
            int num19 = 1;
            int num20 = index9 + num19;
            int num21 = num18 - 8;
            numArray2[index9] = num21;
            int[] numArray3 = numArray1;
            int index10 = num20;
            int num22 = 1;
            int num23 = index10 + num22;
            int num24 = num18 - 4;
            numArray3[index10] = num24;
            int[] numArray4 = numArray1;
            int index11 = num23;
            int num25 = 1;
            int num26 = index11 + num25;
            int num27 = num18 - 7;
            numArray4[index11] = num27;
            int[] numArray5 = numArray1;
            int index12 = num26;
            int num28 = 1;
            int num29 = index12 + num28;
            int num30 = num18 - 7;
            numArray5[index12] = num30;
            int[] numArray6 = numArray1;
            int index13 = num29;
            int num31 = 1;
            int num32 = index13 + num31;
            int num33 = num18 - 4;
            numArray6[index13] = num33;
            int[] numArray7 = numArray1;
            int index14 = num32;
            int num34 = 1;
            int num35 = index14 + num34;
            int num36 = num18 - 3;
            numArray7[index14] = num36;
            int[] numArray8 = numArray1;
            int index15 = num35;
            int num37 = 1;
            int num38 = index15 + num37;
            int num39 = num18 - 7;
            numArray8[index15] = num39;
            int[] numArray9 = numArray1;
            int index16 = num38;
            int num40 = 1;
            int num41 = index16 + num40;
            int num42 = num18 - 3;
            numArray9[index16] = num42;
            int[] numArray10 = numArray1;
            int index17 = num41;
            int num43 = 1;
            int num44 = index17 + num43;
            int num45 = num18 - 6;
            numArray10[index17] = num45;
            int[] numArray11 = numArray1;
            int index18 = num44;
            int num46 = 1;
            int num47 = index18 + num46;
            int num48 = num18 - 6;
            numArray11[index18] = num48;
            int[] numArray12 = numArray1;
            int index19 = num47;
            int num49 = 1;
            int num50 = index19 + num49;
            int num51 = num18 - 3;
            numArray12[index19] = num51;
            int[] numArray13 = numArray1;
            int index20 = num50;
            int num52 = 1;
            int num53 = index20 + num52;
            int num54 = num18 - 2;
            numArray13[index20] = num54;
            int[] numArray14 = numArray1;
            int index21 = num53;
            int num55 = 1;
            int num56 = index21 + num55;
            int num57 = num18 - 6;
            numArray14[index21] = num57;
            int[] numArray15 = numArray1;
            int index22 = num56;
            int num58 = 1;
            int num59 = index22 + num58;
            int num60 = num18 - 2;
            numArray15[index22] = num60;
            int[] numArray16 = numArray1;
            int index23 = num59;
            int num61 = 1;
            int num62 = index23 + num61;
            int num63 = num18 - 5;
            numArray16[index23] = num63;
            int[] numArray17 = numArray1;
            int index24 = num62;
            int num64 = 1;
            int num65 = index24 + num64;
            int num66 = num18 - 5;
            numArray17[index24] = num66;
            int[] numArray18 = numArray1;
            int index25 = num65;
            int num67 = 1;
            int num68 = index25 + num67;
            int num69 = num18 - 2;
            numArray18[index25] = num69;
            int[] numArray19 = numArray1;
            int index26 = num68;
            int num70 = 1;
            int num71 = index26 + num70;
            int num72 = num18 - 1;
            numArray19[index26] = num72;
            int[] numArray20 = numArray1;
            int index27 = num71;
            int num73 = 1;
            int num74 = index27 + num73;
            int num75 = num18 - 5;
            numArray20[index27] = num75;
            int[] numArray21 = numArray1;
            int index28 = num74;
            int num76 = 1;
            int num77 = index28 + num76;
            int num78 = num18 - 1;
            numArray21[index28] = num78;
            int[] numArray22 = numArray1;
            int index29 = num77;
            int num79 = 1;
            int num80 = index29 + num79;
            int num81 = num18 - 8;
            numArray22[index29] = num81;
            int[] numArray23 = numArray1;
            int index30 = num80;
            int num82 = 1;
            int num83 = index30 + num82;
            int num84 = num18 - 8;
            numArray23[index30] = num84;
            int[] numArray24 = numArray1;
            int index31 = num83;
            int num85 = 1;
            int num86 = index31 + num85;
            int num87 = num18 - 1;
            numArray24[index31] = num87;
            int[] numArray25 = numArray1;
            int index32 = num86;
            int num88 = 1;
            int num89 = index32 + num88;
            int num90 = num18 - 4;
            numArray25[index32] = num90;
            var mesh = new Mesh
            {
                vertices = vector3Array1,
                triangles = numArray1,
                bounds = new Bounds(new Vector3(0.0f, 256f, 0.0f), new Vector3(128f, 512f, 128f))
            };


            return mesh;
        }
        #endregion

    }
}