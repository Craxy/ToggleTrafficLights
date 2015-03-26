using System;
using System.Reflection;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
    public class Loading : LoadingExtensionBase
    {
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            //loading.currentMode can not be used, because it might not be set...
//            Configuration.CurrentMode = loading.currentMode;

            DebugLog.Message("Created v.{0} at {1}", Assembly.GetExecutingAssembly().GetName().Version, DateTime.Now);
        }

        public override void OnReleased()
        {
            base.OnReleased();

            DebugLog.Message("Released v.{0}", Assembly.GetExecutingAssembly().GetName().Version);
        }

        private SelectToolButton _selectToolButton = null;

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);


            if (managers.loading.IsGameMode())
            {
                DebugLog.Message("Level loaded");
                
//                //add button
//                _selectToolButton = new SelectToolButton();
//                _selectToolButton.Initialize();
            }
            else
            {
                DebugLog.Message("In Editor -> mod is disabled");
            }
        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            if (_selectToolButton != null)
            {
                _selectToolButton.Destroy();
            }

            DebugLog.Message("Level unloaded");
        }
    }
}