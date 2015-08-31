using System.Collections.Generic;
using System.Linq;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.Components;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages
{
    public abstract class TabBase : UIPanel
    {
        #region Settings
        public static TabSettings DefaultSettings = new TabSettings();
        public ITabSettings Settings { get; set; } = DefaultSettings;

        #endregion


        #region MonoBehaviour
        public override void Start()
        {
            base.Start();

            name = "TabPage";

            relativePosition = new Vector3(0.0f, 0.0f);
            size = parent.size;
        }
        #endregion
    }
}