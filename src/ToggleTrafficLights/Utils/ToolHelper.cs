using System;
using System.Collections.Generic;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
  public class ToolHelper
  {
    private static Dictionary<Type, ToolBase> GetToolsDictionary()
    {
      var tools = ReflectionExtensions.GetNonPublicStaticField<ToolsModifierControl, Dictionary<Type, ToolBase>>("m_Tools");
      return tools;
    }

    public static void AddToToolsModifierControl<T>(T tool)
      where T : ToolBase
    {
      var tools = GetToolsDictionary();
      if (!tools.ContainsKey(typeof(T)))
      {
        tools.Add(typeof(T), tool);
      }
    }

    public static void RemoveFromToolsModifierControl<T>()
      where T : ToolBase
    {
      var tools = GetToolsDictionary();
      if (tools.ContainsKey(typeof(T)))
      {
        tools.Remove(typeof(T));
      }
    }
  }
}
