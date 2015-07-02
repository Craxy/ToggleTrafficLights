using System;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu
{
//    public class SideMenu : MonoBehaviour
//    {
//        #region controls
//
//        public static readonly string Name = "ToggleTrafficLightsSideMenu";
//        private static readonly string RootPanelName = Name + "RootPanel";
//        private UIPanel _rootPanel;
//        private UITabstrip _tabstrip;
//        private UITabContainer _tabContainer;
//
//        private UIPanel[] _tabs;
//        #endregion
//
//        public void CreateControls()
//        {
//
//            var view = UIView.GetAView();
////            var ratio = view.ratio;
//
//            var uiroot = UIView.GetAView().FindUIComponent<UIPanel>("FullScreenContainer");
//
//            _rootPanel = uiroot.AddUIComponent<UIPanel>();
//            _rootPanel.name = RootPanelName;
//            _rootPanel.relativePosition = new Vector3(0.0f, 62.0f);
//            _rootPanel.size = new Vector2(375.0f, view.fixedHeight);
//            uiroot.eventSizeChanged += OnUiRootSizeChanged;
//
//            _tabstrip = _rootPanel.AddUIComponent<UITabstrip>();
//            _tabstrip.name = Name + "Tabstrip";
//
//            _tabContainer = _rootPanel.AddUIComponent<UITabContainer>();
//            _tabContainer.name = Name + "TabContainer";
//
//        }
//
//        #region Setup
//        private void OnUiRootSizeChanged(UIComponent component, Vector2 value)
//        {
//            if(_rootPanel != )
//        }
//        #endregion
//
//        #region MonoBehavior
//        private void Awake()
//        {
//
//        }
//        private void Start()
//        {
//            name = Name;
//
//
//        }
//        private void OnDestroy()
//        {
//            if (_rootPanel != null)
//            {
//                UIControls.DestroyAllComponents(_rootPanel);
//                Destroy(_rootPanel.gameObject);
//            }
//        }
//        private void OnEnable()
//        {
//
//        }
//        private void OnDisable()
//        {
//
//        }
//        #endregion
//
//        #region menus
//
//        public UIPanel AddTab(string name, string title)
//        {
//            return null;
//        }
//        #endregion
//    }
}