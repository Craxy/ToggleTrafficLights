using System;
using System.Collections.Generic;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components
{
    // inspired from CityInfoPanel
    public class UICaption : UIPanel
    {
        #region controls

        // original has textbox
        private UILabel _title;
        private UIButton _close;
        private UIDragHandle _drag;
        protected readonly ICollection<Action> OnSizeChangedActions = new List<Action>();
        protected void DoOnSizeChanged(Action action)
        {
            OnSizeChangedActions.Add(action);
        }
        #endregion

        #region properties

        public string Title
        {
            get { return _title.text; }
            set { _title.text = value; }
        }

        public bool ShowCloseButton
        {
            get { return _close.isVisible; }
            set
            {
                if (_close.isVisible != value)
                {
                    _close.isVisible = value;
                    OnSizeChanged();
                }
            }
        }

        #endregion

        #region Overrides of UIComponent

        public override void Awake()
        {
            base.Awake();

            _drag = AddUIComponent<UIDragHandle>();
            _title = AddUIComponent<UILabel>();
            _close = AddUIComponent<UIButton>();

            name = "Caption";
            height = 40.0f;

            _close.isVisible = true;

            //no access to parent
        }

        #region Overrides of UIComponent

        public override void Start()
        {
            base.Start();

            DoOnSizeChanged(() => width = parent.width);
            relativePosition = Vector3.zero;

            _drag.name = "Drag";
            _drag.target = parent;
            DoOnSizeChanged(() => _drag.width = _close.isVisible ? width - 40 : width);
            _drag.height = height;
            _drag.relativePosition = Vector3.zero;

            _title.name = "Title";
            _title.textScale = 1.2f;
            _title.relativePosition = new Vector3(10.0f, 13.0f);

            _close.name = "Close";
            _close.size = new Vector2(32.0f, 32.0f);
            DoOnSizeChanged(() => _close.relativePosition = new Vector3(width - _close.width - 1.0f, 2.0f));
            _close.normalBgSprite = "buttonclose";
            _close.hoveredBgSprite = "buttonclosehover";
            _close.pressedBgSprite = "buttonclosepressed";
            _close.eventClick += OnCloseClicked;

            OnSizeChanged();
        }

        #endregion

        public override void OnDestroy()
        {
            base.OnDestroy();

            UIControls.DestroyAllComponents(this);
        }

        protected override void OnSizeChanged()
        {
            base.OnSizeChanged();

            foreach (var action in OnSizeChangedActions)
            {
                action();
            }
        }

        #endregion

        #region events

        public event MouseEventHandler CloseClicked;
        protected virtual void OnCloseClicked(UIComponent component, UIMouseEventParameter eventparam)
        {
            var handler = CloseClicked;
            if (handler != null)
            {
                handler(component, eventparam);
            }
        }

        #endregion
    }
}