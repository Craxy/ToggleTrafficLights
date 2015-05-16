using System;
using System.Security.Policy;
using ColossalFramework.Steamworks;
using Craxy.CitiesSkylines.ToggleTrafficLights.ModTools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu.Components
{
    public static class GuiControls
    {
        public static class BufferedTextBox
        {
            private static string _focusedControlBuffer = string.Empty;
            private static string _lastFocusedControl = null;

            private static bool IsFocusedControl(string id)
            {
                return id == GUI.GetNameOfFocusedControl();
            }

            private static bool WasLastFocusedControl(string id)
            {
                return id == _lastFocusedControl;
            }

            public static string Draw(string id, string value, float inputFieldWidth)
            {
                GUI.SetNextControlName(id);
                var isFocused = IsFocusedControl(id);

                //nothing focused
                if (_lastFocusedControl != null && string.IsNullOrEmpty(GUI.GetNameOfFocusedControl()))
                {
                    _lastFocusedControl = null;
                    _focusedControlBuffer = string.Empty;
                }

                //if not focused -> use value
                //if focused: use buffered value
                if (isFocused && !WasLastFocusedControl(id))
                {
                    //focus changed
                    _lastFocusedControl = id;
                    _focusedControlBuffer = value;
                }

                var v = GUILayout.TextField(isFocused ? _focusedControlBuffer : value, GUILayout.Width(inputFieldWidth));

                if (isFocused)
                {
                    _lastFocusedControl = id;
                    _focusedControlBuffer = v;
                }

                return v;
            }
        }

        public static void ColorField(string id, string title, Color value, ColorPicker colorPicker, ColorPicker.OnColorChanged onColorChanged = null, float indent = 0.0f)
        {
            using (Layout.Vertical())
            {
                using (Layout.Horizontal())
                {
                    if (Mathf.Abs(indent) > 0.01f)
                    {
                        GUILayout.Space(indent);
                    }
                    GUILayout.Label(string.Format("{0}:", title));
                }

                using (Layout.Horizontal())
                {
                    GUILayout.Space(indent + 10.0f);

                    var r = (byte) (Mathf.Clamp(value.r*255.0f, Byte.MinValue, Byte.MaxValue));
                    var g = (byte) (Mathf.Clamp(value.g*255.0f, Byte.MinValue, Byte.MaxValue));
                    var b = (byte) (Mathf.Clamp(value.b*255.0f, Byte.MinValue, Byte.MaxValue));
                    var a = (byte) (Mathf.Clamp(value.a*255.0f, Byte.MinValue, Byte.MaxValue));

                    var changed = TryInputField(id + ".r", "r", ref r, Parser.ParseByte, 40f);
                    changed |= TryInputField(id + ".g", "g", ref g, Parser.ParseByte, 40f);
                    changed |= TryInputField(id + ".b", "b", ref b, Parser.ParseByte, 40f);
                    changed |= TryInputField(id + ".a", "a", ref a, Parser.ParseByte, 40f);

                    if (changed)
                    {
                        value.r = Mathf.Clamp01(r/255.0f);
                        value.g = Mathf.Clamp01(g/255.0f);
                        value.b = Mathf.Clamp01(b/255.0f);
                        value.a = Mathf.Clamp01(a/255.0f);
                    }

                    if (onColorChanged != null)
                    {
                        if (changed)
                        {
                            onColorChanged(value);
                        }

                        if (GUILayout.Button("", GUILayout.Width(40)))
                        {
                            colorPicker.SetColor(value, onColorChanged);

                            Vector2 mouse = Input.mousePosition;
                            mouse.y = Screen.height - mouse.y;

                            colorPicker.rect.position = mouse;
                            colorPicker.visible = true;
                        }

                        var lastRect = GUILayoutUtility.GetLastRect();
                        lastRect.x += 4.0f;
                        lastRect.y += 4.0f;
                        lastRect.width -= 8.0f;
                        lastRect.height -= 8.0f;
                        GUI.DrawTexture(lastRect, ColorPicker.GetColorTexture(id, value), ScaleMode.StretchToFill);
                    }
                }
            }
        }

        public static bool TryInputField<T>(string id, string title, ref T value, Func<string, IOption<T>> parse, float textBoxWidth = 100, float indent = 0.0f)
        {
            var changed = false;

            GUILayout.BeginHorizontal();
            {
                if (Mathf.Abs(indent) > 0.01f)
                {
                    GUILayout.Space(indent);
                }
                GUILayout.Label(title);
                GUILayout.FlexibleSpace();

                var sv = value.ToString();
                var res = BufferedTextBox.Draw(id, sv, textBoxWidth);
                if (res != sv)
                {
                    var r = parse(res);
                    if (r.IsSome())
                    {
                        value = r.GetValue();
                        changed = true;
                    }
                }
            }
            GUILayout.EndHorizontal();

            return changed;
        }

        public static void InputField<T>(string id, string title, T value, Action<T> onInputChanged, Func<string, IOption<T>> parse, float textBoxWidth = 100, float indent = 0.0f)
        {
            if (TryInputField(id, title, ref value, parse, textBoxWidth, indent))
            {
                onInputChanged(value);
            }
        }
    }
}