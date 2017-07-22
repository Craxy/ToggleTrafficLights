using System;
using ColossalFramework.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
    /// <summary>
    /// Source: https://github.com/viakmaky/Skylines-ExtendedRoadUpgrade/blob/master/ExtendedRoadUpgrade/UIUtils.cs
    /// </summary>
    public static class UiHelper
    {
        [Flags]
        public enum FindOptions
        {
            None = 0,
            NameContains = 1 << 0
        }


        private static UIView _uiRoot = null;

        private static void FindUiRoot()
        {
            _uiRoot = null;
            foreach (var view in Object.FindObjectsOfType<UIView>())
            {
                if (view.transform.parent == null && view.name == "UIView")
                {
                    _uiRoot = view;
                    break;
                }
            }
        }

        public static T FindComponent<T>(string name, UIComponent parent = null, FindOptions options = FindOptions.None) where T : UIComponent
        {
            if (_uiRoot == null)
            {
                FindUiRoot();
                if (_uiRoot == null)
                {
                    return null;
                }
            }

            foreach (var component in Object.FindObjectsOfType<T>())
            {
                bool nameMatches;
                if ((options & FindOptions.NameContains) != 0)
                {
                    nameMatches = component.name.Contains(name);
                }
                else
                {
                    nameMatches = component.name == name;
                }

                if (!nameMatches)
                {
                    continue;
                }

                var parentTransform = parent != null ? parent.transform : _uiRoot.transform;

                var t = component.transform.parent;
                while (t != null && t != parentTransform)
                {
                    t = t.parent;
                }

                if (t == null)
                {
                    continue;
                }

                return component;
            }

            return null;
        }
    }

    public class Layout : IDisposable
    {
        //abusing IDisposable......
        private static readonly Layout _horizontal = new Layout(() => GUILayout.BeginHorizontal(), GUILayout.EndHorizontal);
        private static readonly Layout _vertical = new Layout(() => GUILayout.BeginVertical(), GUILayout.EndVertical);

        public static Layout Horizontal()
        {
            return _horizontal.Begin();
        }
        public static Layout Vertical()
        {
            return _vertical.Begin();
        }

        private readonly Action _begin;
        private readonly Action _end;

        private Layout(Action begin, Action end)
        {
            _begin = begin;
            _end = end;
        }

        private Layout Begin()
        {
            _begin();
            return this;
        }

        private Layout End()
        {
            _end();
            return this;
        }

        #region Implementation of IDisposable
        public void Dispose()
        {
            End();
        }

        #endregion
    }
}