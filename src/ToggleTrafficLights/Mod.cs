using System.Reflection;
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
    }
}