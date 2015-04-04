using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
    public static class KeyHelper
    {
        public static bool IsToolKeyPressed()
        {
            return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                   && !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                   && Input.GetKeyDown(KeyCode.T);
        }

        public static bool IsInvisibleToolKeyPressed()
        {
            return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                   && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                   && Input.GetKeyDown(KeyCode.T);
        }
    }
}