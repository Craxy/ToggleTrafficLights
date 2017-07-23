using System;
using System.Globalization;
using ColossalFramework;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Tools
{
  public class TrafficLightsTool
  {
    public void ChangeAll(TrafficLights.ChangeMode mode, bool showStats = false)
    {
      if (showStats)
      {
        ChangeAllAndShowStats(mode);
      }
      else
      {
        ChangeAllWithoutStats(mode);
      }
    }

    public void ChangeAllAndShowStats(TrafficLights.ChangeMode mode)
    {
      _stats = TrafficLights.ChangeAll(mode);
    }

    public void ChangeAllWithoutStats(TrafficLights.ChangeMode mode)
    {
      TrafficLights.ChangeAllFast(mode);
    }

    public float ShowStatsForNSeconds { get; set; } = 3.0f;
    private float _elapsedSeconds = 0.0f;
    private TrafficLights.ChangedStatistics _stats = null;

    public void DrawStats(float deltaTimeInSeconds)
    {
      if (_stats == null)
      {
        return;
      }

      _elapsedSeconds += deltaTimeInSeconds;
      if (_elapsedSeconds >= ShowStatsForNSeconds)
      {
        _stats = null;
        _elapsedSeconds = 0.0f;
      }
      else
      {
        // draw stats
        GUILayout.BeginArea(new Rect());
      }
    }
  }

  public class TrafficLights
  {
    public enum ChangeMode : byte
    {
      Remove,
      Add,
      Reset,
    }

    public static ChangedStatistics ChangeAll(ChangeMode mode)
    {
      var changes = new ChangedStatistics();

      var netManager = Singleton<NetManager>.instance;
      var nodes = netManager.m_nodes;
      for (ushort i = 0; i < nodes.m_size; i++)
      {
        var node = nodes.m_buffer[i];

        Change(i, ref node, mode, ref changes);

        nodes.m_buffer[i] = node;
      }

      return changes;
    }

    public static void Change(ushort nodeId, ref NetNode node, ChangeMode mode, ref ChangedStatistics stats)
    {
      stats.Action = mode.ToString();

      if (node.m_flags == NetNode.Flags.None)
      {
        return;
      }
      stats.NumberOfNodesNotNone++;
      if (!Node.IsValidIntersection(nodeId, ref node))
      {
        return;
      }

      var ai = node.Info.GetAI() as RoadBaseAI;
      if (ai == null)
      {
        return;
      }

      stats.NumberOfIntersections++;

      var wantLights = ai.WantTrafficLights();
      var hasLights = CitiesHelper.HasTrafficLights(node.m_flags);
      var isCustom = CitiesHelper.IsCustomTrafficLights(node.m_flags);

      bool shouldHasLights;
      switch (mode)
      {
        case ChangeMode.Remove:
          shouldHasLights = false;
          break;
        case ChangeMode.Add:
          shouldHasLights = true;
          break;
        case ChangeMode.Reset:
          shouldHasLights = wantLights;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
      }
      bool shouldBeCustom = mode != ChangeMode.Reset;


      if (wantLights)
      {
        stats.NumberOfWantLights++;
      }
      else
      {
        stats.NumberOfWantNoLights++;
      }
      if (hasLights)
      {
        stats.NumberOfHadLights++;
      }
      else
      {
        stats.NumberOfHadNoLights++;
      }
      if (isCustom)
      {
        stats.NumberOfWasCustom++;
      }
      else
      {
        stats.NumberOfWasNotCustom++;
      }

      if (shouldHasLights)
      {
        stats.NumberOfHasLights++;
      }
      else
      {
        stats.NumberOfHasLights++;
      }
      if (shouldBeCustom)
      {
        stats.NumberOfIsCustom++;
      }
      else
      {
        stats.NumberOfIsNotCustom++;
      }

      // test if should change
      if (hasLights == shouldHasLights && isCustom == shouldBeCustom)
      {
        return;
      }
      stats.NumberOfChanges++;

      if (hasLights != shouldHasLights)
      {
        if (shouldHasLights)
        {
          node.m_flags |= NetNode.Flags.TrafficLights;
          stats.NumberOfAddedLights++;
        }
        else
        {
          node.m_flags &= ~NetNode.Flags.TrafficLights;
          stats.NumberOfRemovedLights++;
        }
      }

      if (isCustom != shouldBeCustom)
      {
        if (shouldBeCustom)
        {
          node.m_flags |= NetNode.Flags.CustomTrafficLights;
          stats.NumberOfAddedCustoms++;
        }
        else
        {
          node.m_flags &= ~NetNode.Flags.CustomTrafficLights;
          stats.NumberOfRemovedCustoms++;
        }
      }
    }

    /// <summary>
    /// Change without the statistics collection stuff
    /// </summary>
    public static void ChangeAllFast(ChangeMode mode)
    {
      var netManager = Singleton<NetManager>.instance;
      var nodes = netManager.m_nodes;
      for (ushort i = 0; i < nodes.m_size; i++)
      {
        var node = nodes.m_buffer[i];
        ChangeFast(i, ref node, mode);
        nodes.m_buffer[i] = node;
      }
    }

    /// <summary>
    /// Change without the statistics collection stuff
    /// </summary>
    public static void ChangeFast(ushort nodeId, ref NetNode node, ChangeMode mode)
    {
      if (!Node.IsValidIntersection(nodeId, ref node))
      {
        return;
      }
      var ai = node.Info.m_netAI as RoadBaseAI;
      if (ai == null)
      {
        return;
      }

      //todo: tests pre set faster than always set?
      switch (mode)
      {
        case ChangeMode.Remove:
          node.m_flags = (node.m_flags | NetNode.Flags.CustomTrafficLights) & ~NetNode.Flags.TrafficLights;
          break;
        case ChangeMode.Add:
          node.m_flags = node.m_flags | NetNode.Flags.CustomTrafficLights | NetNode.Flags.TrafficLights;
          break;
        case ChangeMode.Reset:
          node.m_flags = node.m_flags & ~NetNode.Flags.CustomTrafficLights;
          if (ai.WantTrafficLights())
          {
            node.m_flags = node.m_flags | NetNode.Flags.TrafficLights;
          }
          else
          {
            node.m_flags = node.m_flags & ~NetNode.Flags.TrafficLights;
          }
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
      }
    }

    public sealed class ChangedStatistics
    {
      public string Action = "";
      public int NumberOfNodesNotNone = 0;
      public int NumberOfIntersections = 0;

      public int NumberOfWantLights = 0;
      public int NumberOfWantNoLights = 0;

      public int NumberOfHadLights = 0;
      public int NumberOfHadNoLights = 0;
      public int NumberOfWasCustom = 0;
      public int NumberOfWasNotCustom = 0;

      public int NumberOfHasLights = 0;
      public int NumberOfHasNoLights = 0;
      public int NumberOfIsCustom = 0;
      public int NumberOfIsNotCustom = 0;

      public int NumberOfChanges = 0;
      public int NumberOfAddedLights = 0;
      public int NumberOfRemovedLights = 0;
      public int NumberOfAddedCustoms = 0;
      public int NumberOfRemovedCustoms = 0;

      public void DrawGuiTable()
      {
        if (!string.IsNullOrEmpty(Action))
        {
          GUILayout.Label(ApplyColor($"<b>{Action}</b>"));
        }
        GUILayout.Space(0.5f);
        DrawLine("Lights changes", NumberOfChanges);
        DrawLine("Lights added", NumberOfAddedLights);
        DrawLine("Lights removed", NumberOfRemovedLights);
      }

      public void DrawDebugGuiTable()
      {
        if (!string.IsNullOrEmpty(Action))
        {
          GUILayout.Label(ApplyColor($"<b>{Action}</b>"));
        }
        GUILayout.Space(0.5f);
        DrawLine("Nodes not none", NumberOfNodesNotNone);
        DrawLine("Intersections", NumberOfIntersections);
        DrawLine("Want lights", NumberOfWantLights);
        DrawLine("Want no lights", NumberOfWantNoLights);
        DrawLine("Had lights", NumberOfHadLights);
        DrawLine("Had no lights", NumberOfHadNoLights);
        DrawLine("Was custom", NumberOfWasCustom);
        DrawLine("Was not custom", NumberOfWasNotCustom);
        DrawLine("Has lights", NumberOfHasLights);
        DrawLine("Has no lights", NumberOfHasNoLights);
        DrawLine("Is custom", NumberOfIsCustom);
        DrawLine("Is not custom", NumberOfIsNotCustom);
        DrawLine("Changes", NumberOfChanges);
        DrawLine("Lights added", NumberOfAddedLights);
        DrawLine("Lights removed", NumberOfRemovedLights);
        DrawLine("Customs added", NumberOfAddedCustoms);
        DrawLine("Customs removed", NumberOfRemovedCustoms);
      }

      public static void DrawLine(string title, int value)
      {
        //default font is not monospace
        const float size = 40f;

        GUILayout.BeginHorizontal();
        GUILayout.Space(7f);
        GUILayout.Label(ApplyColor(title));
        GUILayout.FlexibleSpace();
        GUILayout.Label(ApplyColor(":"));
        GUILayout.Label(ApplyColor(value.ToString(CultureInfo.InvariantCulture)), GUILayout.Width(size));
        GUILayout.EndHorizontal();
      }

//      public static void DrawTable(Tuple<string,string>[] entries)
//      {
//        var skin = new GUISy
//      }

      public static string ApplyColor(string text)
      {
        const string color = "grey";
        return $"<color={color}>{text}</color>";
      }
    }
  }
}
