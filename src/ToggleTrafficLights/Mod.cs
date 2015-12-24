using System.Reflection;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights
{
    public sealed class Mod : IUserMod
    {
        public string Name => "Toggle Traffic Lights";

        public string Description
        {
            get
            {
                return "Remove or add traffic lights at intersections."
#if DEBUG
                    + " v." + Assembly.GetExecutingAssembly().GetName().Version.ToString()
#endif
                ;
            }
        }

        // I'm not using the Settings Windows in C:S: 
        //      ColorFields are quite complicated to create from scratch
        //      and no ColorField to copy is available in Settings Window
//        // called (AND):
//        //  - loading main menu
//        //  - loading level: before deserializing (custom) data
//        public void OnSettingsUI(UIHelperBase helper)
//        {
//            SettingsUi.Create(helper);
//        }
    }
}