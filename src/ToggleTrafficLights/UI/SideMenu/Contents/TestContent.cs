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
            lbl.text = "Hallo Welt " + _i++;
            lbl.textScale = 2.0f;

            lbl.relativePosition = new Vector3(10.0f, 10.0f);
        }

        #endregion
    }
}