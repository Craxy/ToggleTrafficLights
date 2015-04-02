using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    public static class KeyHelper
    {
        public static bool IsToolKeyPressed()
        {
            return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) 
                   && Input.GetKeyDown(KeyCode.T);
        }
    }
}