﻿using System;
using System.Diagnostics;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.Behaviours
{
  public sealed class JunctionSettingsBehaviour : IDisposable
  {
    public JunctionSettingsBehaviour()
    {
      Add();
    }

    public void Dispose()
    {
      Remove();
    }

    private void Add()
    {
      _junctionSettings = TrafficRoutesInfoViewPanel.instance.Find<UIPanel>("JunctionSettings");
      AddButtons();
    }

    private void Remove()
    {
      RemoveButtons();
      _junctionSettings = null;
    }

    private UIPanel _junctionSettings;
    private Vector2 _originalPanelLightsPosition;
    private Vector2 _originalPanelStopSignsPosition;
    private const string PanelTtlName = "PanelTTL";

    private void AddButtons()
    {
      // traffic lights are hooked with add, remove, reset all lights
      // under the traffic lights buttons to add, remove, reset all lights
      // todo: implement: always remove lights

      //move PanelLights slightly up
      // default: 46,42
      _originalPanelLightsPosition =
        UpdateRelativePosition(_junctionSettings.Find<UIPanel>("PanelLights"), newY: 25.0f);
      //move PanelStopSigns slighly down
      // default: 46,90
      _originalPanelStopSignsPosition =
        UpdateRelativePosition(_junctionSettings.Find<UIPanel>("PanelStopSigns"), newY: 110.0f);

      // register click events for Traffic Lights icons
      UpdateIcons(_junctionSettings.Find<UIPanel>("PanelLights"), Mode.Add);

      // add buttons under PanelLights panel to change all lights (same behaviour as icons)
      AddToggleTrafficLightsButtons(_junctionSettings, PanelTtlName);
    }

    private void RemoveButtons()
    {
      //move PanelLights to original position
      UpdateRelativePosition(_junctionSettings.Find<UIPanel>("PanelLights"), _originalPanelLightsPosition);

      //move PanelStopSigns to original position
      UpdateRelativePosition(_junctionSettings.Find<UIPanel>("PanelStopSigns"), _originalPanelStopSignsPosition);

      // unregister click events for Traffic Lights icons
      UpdateIcons(_junctionSettings.Find<UIPanel>("PanelLights"), Mode.Remove);

      // remove buttons under PanelLights panel to change all lights (same behaviour as icons)
      RemoveToggleTrafficLightsButtons(_junctionSettings, PanelTtlName);
    }

    private void RemoveToggleTrafficLightsButtons(UIPanel parent, string name)
    {
      var pnl = parent.Find<UIPanel>(name);
      parent.RemoveUIComponent(pnl);
      GameObject.Destroy(pnl.gameObject);
    }

    private enum Mode : byte
    {
      Add,
      Remove,
    }

    private static void UpdateIcons(UIPanel panelLights, Mode mode)
    {
      string GetTooltip(string tooltip)
      {
        return mode == Mode.Add ? tooltip : "";
      }

      void UpdateClick(UIComponent comp, MouseEventHandler handler)
      {
        if (mode == Mode.Add)
        {
          comp.eventClick += handler;
        }
        else
        {
          comp.eventClick -= handler;
        }
      }

      foreach (var c in panelLights.components)
      {
        switch (c)
        {
          case UISprite sprite:
            switch (sprite.spriteName)
            {
              case "IconJunctionTrafficLights":
                sprite.tooltip = GetTooltip("Ctrl + Left Click: Add traffic lights to all junctions.");
                UpdateClick(sprite, AddAllTrafficLightsOnControlPressed);
                break;
              case "IconJunctionNoTrafficLights":
                sprite.tooltip = GetTooltip("Ctrl + Left Click: Remove traffic lights from all junctions.");
                UpdateClick(sprite, RemoveAllTrafficLightsOnControlPressed);
                break;
              default:
                break;
            }
            break;
          case UILabel label:
            switch (label.localeID)
            {
              case "INFO_TRAFFICROUTES_TOGGLELIGHTS":
                label.tooltip = GetTooltip("Ctrl + Left Click: Reset all traffic lights.");
                UpdateClick(label, ResetAllTrafficLightsOnControlPressed);
                break;
              default:
                break;
            }
            break;
          default:
            break;
        }
      }
    }

    private UIPanel AddToggleTrafficLightsButtons(UIPanel parent, string name)
    {
      var pnl = parent.AddUIComponent<UIPanel>();
      pnl.name = name;
      pnl.relativePosition = new Vector3(46.0f, 55.0f);
//      pnl.relativePosition = new Vector3(85.0f, 10.0f);
      pnl.height = 20 * 3 + 2 * 3;
      pnl.width = 300;
      pnl.autoLayout = true;
      pnl.autoLayoutDirection = LayoutDirection.Vertical;
      pnl.autoLayoutPadding = new RectOffset(0, 0, 1, 1);
      pnl.autoLayoutStart = LayoutStart.TopRight;

      CreateButton(pnl, "ResetAll", "Reset all", "Reset all traffic lights.", ResetAllTrafficLights);
      CreateButton(pnl, "RemoveAll", "Remove all", "Remove traffic lights from all junctions.", RemoveAllTrafficLights);
      CreateButton(pnl, "AddAll", "Add all", "Add traffic lights to all junctions.", AddAllTrafficLights);

      return pnl;
    }

    private UIButton CreateButton(UIPanel parent, string name, string text, string tooltip, MouseEventHandler onClick)
    {
      //Template: PublicTransportInfoViewPanel.LinesOverview.LinesOverview

      var btn = parent.AddUIComponent<UIButton>();
      btn.name = name;

      btn.height = 19.5f;
      btn.width = 120.0f;
      btn.disabledBgSprite = "ButtonMenuDisabled";
      btn.disabledColor = new Color32(153, 153, 153, 255);
      btn.hoveredBgSprite = "ButtonMenuHovered";
      btn.hoveredTextColor = new Color32(7, 132, 255, 255);
      btn.normalBgSprite = "ButtonMenu";
      btn.outlineColor = new Color32(0, 0, 0, 255);
      btn.pressedBgSprite = "ButtonMenuPressed";
      btn.pressedTextColor = new Color32(30, 30, 44, 255);
      btn.textPadding = new RectOffset(10, 10, 4, 2);
      btn.textScale = 0.75f;
      btn.font = UIDynamicFont.FindByName("OpenSans-Regular");

      btn.text = text;
      btn.tooltip = tooltip;
      btn.eventClick += onClick;

      return btn;
    }

    private static Vector2 UpdateRelativePosition(UIPanel pnl, float? newX = null, float? newY = null)
    {
      var newPos = (Vector2) pnl.relativePosition;
      if (newX.HasValue)
      {
        newPos.x = newX.Value;
      }
      if (newY.HasValue)
      {
        newPos.y = newY.Value;
      }
      return UpdateRelativePosition(pnl, newPos);
    }

    private static Vector2 UpdateRelativePosition(UIPanel pnl, Vector2 newPosition)
    {
      var pos = pnl.relativePosition;
      pnl.relativePosition = newPosition;
      return pos;
    }

    #region Change Lights

    private static float _lastChangeTime = 0;
    private const float MinTimeBetweenChanges = 0.5f; // in seconds

    private static void AddAllTrafficLightsOnControlPressed(UIComponent _, UIMouseEventParameter evt) 
      => HandleClick(evt, TrafficLights.ChangeMode.Add, true);
    private static void RemoveAllTrafficLightsOnControlPressed(UIComponent _, UIMouseEventParameter evt) 
      => HandleClick(evt, TrafficLights.ChangeMode.Remove, true);
    private static void ResetAllTrafficLightsOnControlPressed(UIComponent _, UIMouseEventParameter evt) 
      => HandleClick(evt, TrafficLights.ChangeMode.Reset, true);
    private static void AddAllTrafficLights(UIComponent _, UIMouseEventParameter evt) 
      => HandleClick(evt, TrafficLights.ChangeMode.Add, false);
    private static void RemoveAllTrafficLights(UIComponent _, UIMouseEventParameter evt) 
      => HandleClick(evt, TrafficLights.ChangeMode.Remove, false);
    private static void ResetAllTrafficLights(UIComponent _, UIMouseEventParameter evt) 
      => HandleClick(evt, TrafficLights.ChangeMode.Reset, false);
    private static void HandleClick(UIMouseEventParameter evt, TrafficLights.ChangeMode mode, bool requiresControl)
    {
      if (evt.buttons != UIMouseButton.Left)
      {
        return;
      }
      if (requiresControl && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
      {
        return;
      }

      var currentTime = Time.time;
      if (currentTime - _lastChangeTime >= MinTimeBetweenChanges)
      {
        _lastChangeTime = currentTime;
      }
      else
      {
        //not enough time passed
        DebugLog.Info("Not enough time passed for another update");
        return;
      }

      DebugLog.Info($"Change all intersections: {mode} all");
      TrafficLights.ChangeAllFast(mode);
//      var stats = TrafficLights.ChangeAll(mode);
      //todo: handle stats?
    }

    private static void TimeChange(TrafficLights.ChangeMode mode)
    {
      var numberOfTests = 1000;
      TrafficLights.ChangeAll(mode);
      var sw = new Stopwatch();
      sw.Start();
      for (var i = 0; i < numberOfTests; i++)
      {
        TrafficLights.ChangeAll(mode);
      }
      sw.Stop();
      var stats = sw.Elapsed;
      sw.Reset();
      sw.Start();
      for (var i = 0; i < numberOfTests; i++)
      {
        TrafficLights.ChangeAllFast(mode);
      }
      sw.Stop();
      var fast = sw.Elapsed;

      DebugLog.Info($"Stats: {stats}, Fast: {fast}");

      //Add: (small map, 1000 iterations)
      //  Stats: 00:00:47.1062748
      //  Fast : 00:00:02.5574887
    }

    #endregion Change Lights
  }
}