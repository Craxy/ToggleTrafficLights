using System.Globalization;
using System.Reflection;
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
                return "Remove or add traffic lights at intersections. Tool can be selected in the roads menu."
#if DEBUG
                    + " v." + Assembly.GetExecutingAssembly().GetName().Version.ToString()
#endif
                ;
            }
        }

        // do not use option menu since options for elements are extremly limited...
//        public void OnSettingsUI(UIHelperBase helper)
//        {
//        }
    }
}