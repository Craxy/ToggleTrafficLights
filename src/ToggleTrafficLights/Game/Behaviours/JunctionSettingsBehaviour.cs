using System;
using System.Diagnostics;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.Behaviours
{
  public sealed class JunctionSettingsBehaviour : IDisposable
  {
    public JunctionSettingsBehaviour(SaveGameOptions options)
    {
      Options = options;
      Add();
    }

    public void Dispose()
    {
      Remove();
    }

    private void Add()
    {
      DebugLog.Info($"{nameof(JunctionSettingsBehaviour)}.Add");
      
      _junctionSettings = TrafficRoutesInfoViewPanel.instance.Find<UIPanel>("JunctionSettings");
      AddButtons();
      
      _changer = new TrafficLightsHandlingChanger();
      ApplyTrafficLightsHandling(Options.TrafficLights);
    }

    private void Remove()
    {
      DebugLog.Info($"{nameof(JunctionSettingsBehaviour)}.Remove");
      
      _changer.Dispose();
      _changer = null;
      
      RemoveButtons();

      _junctionSettings = null;
    }

    private UIPanel _junctionSettings;
    private Vector2 _originalPanelLightsPosition;
    private Vector2 _originalPanelStopSignsPosition;
    private const string PanelChangeAllLights = "PanelChangeAllLights";
    private const string PanelChangeHandlingLights = "PanelChangeHandlingLights";
    public readonly SaveGameOptions Options;

    private void AddButtons()
    {
      // traffic lights are hooked with add, remove, reset all lights
      // under the traffic lights buttons to add, remove, reset all lights
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
      AddToggleTrafficLightsButtons(_junctionSettings, PanelChangeAllLights);

      AddTrafficLightsDisabler(_junctionSettings, PanelChangeHandlingLights);
    }

    private void RemoveButtons()
    {
      // remove checkbox to disable traffic lights
      RemoveComponent(_junctionSettings, PanelChangeHandlingLights);
      
      // remove buttons under PanelLights panel to change all lights (same behaviour as icons)
      RemoveComponent(_junctionSettings, PanelChangeAllLights);
      
      // unregister click events for Traffic Lights icons
      UpdateIcons(_junctionSettings.Find<UIPanel>("PanelLights"), Mode.Remove);
      
      //move PanelLights to original position
      UpdateRelativePosition(_junctionSettings.Find<UIPanel>("PanelLights"), _originalPanelLightsPosition);

      //move PanelStopSigns to original position
      UpdateRelativePosition(_junctionSettings.Find<UIPanel>("PanelStopSigns"), _originalPanelStopSignsPosition);
    }

    private void RemoveComponent(UIPanel parent, string name)
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
//      pnl.relativePosition = new Vector3(46.0f, 55.0f);
//      pnl.relativePosition = new Vector3(85.0f, 10.0f);
      pnl.relativePosition = new Vector3(265.0f, 55.0f);
      pnl.height = 20 * 3 + 2 * 3;
      pnl.width = 120;
      pnl.autoLayout = true;
      pnl.autoLayoutDirection = LayoutDirection.Vertical;
      pnl.autoLayoutPadding = new RectOffset(0, 0, 1, 1);
      pnl.autoLayoutStart = LayoutStart.TopLeft;

      CreateButton(pnl, "AddAll", "Add all", "Add traffic lights to all junctions.", AddAllTrafficLights);
      CreateButton(pnl, "RemoveAll", "Remove all", "Remove traffic lights from all junctions.", RemoveAllTrafficLights);
      CreateButton(pnl, "ResetAll", "Reset all", "Reset all traffic lights.", ResetAllTrafficLights);

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

    private UICheckBox CreateCheckBox(UIPanel parent, string name, string text, string tooltip, bool defaultValue, OnCheckChanged onCheckChanged)
    {
      var helper = new UIHelper(parent);

      var cb = (UICheckBox)helper.AddCheckbox(text, defaultValue, onCheckChanged);
      cb.name = name;
      cb.tooltip = tooltip;
      cb.height = 16.0f;

      var lbl = cb.Find<UILabel>("Label");
      lbl.textScale = 0.8125f;
      lbl.anchor = UIAnchorStyle.Top | UIAnchorStyle.Left;
      var rp = lbl.relativePosition;
      rp.y = 2.0f;
      lbl.relativePosition = rp;

      cb.width = lbl.relativePosition.x + lbl.width;

      return cb;
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

    private UIPanel AddTrafficLightsDisabler(UIPanel parent, string name)
    {
      var pnl = parent.AddUIComponent<UIPanel>();
      pnl.name = name;
      pnl.relativePosition = new Vector3(80.0f, 67.5f);
      pnl.height = 16 + 2;
      pnl.autoLayout = true;
      pnl.autoLayoutDirection = LayoutDirection.Vertical;
      pnl.autoLayoutPadding = new RectOffset(0, 0, 1, 1);
      pnl.autoLayoutStart = LayoutStart.TopLeft;

      const string tooltip = "When enabled new junctions are always created without traffic lights.\n" +
                             "\n" +
                             "Existing Traffic Lights aren't touched, neither does it prevent you \n" +
                             "from manual changing the traffic lights.\n" +
                             "All it does is ensure all newly created junctions don't have traffic lights.\n" +
                             "\n" +
                             "Note: This is set on a per savegame basis and is not a global setting!";
      var cb = CreateCheckBox(pnl, PanelChangeHandlingLights, "Disable Traffic Lights", tooltip, Options.TrafficLights == SaveGameOptions.TrafficLightsHandling.NoTrafficLights, OnChangeHandlingLightsChanged);
      pnl.width = cb.width;
      
      return pnl;
    }

    #region Traffic Lights Handling
    private TrafficLightsHandlingChanger _changer = null;
    private void OnChangeHandlingLightsChanged(bool isChecked)
    {
      ChangeTrafficLightsHandling(isChecked ? SaveGameOptions.TrafficLightsHandling.NoTrafficLights : SaveGameOptions.TrafficLightsHandling.Default);
    }
    public void ChangeTrafficLightsHandling(SaveGameOptions.TrafficLightsHandling handling)
    {
      if (Options.TrafficLights == handling)
      {
        return;
      }
      
      Options.TrafficLights = handling;
      ApplyTrafficLightsHandling(handling);
    }

    private void ApplyTrafficLightsHandling(SaveGameOptions.TrafficLightsHandling handling)
    {
      DebugLog.Info($"Apply Traffic Lights Handling: {handling}");
      
      switch (handling)
      {
        case SaveGameOptions.TrafficLightsHandling.Default:
          _changer.Reset();
          break;
        case SaveGameOptions.TrafficLightsHandling.NoTrafficLights:
          _changer.Change(TrafficLights.ChangeMode.Remove);
          break;
        case SaveGameOptions.TrafficLightsHandling.AllTrafficLights:
          _changer.Change(TrafficLights.ChangeMode.Remove);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(handling), handling, null);
      }
    }
    #endregion Traffic Lights Handling
    private void DisableLights(UIComponent _, UIMouseEventParameter evt)
    {
      ChangeTrafficLightsHandling(SaveGameOptions.TrafficLightsHandling.NoTrafficLights);
    }
    private void ResetLights(UIComponent _, UIMouseEventParameter evt)
    {
      ChangeTrafficLightsHandling(SaveGameOptions.TrafficLightsHandling.Default);
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
