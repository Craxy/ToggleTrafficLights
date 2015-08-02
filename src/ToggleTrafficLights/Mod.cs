using System.Globalization;
using System.Reflection;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;

namespace Craxy.CitiesSkylines.ToggleTrafficLights
{
    public sealed class Mod : IUserMod
    {
        public string Name
        {
            get { return "Toggle Traffic Lights"; }
        }

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

        // called (AND):
        //  - loading main menu
        //  - loading level: before deserializing (custom) data
        public void OnSettingsUI(UIHelperBase helper)
        {
            SettingsUi.Create(helper);
        }
    }
}