using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Contents;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using JetBrains.Annotations;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu
{
    public class SideMenu : MonoBehaviour
    {
        #region controls

        private UIPanel _root;
        private UITabstrip _tabstrip;
//        private UIScrollablePanel _contentPanel;
        private UITabContainer _container;

        #region tabs

        public UIPanel AddTab(string title)
        {

            //use policies panel as template
            var ts = ToolsModifierControl.policiesPanel.Find("Tabstrip") as UITabstrip;
            Debug.Assert(ts != null, "ts != null");
            var template = ts.tabs.FirstOrDefault(t => t is UIButton) as UIButton;

            var btn = _tabstrip.AddTab(title, template, true);
            // update with of tab buttons
            {
                var width = _tabstrip.width/ ((float)_tabstrip.tabCount);
                _tabstrip.tabs.ForEach(t => t.width = width);
            }

            var page = _tabstrip.tabPages.components[_tabstrip.tabPages.childCount - 1];
            page.relativePosition = new Vector3(0.0f, 0.0f);

            //todo: scrollable panel
            var pnl = page.AddUIComponent<UIPanel>();
            pnl.name = "Content";
            pnl.relativePosition = new Vector3(5.0f, 5.0f);
            pnl.size = new Vector2(page.size.x - 10.0f, page.size.y - 10.0f);
            return pnl;
        }
        #endregion

        #endregion

        #region position

        private const float Width = 383.0f;
        private const float Top = 62.0f;
        private const float Bottom = 0.0f;
        private const float Left = 0.0f;

        private void UpdatePositionAndSize()
        {
            if (_root == null)
            {
                return;
            }

            var parent = GetParent();
            if (parent == null)
            {
                DebugLog.Info("No Parent");
                return;
            }

            // todo: why -Top necessary?
            _root.position = new Vector3(Left, -1.0f * Top);
            _root.size = new Vector2(Width, parent.height - Top - Bottom);
        }

        private UIComponent GetParent()
        {
            return GetFullScreenContainer();
        }
        private static UIComponent GetFullScreenContainer()
        {
            return UIView.Find("FullScreenContainer");
        }
        #endregion

        #region MonoBehaviour

        [UsedImplicitly]
        private void Awake()
        {
            name = "ToggleTrafficLightsSideMenu";

            //add to FullScreenContainer
            var parent = GetParent();
            {
                _root = parent.AddUIComponent<UIPanel>();
                _root.name = "RootPanel";

//                _root.backgroundSprite = "UnlockingPanel2";
                _root.backgroundSprite = "PoliciesBubble";
                _root.color = new Color32(11, 25, 35, 255);

                UpdatePositionAndSize();
            }

            const float marginLeft = 7.0f;
            const float marginRight = 7.0f;
            const float marginTop = 11.0f;
            const float marginBottom = 7.0f;

            var fromTop = marginTop;
            // caption
            {
                var caption = _root.AddUIComponent<UILabel>();
                caption.name = "Caption";

                caption.text = "Toggle Traffic Lights";
                caption.textAlignment = UIHorizontalAlignment.Center;

                caption.relativePosition = new Vector3(marginLeft, fromTop);
                caption.autoSize = false;
                caption.autoHeight = true;
                caption.width = Width - marginLeft - marginRight;

                fromTop += caption.height;
            }
            fromTop += marginTop * 1.5f;
            // tabs
            {
                _tabstrip = _root.AddUIComponent<UITabstrip>();
                _tabstrip.name = "Menu";

                _tabstrip.relativePosition = new Vector3(marginLeft, fromTop);
                _tabstrip.width = Width - marginLeft - marginRight;

                fromTop += _tabstrip.height;
            }

            fromTop += marginTop;
            // content container
            {
                _container = _root.AddUIComponent<UITabContainer>();
                _container.name = "ContentContainer";

//                _container.backgroundSprite = "UnlockingPanel2";
                _container.backgroundSprite = "GenericPanel";
                _container.color = new Color32(91, 97, 106, 255);

                _container.relativePosition = new Vector3(marginLeft, fromTop);
                _container.size = new Vector2(_root.width - marginLeft - marginRight, _root.height - fromTop - marginBottom);

                // there are two properties that return the m_TabContainer: UITabstrip.tabContainer and UITabstrip.tabPages
                //      but only tabPages has a setter -- and it wants a UITabContainer
                //      because LOGIC...
                _tabstrip.tabPages = _container;
            }

            //            fromTop +=  marginTop;
            //            // content
            //            {
            //                _contentPanel = _root.AddUIComponent<UIScrollablePanel>();
            //                _contentPanel.name = "ContentPanel";
            //
            //                //todo: remove
            //                _contentPanel.backgroundSprite = "UnlockingPanel2";
            //
            //                _contentPanel.relativePosition = new Vector3(marginLeft, fromTop);
            //                _contentPanel.size = new Vector2(_root.width - marginLeft - marginRight, _root.height - fromTop  - marginBottom);
            //            }

            // add tabs
            {
                {
                    var page = AddTab("Batch cmds");
                    page.AddUIComponent<BatchTab>();
                }

                {
                    var page = AddTab("Usage");
                    page.AddUIComponent<UsageTab>();
                }

                {
                    var page = AddTab("Options");
                    page.AddUIComponent<OptionsTab>();
                }
            }

            _tabstrip.selectedIndex = 0;
            {
                var i = 0;
                foreach (var p in _tabstrip.tabPages.components)
                {
                    p.isVisible = i == _tabstrip.selectedIndex;

                    i++;
                }
            }

        }

        [UsedImplicitly]
        private void Start()
        {
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            GetParent()?.RemoveUIComponent(_root);
            _root?.DestroyAllComponents();
            Destroy(_root);

            _root = null;
        }

        [UsedImplicitly]
        private void OnEnable()
        {
            _root.isVisible = true;
        }

        [UsedImplicitly]
        private void OnDisable()
        {
            _root.isVisible = false;
        }
        #endregion
    }
}