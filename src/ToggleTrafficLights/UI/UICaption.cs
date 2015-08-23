using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI
{
    // inspired from CityInfoPanel
    internal sealed class UICaption : UIPanel
    {
        #region controls
        #region title
        private UILabel _uiTitle;
        private void SetTextOnUiTitle([NotNull] string value)
        {
            if (_uiTitle != null)
            {
                _uiTitle.text = value;
            }
        }
        private string _title = "Caption";
        [NotNull]
        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnTitleChanged(value);
                }
            }
        }
        public event Action<string> TitleChanged;
        private void OnTitleChanged([NotNull] string title)
        {
            TitleChanged?.Invoke(title);
        }
        #endregion
        #region close button
        private UIButton _uiClose;
        private void SetIsVisibleOnUiClose(bool visible)
        {
            if (_uiClose != null)
            {
                _uiClose.isVisible = visible;
            }
        }

        private bool _showCloseButton = true;
        public bool ShowCloseButton
        {
            get { return _showCloseButton; }
            set
            {
                if (_showCloseButton != value)
                {
                    _showCloseButton = value;
                    OnShowCloseButtonChanged(value);
                }
            }
        }
        public event Action<bool> ShowCloseButtonChanged;
        private void OnShowCloseButtonChanged(bool showCloseButton)
        {
            ShowCloseButtonChanged?.Invoke(showCloseButton);
        }

        #endregion
        #region drag
        private UIDragHandle _uiDrag;
        private void SetEnabledOnUiDrag(bool enabled)
        {
            if (_uiDrag != null)
            {
                _uiDrag.enabled = enabled;
            }
        }
        private bool _allowDragging = true;
        public bool AllowDragging
        {
            get { return _allowDragging; }
            set
            {
                if (_allowDragging != value)
                {
                    _allowDragging = value;
                    OnAllowDraggingChanged(value);
                }
            }
        }
        public event Action<bool> AllowDraggingChanged;
        private void OnAllowDraggingChanged(bool allowDragging)
        {
            AllowDraggingChanged?.Invoke(allowDragging);
        }
        #endregion

        #region size changed
        private readonly ICollection<Action> _onSizeChangedActions = new List<Action>();
        private void DoOnSizeChanged([NotNull] Action action)
        {
            _onSizeChangedActions.Add(action);
        }

        #endregion

        #region MonoBehaviour

        public override void Awake()
        {
            base.Awake();

            CreateControls();
        }

        public override void OnEnable()
        {
            base.OnEnable();

            // parent is not available at awake
            // and no parent changed event
            relativePosition = Vector3.zero;
            _uiDrag.target = parent;
        }

        #endregion

        #region Initialization

        private void CreateControls()
        {
            name = "Caption";

            height = 40.0f;
            DoOnSizeChanged(() => width = parent.width);
            DoOnSizeChanged(() => relativePosition = Vector3.zero);

            _uiTitle = AddUIComponent<UILabel>();
            {
                _uiTitle.name = "Title";
                _uiTitle.textScale = 1.2f;
                _uiTitle.relativePosition = new Vector3(10.0f, 13.0f);

                SetTextOnUiTitle(Title);
                TitleChanged += SetTextOnUiTitle;
            }

            _uiClose = AddUIComponent<UIButton>();
            {
                _uiClose.name = "CloseButton";
                _uiClose.size = new Vector2(32.0f, 32.0f);

                _uiClose.normalBgSprite = "buttonclose";
                _uiClose.hoveredBgSprite = "buttonclosehover";
                _uiClose.pressedBgSprite = "buttonclosepressed";

                SetIsVisibleOnUiClose(ShowCloseButton);
                ShowCloseButtonChanged += SetIsVisibleOnUiClose;

                _uiClose.eventClick += OnCloseClicked;
            }

            _uiDrag = AddUIComponent<UIDragHandle>();
            {
                _uiDrag.name = "Drag";

                DoOnSizeChanged(() => _uiDrag.target = parent);
                DoOnSizeChanged(() => _uiDrag.width = ShowCloseButton ? width - 40 : width);
                DoOnSizeChanged(() => _uiDrag.height = height);


                SetEnabledOnUiDrag(AllowDragging);
                AllowDraggingChanged += SetEnabledOnUiDrag;
            }
        }
        #endregion

        #region Events
        public event MouseEventHandler CloseClicked;
        private void OnCloseClicked([NotNull] UIComponent component, [NotNull] UIMouseEventParameter eventparam)
        {
            var handler = CloseClicked;
            handler?.Invoke(component, eventparam);
        }

        #endregion
    }
}