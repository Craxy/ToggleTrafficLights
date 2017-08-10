using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using UnityEngine;
using ReflectionExtensions = Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions.ReflectionExtensions;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.Behaviours
{
  public enum State
  {
    Off, // IntersectionTool is not active
    Hidden, // IntersectionTool active but keeping current InfoMode and GUI
    Active, // IntersectionTool is active and RoadsMenu-TTL button is selected (-> no InfoMode and custom GUI)
    TrafficRoutes, // IntersectionTool is not active, TrafficRoutes->Junctions is active (Buttons are inserted)
  }
     
  public static class StateExtensions
  {
    public static bool Is(this State state, State test)
    {
      return state == test;
    }
    
    public static bool IsAnyOf(this State state, State test)
    {
      return state == test;
    }
    
    public static bool IsAnyOf(this State state, State test1, State test2)
    {
      return state == test1 || state == test2;
    }
    
    public static bool IsAnyOf(this State state, State test1, State test2, State test3)
    {
      return state == test1 || state == test2 || state == test3;
    }
  }

  public enum Message
  {
    IntersectionToolDisabled,

    JunctionSettingsShow, // activate JunctionSettings tab
    JunctionSettingsHide, // change JunctionSettings to something else

    ButtonLeft,
    ButtonRight,

    ShortcutMenu,
    ShortcutHidden,
    ShortcutTrafficRoutes,
    ShortcutEscape,
  }
  
  public sealed class Level : MonoBehaviour
  {
    #region fields
    public Options Options;
    public SaveGameOptions GameOptions;
    
    public State Current = State.Off;
    private JunctionTool _tool;
    private UIPanel _junctionSettings;
    private JunctionSettingsBehaviour _junctionSettingsBehaviour;
    #endregion fields
    
    #region MonoBehaviour
    private void Start()
    {
      //todo: dalay after every mod is loaded -> give tome to load roads
      Setup();
    }
    //    private void OnEnable() {}
    //    private void OnDisable() {}
    private void OnDestroy()
    {
      Destroy();
    }
    private void OnGUI()
    {
      HandleShortcuts();
 
      DisplayDebugGui();
    }
    private bool _messageReceived = false;
    private void Update()
    {
      if (_messageReceived)
      {
        _messageReceived = false;
        UpdateState();
      }
    }
    #endregion

    #region Debug
    [Conditional("DEBUG")]
    private void SetupDebug()
    {
#if DEBUG
      _debugLevel.Setup();
#endif
    }

    [Conditional("DEBUG")]
    private void DestroyDebug()
    {
#if DEBUG
      _debugLevel.Dispose();
#endif
    }
    [Conditional("DEBUG")]
    private void DisplayDebugGui()
    {
#if DEBUG
      // when ShowDebugGuiKey is checked before showing gui:
      // ArgumentException: Getting control 0's position in a group with only 0 controls when doing KeyDown. Aborting.
      // That happens when when OnGUI with events EventType.Layout and EventType.Repaint draw different content
      // Problem: Key.IsPressed(...) isn't registered in Layout, but Repaint -> first OnGUI with Layout isn't drawn, but Repaint is -> different content
      // solution: check Key after drawing the gui
      //   that way ShowGUI is drawn one frame later, but starts with Layout and therefore doesn't raise an exception
      if (_showDebugGui)
      {
        _debugLevel.ShowGui(this);
      }
      if (ShowDebugGuiKey.IsPressed(Event.current))
      {
        _showDebugGui = !_showDebugGui;
      }
#endif
    }
#if DEBUG
    private static readonly InputKey ShowDebugGuiKey = SavedInputKey.Encode(KeyCode.Keypad0, true, false, true);
    private bool _showDebugGui = false;
    private readonly DebugLevel _debugLevel = new DebugLevel();
#endif
    [Conditional("DEBUG")]
    private void Log(string message)
    {
#if DEBUG
      _debugLevel.Log(message);
#endif
    }

    #endregion Debug
    
    #region Setup

    private void Setup()
    {
      SetupDebug();
      SetupTtlButton();
      SetupTool();
      SetupTrafficRoutesInfoView();
      
      DebugLog.Info("Level created");
    }

    private void SetupTtlButton()
    {
      ToggleTrafficLightsButton.InitialAdd();
    }

    private void SetupTool()
    {
      _tool = ToolHelper.AddTool<JunctionTool>();
      _tool.Disabled += OnToolDisabled;
    }

    private void SetupTrafficRoutesInfoView()
    {
      _junctionSettings = TrafficRoutesInfoViewPanel.instance.Find<UIPanel>("JunctionSettings");
      _junctionSettings.eventVisibilityChanged += OnJunctionSettingsVisibilityChanged;

      _junctionSettingsBehaviour = new JunctionSettingsBehaviour(GameOptions);
    }

    #endregion Setup

    #region Destroy

    private void Destroy()
    {
      _junctionSettingsBehaviour.Dispose();
      _junctionSettingsBehaviour = null;

      _junctionSettings.eventVisibilityChanged -= OnJunctionSettingsVisibilityChanged;
      _junctionSettings = null;

      ToggleTrafficLightsButton.DestroyAll();

      _tool.Disabled -= OnToolDisabled;
      ToolHelper.RemoveTool(_tool);
      _tool = null;

      Options = null;
      
      ForgetPreviousTool();

      DestroyDebug();
      
      DebugLog.Info("Level destroyed");
    }

    #endregion

    #region State

    public State CalcState()
    {
      DebugLog.Info("Recalc state");

      if (IntersectionToolIsActive)
      {
        //Hidden or enabled
        if (RoadsOptionPanelIsVisible)
        {
          return State.Active;
        }
        else
        {
          return State.Hidden;
        }
      }
      else if (JunctionSettingsIsActive)
      {
        return State.TrafficRoutes;
      }
      else
      {
        return State.Off;
      }
    }

    private bool UpdateState()
    {
      var newState = CalcState();
      if (newState != Current)
      {
        var oldState = Current;
        Current = newState;
        OnStateChanged(oldState, newState);
        return true;
      }
      else
      {
        return false;
      }
    }

    public event Action<State, State> StateChanged;

    private void OnStateChanged(State oldState, State newState)
    {
      Log($"{oldState} -> {newState}");
      StateChanged?.Invoke(oldState, newState);
    }

    #endregion State

    #region Shortcuts

    private void HandleShortcuts()
    {
      if (Options == null)
      {
        return;
      }

      var current = Event.current;
      if (current == null || !current.isKey || current.keyCode == KeyCode.None)
      {
        return;
      }
      if (UIView.HasModalInput() || UIView.HasInputFocus())
      {
        return;
      }
      HandleShortcut(current, Options.ShortcutActivateTTLWithoutMenu, Message.ShortcutHidden)
        ?.HandleShortcut(current, Options.ShortcutActivateTTLWithMenu, Message.ShortcutMenu)
        ?.HandleShortcut(current, Options.ShortcutActivateTrafficRoutesJunctions, Message.ShortcutTrafficRoutes);
    }

    private Level HandleShortcut(Event e, SavedInputKey shortcut, Message message)
    {
      if (shortcut.IsPressed(e))
      {
        Send(message);
        return null;
      }
      else
      {
        return this;
      }
    }

    #endregion Shortcuts

    #region Events

    private void OnToolDisabled()
    {
      Send(Message.IntersectionToolDisabled);
    }

    private void OnJunctionSettingsVisibilityChanged(UIComponent _, bool visible)
    {
      Send(visible ? Message.JunctionSettingsShow : Message.JunctionSettingsHide);
    }

    #endregion Events

    #region Actions

    private void ActivateIntersectionTool(bool keepInfoMode, JunctionTool.Elevation elevation)
    {
      var tool = ToolHelper.SetTool<JunctionTool>();
      
      if (tool == null)
      {
        Utils.Log.Error($"ToolHelper.SetTool returned null.\n{GetToolLog()}");
      }
      
      if (keepInfoMode)
      {
        tool.CurrentElevation = elevation;
      }
    }

    private void ActivateIntersectionTool(bool keepInfoMode)
    {
      ActivateIntersectionTool(keepInfoMode, GetElevationFrom(CurrentInfoMode));
    }

    private JunctionTool.Elevation GetElevationFrom(InfoManager.InfoMode infoMode)
    {
      return infoMode != InfoManager.InfoMode.None
        ? JunctionTool.Elevation.Underground
        : JunctionTool.Elevation.OvergroundWithTunnels;
    }

    private void OpenJunctionSettings()
    {
      SelectUiButton("InfoTrafficRoutes");
    }

    private void OpenJunctionSettingsHere()
    {
      ActivateTool<DefaultTool>();
      SetInfoMode(InfoManager.InfoMode.TrafficRoutes, InfoManager.SubInfoMode.JunctionSettings);
    }

    private void OpenRoadsOptionPanel()
    {
      if (!RoadsOptionPanelIsVisible)
      {
        // open up RoadsOptionPanel and select TTL button
        DebugLog.Info($"Click on Roads because RoadsPanel is not visible");
        var tutorialUiTag = (TutorialUITag) MonoTutorialTag.Find("Roads");
        tutorialUiTag.target.SimulateClick();
      }
    }

    private bool ActivateTool<T>()
      where T : ToolBase
    {
      return ActivateAndReturnTool<T>() != null;
    }

    private T ActivateAndReturnTool<T>()
      where T : ToolBase
    {
      var tool = ToolsModifierControl.SetTool<T>();
      if (tool == null)
      {
        Utils.Log.Error($"SetTool returned null.\n{GetToolLog()}");
      }
      
      return tool;
    }

    private void SetInfoMode(InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode)
    {
      InfoManager.instance.SetCurrentMode(infoMode, subInfoMode);
    }

    private void ResetInfoMode()
    {
      SetInfoMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
    }

    private void SelectUiButton(string tagString)
    {
      //Source: KeyShortcuts.SelectUIButton
      if (PopsManager.exists)
      {
        PopsManager.instance.ShortcutPressed(tagString);
      }
      var tutorialUiTag = MonoTutorialTag.Find(tagString) as TutorialUITag;
      if (tutorialUiTag == null || tutorialUiTag.finalTarget == null)
      {
        return;
      }
      for (var index = 100; index > 0; --index)
      {
        if (tutorialUiTag.target == tutorialUiTag.finalTarget)
        {
          tutorialUiTag.target.SimulateClick();
          return;
        }
        tutorialUiTag.target.SimulateClick();
        if (!tutorialUiTag.target.isEnabled)
        {
          return;
        }
      }
      CODebugBase<LogChannel>.Error(LogChannel.CommandLine,
        "SelectUIButton() was terminated to prevent an infinite loop. This might be some kind of bug... :D");
    }

    #region previous tool

    private ToolBase _previousTool = null;
    private InfoManager.InfoMode _previousInfoMode;
    private InfoManager.SubInfoMode _previousSubInfoMode;

    private void RememberCurrentTool()
    {
      _previousTool = CurrentTool;
      var im = InfoManager.instance;
      _previousInfoMode = im.NextMode;
      _previousSubInfoMode = im.NextSubMode;
      DebugLog.Info($"Remembering {_previousTool?.GetType().Name} and {im.NextMode},{im.NextSubMode}");
    }

    private void RestorePreviousTool()
    {
      if (_previousTool == ToolsModifierControl.GetTool<CameraTool>())
      {
        DebugLog.Info($"Restoring CameraTool -> DefaultTool");
        ToolsModifierControl.SetTool<DefaultTool>();
      }
      if (_previousTool != null)
      {
        DebugLog.Info($"Restoring {_previousTool?.GetType().Name}");
        ToolsModifierControl.toolController.CurrentTool = _previousTool;
        if (_previousTool == ToolsModifierControl.GetTool<DefaultTool>())
        {
          DebugLog.Info($"Restoring Modes {_previousInfoMode},{_previousSubInfoMode}");
          InfoManager.instance.SetCurrentMode(_previousInfoMode, _previousSubInfoMode);
        }
      }
      else
      {
        DebugLog.Info($"Restoring [null] -> DefaultTool");
        // no previous tool registered: reset to default tool
        ToolsModifierControl.SetTool<DefaultTool>();
      }
    }

    private void ForgetPreviousTool()
    {
      if (_previousTool == null)
      {
        return;
      }

      DebugLog.Info($"Remembering {_previousTool?.GetType().Name} and {_previousInfoMode},{_previousSubInfoMode}");
      _previousTool = null;
      _previousInfoMode = InfoManager.InfoMode.None;
      _previousSubInfoMode = InfoManager.SubInfoMode.Default;
    }

    private bool SomeToolRemembered => _previousTool != null;

    #endregion previous tool

    #endregion Actions

    #region Messages

    public void Send(Message message)
    {
      Log($"!{message}");

      OnMessage(message);
    }

    private void OnMessage(Message message)
    {
      _messageReceived = true;

      UpdateState();

      switch (message)
      {
        case Message.ButtonLeft when Current.IsAnyOf(State.Off, State.TrafficRoutes):
        {
          var elevation = GetElevationFrom(CurrentInfoMode);
          ForgetPreviousTool();
          OpenRoadsOptionPanel();
          ActivateIntersectionTool(Options.KeepInfoMode.value, elevation);
        }
          break;
        case Message.ButtonLeft when Current.Is(State.Active):
          ForgetPreviousTool();
          ActivateTool<NetTool>();
          break;

        case Message.ButtonRight when Current.IsAnyOf(State.Off, State.Active):
          ForgetPreviousTool();
          OpenJunctionSettingsHere();
          break;
        case Message.ButtonRight when Current.Is(State.TrafficRoutes):
          ForgetPreviousTool();
          ActivateTool<NetTool>();
          break;

        case Message.IntersectionToolDisabled:
          break;


        case Message.ShortcutMenu when Current.IsAnyOf(State.Off, State.TrafficRoutes):
        {
          var elevation = GetElevationFrom(CurrentInfoMode);
          ForgetPreviousTool();
          OpenRoadsOptionPanel();
          ActivateIntersectionTool(Options.KeepInfoMode.value, elevation);
        }
          break;
        case Message.ShortcutMenu when Current.IsAnyOf(State.Active, State.Hidden):
          ForgetPreviousTool();
          ActivateTool<NetTool>();
          break;

        case Message.ShortcutHidden when Current.Is(State.Off) && RoadsOptionPanelIsVisible:
          ForgetPreviousTool();
          ActivateIntersectionTool(Options.KeepInfoMode.value);
          break;
        case Message.ShortcutHidden when Current.Is(State.Off):
          RememberCurrentTool();
          ActivateIntersectionTool(Options.KeepInfoMode.value);
          break;
        case Message.ShortcutHidden when Current.Is(State.Active):
          ForgetPreviousTool();
          ActivateTool<NetTool>();
          break;
        case Message.ShortcutHidden when Current.Is(State.Hidden) && SomeToolRemembered:
          RestorePreviousTool();
          ForgetPreviousTool();
          break;
        case Message.ShortcutHidden when Current.Is(State.Hidden):
          ActivateTool<DefaultTool>();
          ForgetPreviousTool();
          break;
        case Message.ShortcutHidden when Current.Is(State.TrafficRoutes) && RoadsOptionPanelIsVisible:
          ForgetPreviousTool();
          ActivateIntersectionTool(Options.KeepInfoMode.value);
          break;
        case Message.ShortcutHidden when Current.Is(State.TrafficRoutes):
          // keep potential previous remembered tool
          ActivateIntersectionTool(Options.KeepInfoMode.value);
          break;

        case Message.ShortcutTrafficRoutes when Current.Is(State.Off) && RoadsOptionPanelIsVisible:
          ForgetPreviousTool();
          OpenJunctionSettingsHere();
          break;
        case Message.ShortcutTrafficRoutes when Current.Is(State.Off):
          RememberCurrentTool();
          OpenJunctionSettingsHere();
          break;
        case Message.ShortcutTrafficRoutes when Current.Is(State.Hidden):
          // keep remembered tool from hidden
          OpenJunctionSettingsHere();
          break;
        case Message.ShortcutTrafficRoutes when Current.Is(State.Active):
          ForgetPreviousTool();
          OpenJunctionSettingsHere();
          break;
        case Message.ShortcutTrafficRoutes when Current.Is(State.TrafficRoutes) && RoadsOptionPanelIsVisible:
          ForgetPreviousTool();
          ActivateTool<NetTool>();
          break;
        case Message.ShortcutTrafficRoutes when Current.Is(State.TrafficRoutes) && SomeToolRemembered:
          RestorePreviousTool();
          ForgetPreviousTool();
          break;
        case Message.ShortcutTrafficRoutes when Current.Is(State.TrafficRoutes):
          ForgetPreviousTool();
          SetInfoMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
          break;

        case Message.ShortcutEscape:
          //todo: implement
          break;
        case Message.JunctionSettingsShow:
          //do nothing
          break;
        case Message.JunctionSettingsHide:
          //do nothing
          break;

        default:
          break;
      }

      UpdateState();
    }

    #endregion Messages

    #region Info

    public ToolBase CurrentTool => ToolsModifierControl.toolController.CurrentTool;
    public bool RoadsOptionPanelIsVisible => UIView.Find<UIPanel>("RoadsPanel").isVisible;
    public bool JunctionSettingsIsVisible => _junctionSettings.isVisible;

    public bool JunctionSettingsIsActive
      => CurrentTool is DefaultTool
         && IsInfoMode(InfoManager.InfoMode.TrafficRoutes, InfoManager.SubInfoMode.JunctionSettings);

    public bool IntersectionToolIsActive => CurrentTool is JunctionTool;
    public InfoManager.InfoMode CurrentInfoMode => InfoManager.instance.NextMode;
    public InfoManager.SubInfoMode CurrentSubInfoMode => InfoManager.instance.NextSubMode;

    public bool IsInfoMode(InfoManager.InfoMode infoMode, InfoManager.SubInfoMode subInfoMode)
    {
      var im = InfoManager.instance;
      return im.NextMode == infoMode && im.NextSubMode == subInfoMode;
    }

    #endregion
    
    private static string GetToolLog()
    {
      var tools = ReflectionExtensions
        .GetNonPublicStaticField<ToolsModifierControl, Dictionary<Type, ToolBase>>("m_Tools")
        .Values
        .Select(t => t.GetType().FullName)
        .ToArray();
      var components = ToolsModifierControl.toolController.GetComponents<ToolBase>()
        .Select(t => t.GetType().FullName).ToArray();
      var msg = $"Collected tools: {string.Join(", ", tools)}\nAdded Components: {string.Join(", ", components)}";
      return msg;
    }
  }
}
