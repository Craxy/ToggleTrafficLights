using System;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu
{
    public class OptionsUi : UIWindow
    {
        #region controls

        #endregion

        public static string Name = "ToggleTrafficLightsOptionsUI";

        #region Overrides of UIWindow

        public void DoThrow()
        {
            throw new NotImplementedException("Not yet here...");
        }
        public void ThrowStuff()
        {
            DoThrow();
        }
        public override void Start()
        {
            base.Start();

            const float offsetBetweenGroups = 10.0f;
            name = Name;
            width = 450.0f;
            height = 500.0f;
            relativePosition = new Vector3(Mathf.Floor((((float) GetUIView().fixedWidth) - width) / 2.0f), Mathf.Floor((((float)GetUIView().fixedHeight) - height) / 2.0f));

            canFocus = true;
            isInteractive = true;

            ShowCloseButton = false;
            Title = "ToggleTrafficLights options";


            //save button
            var save = AddUIComponent<UIButton>();
            save.name = "Save";
            save.text = "Save";
            save.size = new Vector2(150.0f, 30.0f);
            save.textScale = 0.9f;
            save.normalBgSprite = "ButtonMenu";
            save.hoveredBgSprite = "ButtonMenuHovered";
            save.pressedBgSprite = "ButtonMenuPressed";
            save.relativePosition = new Vector3(width - save.width - 15.0f, height - save.height - offsetBetweenGroups);
            save.eventClick += (_, __) => OnCloseRequested();

            // background
            var panel = AddUIComponent<UIPanel>();
            panel.name = "Content";
            panel.backgroundSprite = "UnlockingPanel";
            //height: excluding caption (top) and save button (bottom)
            panel.size = new Vector2(width - (4.0f * 2), save.relativePosition.y - offsetBetweenGroups - (Caption.height));
            panel.relativePosition = new Vector3(4.0f, Caption.height);

            //Options
            {
                var left = 5.0f;
                var leftIncrement = 10.0f;
                var topStart = 5.0f;
                var top = topStart;
                var lineHeight = 25.0f;
                var topOffset = lineHeight + 3.0f;

                //highlighting when mouse over
                {
                    var header = panel.AddUIComponent<UILabel>();
                    header.text = "Mouse over: highlighting of current intersection";
                    header.textScale = 1.0f;
                    header.relativePosition = new Vector3(left, top);

                    {
                        top += topOffset;
                        var colorPanelW = panel.AddUIComponent<UIColorPanel>();
                        {
                            colorPanelW.Title = "with lights:";
                            colorPanelW.size = new Vector2(25.0f, lineHeight);
                            colorPanelW.relativePosition = new Vector3(left + leftIncrement, top);
                            colorPanelW.Color = Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value;
                            colorPanelW.ColorChanged += (_, args) => Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value = args.Value;
                            Options.ToggleTrafficLightsTool.HasTrafficLightsColor.ValueChanged += (_, c) => colorPanelW.Color = c;
                        }
                        top += topOffset;
                        var colorPanelWo = panel.AddUIComponent<UIColorPanel>();
                        {
                            colorPanelWo.Title = "without lights:";
                            colorPanelWo.size = new Vector2(25.0f, lineHeight);
                            colorPanelWo.relativePosition = new Vector3(left + leftIncrement, top);
                            colorPanelWo.Color = Options.ToggleTrafficLightsTool.HasNoTrafficLightsColor.Value;
                            colorPanelWo.ColorChanged += (_, args) => Options.ToggleTrafficLightsTool.HasNoTrafficLightsColor.Value = args.Value;
                            Options.ToggleTrafficLightsTool.HasNoTrafficLightsColor.ValueChanged += (_, c) => colorPanelWo.Color = c;
                        }
                        // allign titles
                        var maxWidth = Mathf.Max(colorPanelW.TitleWidth, colorPanelWo.TitleWidth);
                        colorPanelW.CustomTitleWidth = colorPanelWo.CustomTitleWidth = Option.Some(maxWidth);
                    }
                }
                top += topOffset + offsetBetweenGroups;

                //highlighting of intersections
                {
                    var header = panel.AddUIComponent<UILabel>();
                    header.text = "Intersections: highlighting of all traffic lights";
                    header.textScale = 1.0f;
                    header.relativePosition = new Vector3(left, top);

                    // intersections to highlight
                    top += topOffset;
                    {
                        var lbl = panel.AddUIComponent<UILabel>();
                        lbl.text = "intersections to highlight:";
                        lbl.textScale = 0.9f;
                        lbl.relativePosition = new Vector3(left + leftIncrement, top + ((lineHeight - lbl.height) / 2.0f));

                        var cb = UIControls.CreateDropDown(panel);
                        foreach (var value in Enum.GetNames(typeof(Options.GroundMode)))
                        {
                            cb.AddItem(value);
                        }
                        cb.selectedIndex = Array.IndexOf(((Options.GroundMode[]) Enum.GetValues(typeof (Options.GroundMode))), Options.HighlightIntersections.IntersectionsToHighlight.Value);
                        cb.eventSelectedIndexChanged += (_, idx) =>
                        {
                            var val = ((Options.GroundMode[])Enum.GetValues(typeof(Options.GroundMode)))[idx];
                            Options.HighlightIntersections.IntersectionsToHighlight.Value = val;
                        };
                        Options.HighlightIntersections.IntersectionsToHighlight.ValueChanged += (_, mode) => 
                                cb.selectedIndex = Array.IndexOf(((Options.GroundMode[])Enum.GetValues(typeof(Options.GroundMode))), mode);
                        cb.height = lineHeight;
                        cb.relativePosition = new Vector3(lbl.relativePosition.x + lbl.width + 5.0f, top);
                    }

                    // colors
                    {
                        top += topOffset;
                        var colorPanelW = panel.AddUIComponent<UIColorPanel>();
                        {
                            colorPanelW.Title = "with lights:";
                            colorPanelW.size = new Vector2(25.0f, lineHeight);
                            colorPanelW.relativePosition = new Vector3(left + leftIncrement, top);
                            colorPanelW.Color = Options.HighlightIntersections.HasTrafficLightsColor.Value;
                            colorPanelW.ColorChanged += (_, args) => Options.HighlightIntersections.HasTrafficLightsColor.Value = args.Value;
                            Options.HighlightIntersections.HasTrafficLightsColor.ValueChanged += (_, c) => colorPanelW.Color = c;
                        }
                        top += topOffset;
                        var colorPanelWo = panel.AddUIComponent<UIColorPanel>();
                        {
                            colorPanelWo.Title = "without lights:";
                            colorPanelWo.size = new Vector2(25.0f, lineHeight);
                            colorPanelWo.relativePosition = new Vector3(left + leftIncrement, top);
                            colorPanelWo.Color = Options.HighlightIntersections.HasNoTrafficLightsColor.Value;
                            colorPanelWo.ColorChanged += (_, args) => Options.HighlightIntersections.HasNoTrafficLightsColor.Value = args.Value;
                            Options.HighlightIntersections.HasNoTrafficLightsColor.ValueChanged += (_, c) => colorPanelWo.Color = c;
                        }
                        // allign titles
                        var maxWidth = Mathf.Max(colorPanelW.TitleWidth, colorPanelWo.TitleWidth);
                        colorPanelW.CustomTitleWidth = colorPanelWo.CustomTitleWidth = Option.Some(maxWidth);
                    }

                }
            }
        }

        protected override void OnCloseRequested()
        {
            //todo: save
            Hide();

            DoThrow();

        }

        #endregion

    }
}