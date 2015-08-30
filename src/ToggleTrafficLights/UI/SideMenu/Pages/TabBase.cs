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
        public virtual ITabSettings Settings => DefaultSettings;
        #endregion

        protected UIPanel Content { get; private set; }

        #region MonoBehaviour
        public override void Start()
        {
            base.Start();

            name = "TabPage";

            relativePosition = new Vector3(0.0f, 0.0f);
            size = parent.size;

            Content = AddUIComponent<UIPanel>();
            relativePosition = new Vector3();
        }
        #endregion
    }
}