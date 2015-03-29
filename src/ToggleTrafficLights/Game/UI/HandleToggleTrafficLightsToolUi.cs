//using ColossalFramework.UI;
//using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
//using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
//
//namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
//{
//    public class HandleToggleTrafficLightsToolUi
//    {
//        #region members
//        private UIButton _button = null;
//        private ToggleTrafficLightsTool _trafficLightsTool = null;
//        #endregion
//
//        #region properties
//        public bool Initialized
//        {
//            get { return _button != null; }
//        }
//        public bool Enabled
//        {
//            get { return _trafficLightsTool != null && _trafficLightsTool.enabled; }
//        }
//        #endregion
//
//        #region game loops
//        public void OnSimulationStep()
//        {
//            //initialization happens when roadspanel gets opened for the first time
//            if (!Initialized)
//            {
//                if (Initialize())
//                {
//                    DebugLog.Info("HandleToggleTrafficLightsToolUi initialized");
//                }
//                else
//                {
//                    return;
//                }
//            }
//
//            //TODO: handle shortcut
//        }
//
//        #endregion
//
//        #region Initialization
//        public bool Initialize()
//        {
//            if (Initialized)
//            {
//                DebugLog.Info("Initialize: HandleToggleTrafficLightsToolUi already initialized");
//                return true;
//            }
//        }
//        #endregion
//    }
//}