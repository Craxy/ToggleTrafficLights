using System;
using System.Collections.Generic;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils
{
  public static class ToolHelper
  {
    private static Dictionary<Type, ToolBase> GetToolsDictionary()
    {
      var tools = ReflectionExtensions.GetNonPublicStaticField<ToolsModifierControl, Dictionary<Type, ToolBase>>("m_Tools");
      return tools;
    }

    private static void AddToToolsModifierControl<T>(T tool)
      where T : ToolBase
    {
      var tools = GetToolsDictionary();
      if (!tools.ContainsKey(typeof(T)))
      {
        tools.Add(typeof(T), tool);
      }
    }

    private static void RemoveFromToolsModifierControl<T>()
      where T : ToolBase
    {
      var tools = GetToolsDictionary();
      if (tools.ContainsKey(typeof(T)))
      {
        tools.Remove(typeof(T));
      }
    }

    public static T AddTool<T>()
      where T : ToolBase
    {
      var current = ToolsModifierControl.GetCurrentTool<ToolBase>();  //Assert ToolsModifierControl.CollectTools() is called
      var tool = ToolsModifierControl.toolController.gameObject.GetComponent<T>()
              ?? ToolsModifierControl.toolController.gameObject.AddComponent<T>();
      ToolsModifierControl.toolController.CurrentTool = current;

      return tool;
    }

    public static void RemoveTool<T>(T tool)
      where T : ToolBase
    {
      GameObject.Destroy(tool);
    }

    /// <summary>
    /// ToolsModifierControl.GetTool
    /// But instead of searching the tool in ToolsModifierControl.m_Tools,
    /// this method uses ToolsModifierControl.toolController.gameObject.GetComponent<T>
    /// </summary>
    public static T GetTool<T>()
      where T : ToolBase
    {
      return ToolsModifierControl.toolController.GetComponent<T>();
    }

    /// <summary>
    /// ToolsModifierControl.SetTool
    /// But instead of searching the tool in ToolsModifierControl.m_Tools,
    /// this method uses ToolsModifierControl.toolController.gameObject.GetComponent<T>
    /// </summary>
    public static T SetTool<T>()
      where T : ToolBase
    {
      var toolController = ToolsModifierControl.toolController;
      if (toolController == null)
      {
        return null;
      }
      var tool = toolController.GetComponent<T>();
      if (tool == null)
      {
        return null;
      }

      if (!ToolsModifierControl.keepThisWorldInfoPanel)
      {
        WorldInfoPanel.HideAllWorldInfoPanels();
      }
      GameAreaInfoPanel.Hide();
      ToolsModifierControl.keepThisWorldInfoPanel = false;
      if (toolController.CurrentTool != tool)
      {
        toolController.CurrentTool = tool;
      }
      return tool;
    }
  }
}
