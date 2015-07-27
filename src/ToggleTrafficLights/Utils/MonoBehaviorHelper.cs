using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    public static class MonoBehaviorHelper
    {
        public static void Disable(this MonoBehaviour behaviour)
        {
            if (behaviour != null)
            {
                behaviour.enabled = false;
            }
        }
        public static void Enable(this MonoBehaviour behaviour)
        {
            if (behaviour != null)
            {
                behaviour.enabled = true;
            }
        }
        public static T AddMonoBehaviourDisabled<T>(this GameObject go) where T : MonoBehaviour
        {
            if(go != null)
            {
                var obj = go.AddComponent<T>();
                obj.enabled = false;

                return obj;
            }
            return default(T);
        }
    }
}
