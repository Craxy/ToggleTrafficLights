using System;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    public class HandleTool
    {
        #region members
        private UIButton _button = null;
        private ToggleTrafficLightsTool _trafficLightsTool = null;
        private int _previousToolModeSelectedIndex = 0;


        #endregion

        #region properties
        public bool Initialized
        {
            get { return _button != null; }
        }
        public bool Enabled
        {
            get { return _trafficLightsTool != null && _trafficLightsTool.enabled; }
        }
        #endregion

        #region Initialization

        /// <summary>
        /// Call in gameloop untill function returns true as indicator it is initialized.
        /// Necessary, because we need to wait untill RoadsPanel gets opened.
        /// That's pretty strange because for all other panels you can add buttons (and events) right away...but for Roads[Option]Panel....nope....
        /// Additional it's not possible to use UIView.Find.
        /// So I used the code from ExtendedRoadUpgrade https://github.com/viakmaky/Skylines-ExtendedRoadUpgrade (MIT licence)
        /// I have no idea how viakmaky found a working solution...but he did it. Thanks pal!
        /// </summary>
        public bool Initialize()
        {
            if (Initialized)
            {
                DebugLog.Info("Initialize: SelectToolButton already initialized");
                return true;
            }
        }

        #endregion


        #region game loop
        

        #endregion

        #region ToolMode

        private void SetToolMode(ToolMode mode)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        private enum ToolMode
        {
            None,
            Intersection,
        }
        #endregion
    }
}