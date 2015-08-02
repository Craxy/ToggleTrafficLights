using System;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using ICities;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu
{
    public class SettingsUi
    {
        public SettingsUi(UIHelperBase helper)
        {
            Debug.Assert(helper != null);
            UIHelper = helper;

            Setup();
        }

        //todo: how to dispose?
        public static SettingsUi Create(UIHelperBase helper)
        {
            return new SettingsUi(helper);
        }

        private void Setup()
        {
//            {
//                UIScrollablePanel rootPanel = ((UIHelper)UIHelper).self as UIScrollablePanel;
//                rootPanel.autoLayout = false;
//                UITabstrip strip = rootPanel.AddUIComponent<UITabstrip>();
//                // Matching the size of the root panel so no scrolling happens accidentally
//                strip.relativePosition = new Vector3(0, 0);
//                strip.size = new Vector2(744, 40);
//                UITabContainer container = rootPanel.AddUIComponent<UITabContainer>();
//                // Matching the size of the root panel so no scrolling happens accidentally
//                container.relativePosition = new Vector3(0, 40);
//                container.size = new Vector3(744, 713);
//                // Associated the strips to the tabs container
//                strip.tabPages = container;
//                // E.g get some button from the templates and copy the properties in the strip button
//                UIButton template = (UIButton)UITemplateManager.Peek("OptionsButtonTemplate");
//                UIButton newButton = strip.AddTab("test", template, true);
//                newButton.textColor = template.textColor;
//                newButton.pressedTextColor = template.pressedTextColor;
//                newButton.hoveredTextColor = template.hoveredTextColor;
//                newButton.focusedTextColor = template.hoveredTextColor;
//                newButton.disabledTextColor = template.hoveredTextColor;
//                // Get the current container and use the UIHelper to have something in there
//                UIPanel stripRoot = strip.tabContainer.components[0] as UIPanel;
//                stripRoot.autoLayout = true;
//                UIHelper stripHelper = new UIHelper(stripRoot);
//                UIComponent box = stripHelper.AddCheckbox("New check", true, @checked => DebugLog.Info("Check Changed")) as UIComponent;
//                box.FitTo(stripRoot, LayoutDirection.Horizontal);
//                newButton = strip.AddTab("test2");
//                newButton.textColor = template.textColor;
//                newButton.pressedTextColor = template.pressedTextColor;
//                newButton.hoveredTextColor = template.hoveredTextColor;
//                newButton.focusedTextColor = template.hoveredTextColor;
//                newButton.disabledTextColor = template.hoveredTextColor;
//                // And so forth...
//
//                // Force the strip states to reset
//                strip.selectedIndex = -1;
//                // Set the first tab to active
//                strip.selectedIndex = 0;
//            }




//            {
//                var g = UIHelper.AddUIGroup("Mouse over: highlighting of current intersection");
//
//                var btn = g.AddUIButton("Hallo Welt", () => DebugLog.Info("JUHUUUU"));
//
////                g.AddUILabel("Hallo Welt");
//
//

            var g = UIHelper.AddUIGroup("Hallo Welt");

            g.AddUIButton("Hallo Welt", () => DebugLog.Info("LALALALA"));

            var withLights = g.AddUIColorPanel("with lights:", Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value);
            withLights.name = "WithLights";
            withLights.ColorChanged += (_, args) => Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value = args.Value;
            Options.ToggleTrafficLightsTool.HasTrafficLightsColor.ValueChanged += (_, c) => withLights.Color = c;
//                //
//                //                var withoutLights = g.AddUIColorPanel("without lights:", Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value);
//                //                withoutLights.ColorChanged += (_, args) => Options.ToggleTrafficLightsTool.HasTrafficLightsColor.Value = args.Value;
//                //                Options.ToggleTrafficLightsTool.HasTrafficLightsColor.ValueChanged += (_, c) => withoutLights.Color = c;
//
//                //align color pickers
//            }

            g.AddUISpace(50);
            g.AddUILabel("qwekrhjsjkfnjsdbfbnw");
            g.AddUIButton("Ups", () => DebugLog.Info("FOOOOOO"));

        }

        public UIHelperBase UIHelper { get; private set; }

        public UIScrollablePanel Root
        {
            get
            {
                return ((UIHelper)UIHelper).self as UIScrollablePanel;
            }
        }
    }
}