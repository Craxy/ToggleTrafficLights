using System;
using System.Linq;
using ColossalFramework.UI;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Contents
{
    public class TestContent : UIPanel
    {
        #region Overrides of UIComponent

        public static int _i = 1;

        public override void Start()
        {
            base.Start();

            name = "TestContentPanel";

            relativePosition = new Vector3(0.0f, 0.0f);
            size = parent.size;

            var lbl = AddUIComponent<UILabel>();
            lbl.relativePosition = new Vector3(10.0f, 10.0f);
            lbl.wordWrap = true;
            lbl.maximumSize = new Vector2(width - 20.0f, 0.0f);
            lbl.text = "Hallo Welt " + _i++ + ( string.Join(" ", Enumerable.Repeat("blablabla", 10).ToArray()) );
            lbl.textScale = 1.5f;
        }

        #endregion
    }
}