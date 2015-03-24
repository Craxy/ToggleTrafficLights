using System;
using System.Reflection;
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

        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);


            if (managers.loading.IsGameMode())
            {
                DebugLog.Message("Level loaded");
            }
            else
            {
                DebugLog.Message("In Editor -> mod is disabled");
            }

//            switch (mode)
//            {
//
//                case LoadMode.NewGame:
//                case LoadMode.LoadGame:
//                    Configuration.IsInGame = true;
//                    break;
//                case LoadMode.NewMap:
//                case LoadMode.LoadMap:
//                case LoadMode.NewAsset:
//                case LoadMode.LoadAsset:
//                    Configuration.IsInEditor = true;
//                    break;
//                default:
//                    throw new ArgumentOutOfRangeException("mode");
//            }

        }

        public override void OnLevelUnloading()
        {
            base.OnLevelUnloading();

            DebugLog.Message("Level unloaded");
        }
    }
}