using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    internal sealed class HandleToolThreading : ThreadingExtensionBase, ILoadingExtension
    {
        #region fields
        private bool _loadingLevel = false;
        private ToolUi _ui = new ToolUi(); 
        #endregion

        #region Implementation of ILoadingExtension
        void ILoadingExtension.OnCreated(ILoading loading)
        {
            throw new System.NotImplementedException();
        }

        void ILoadingExtension.OnLevelLoaded(LoadMode mode)
        {
            _loadingLevel = false;
            DebugLog.Info("OnLevelLoaded, UI visible: {0}", _ui.IsVisible);
        }

        void ILoadingExtension.OnLevelUnloading()
        {
            _ui.DestroyView();
            _loadingLevel = true;
        }
        #endregion

        #region Overrides of ThreadingExtensionBase
        public override void OnCreated(IThreading threading)
        {
            base.OnCreated(threading);
        }

        public override void OnReleased()
        {
            base.OnReleased();

            _ui.Destroy();

            DebugLog.Info("HandleToolThreading: OnReleased");
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            if (_loadingLevel)
            {
                return;
            }

            _ui.OnUpdate();
        }
        #endregion
    }
}