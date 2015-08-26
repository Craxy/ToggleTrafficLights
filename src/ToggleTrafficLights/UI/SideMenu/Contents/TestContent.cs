using ColossalFramework.UI;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Contents
{
    public class TestContent : SideMenuContent
    {
        #region Overrides of UIComponent

        public override void Awake()
        {
            base.Awake();

            var lbl = AddUIComponent<UILabel>();
            lbl.text = "Hallo Welt";
            lbl.textScale = 2.0f;

            lbl.relativePosition = new Vector3(10.0f, 10.0f);
        }

        #endregion

        #region Overrides of SideMenuContent

        public override string Title { get; } = "Test";

        #endregion
    }
}