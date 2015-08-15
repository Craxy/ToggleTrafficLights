using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components
{
    public abstract class UIWindow : UIPanel
    {
        #region controls

        private UICaption _caption;
        #endregion

        #region properties

        public UICaption Caption
        {
            get { return _caption; }
        }

        public string Title
        {
            get { return _caption.Title; }
            set { _caption.Title = value; }
        }

        public bool ShowCloseButton
        {
            get { return _caption.ShowCloseButton; }
            set { _caption.ShowCloseButton = value; }
        }
        #endregion

        #region Overrides of UIComponent

        #region Overrides of UIComponent

        public override void Start()
        {
            base.Start();

            backgroundSprite = "UnlockingPanel2";

            width = 100.0f;
            height = 100.0f;

            _caption = AddUIComponent<UICaption>();
            _caption.CloseClicked += (_, __) => OnCloseRequested();
        }

        #endregion

        public override void OnDestroy()
        {
            base.OnDestroy();

            UIControls.DestroyAllComponents(this);
        }

        #endregion

        protected virtual void OnCloseRequested()
        {
            Hide();
        }
    }
}