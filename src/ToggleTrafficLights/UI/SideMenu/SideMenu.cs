using System.Collections.Generic;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components;
using Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Contents;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using JetBrains.Annotations;
using UnityEngine;

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
        private readonly IList<SideMenuContent> _contents = new List<SideMenuContent>();
        public void AddContent<T>()
            where T : SideMenuContent
        {
            var c = _contentPanel.AddUIComponent<T>();
            c.isVisible = false;
            AddTab(c);
        }

        private void AddTab<T>([NotNull] T content)
            where T : SideMenuContent
        {
            _contents.Add(content);

            var btn = _tabstrip.AddTab(content.Title);
            //todo: setup
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

            DebugLog.Info("ParentHeight: {0}", parent.height);
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

                _tabstrip.eventSelectedIndexChanged += OnSelectedMenuItemChanged;

                // container
                {
                    _container = _root.AddUIComponent<UITabContainer>();
                    _container.name = "ContentContainer";

                    _container.backgroundSprite = "UnlockingPanel2";

                    _container.relativePosition = new Vector3(marginLeft, fromTop);
                    _container.size = new Vector2(_root.width - marginLeft - marginRight, _root.height - fromTop  - marginBottom);

                    _tabstrip.tabContainer
                }

                fromTop += _tabstrip.height;
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
                AddContent<TestContent>();
                AddContent<TestContent>();
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

        #region menu
        private void OnSelectedMenuItemChanged(UIComponent component, int value)
        {
            OnSelectedMenuItemChanged(_contents[value]);
        }
        private void OnSelectedMenuItemChanged([NotNull] SideMenuContent selectedContent)
        {
            DebugLog.Info("Selected: {0}", selectedContent.Title);
        }
        #endregion
    }
}