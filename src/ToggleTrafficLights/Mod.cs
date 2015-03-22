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
                return "Remove or add traffic lights at intersections. Use Ctrl+T to enable the tool and mouse click to enable or disable traffic lights."
#if DEBUG
                    + " v." + Assembly.GetExecutingAssembly().GetName().Version
#endif
                ;
            }
        }
    }
}