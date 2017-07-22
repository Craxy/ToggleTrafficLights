using System;
using System.Collections.Generic;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.Behaviours
{
  internal class DebugMainMachine : IDisposable
  {
    private readonly List<string> _log = new List<string>();

    public void Log(string message)
    {
      _log.Add(message);
    }

    private Texture2D _backgroundTexture = null;

    private readonly ImguiWindow.WindowSize _size = new ImguiWindow.WindowSize
    {
      Left = 75f,
      Top = 125f,
      Width = 225f,
      Height = 600f,
      Padding = 5f,
    };

    private bool _initialized = false;

    public void Setup()
    {
      if (_initialized)
      {
        return;
      }

      SetupTexture();
      _initialized = true;
    }

    private void SetupTexture()
    {
      _backgroundTexture = new Texture2D(1, 1);
      _backgroundTexture.SetPixel(0, 0, new Color(0.321f, 0.321f, 0.321f, 1.0f));
      _backgroundTexture.Apply();
    }

    private GUIStyle _skin = null;

    public void ShowGui(MainMachine mainMachine)
    {
      if (!_initialized)
      {
        Setup();
      }
      if (_skin == null)
      {
        _skin = new GUIStyle(GUI.skin.label) {fontSize = 11};
      }

      GUILayout.BeginArea(new Rect(_size.Left, _size.Top, _size.Width, _size.Height));
      {
        GUI.Box(new Rect(0f, 0f, _size.Width, _size.Height), _backgroundTexture);
        GUILayout.BeginArea(new Rect(_size.Padding, _size.Padding, _size.Width - 2 * _size.Padding,
          _size.Height - 2 * _size.Padding));
        {
          using (Layout.Vertical())
          {
            ShowStatus(mainMachine);
            GUILayout.Space(8.0f);
            ShowMessages(mainMachine);
            GUILayout.Space(8.0f);
            ShowLog(mainMachine);
          }
        }
        GUILayout.EndArea();
      }
      GUILayout.EndArea();
    }

    private void ShowStatus(MainMachine mainMachine)
    {
      using (Layout.Vertical())
      {
        var im = InfoManager.instance;
        GUILayout.Label($"State: {mainMachine.Current}");
        GUILayout.Label($"Tool: {mainMachine.CurrentTool?.GetType().Name}");
        GUILayout.Label($"Current Mode: {im.CurrentMode}, {im.CurrentSubMode}", _skin);
        GUILayout.Label($"Next Mode: {im.NextMode}, {im.NextSubMode}", _skin);
      }
    }

    private void ShowMessages(MainMachine mainMachine)
    {
      using (Layout.Vertical())
      {
        foreach (Message msg in Enum.GetValues(typeof(Message)))
        {
          if (GUILayout.Button(msg.ToString()))
          {
            mainMachine.Send(msg);
          }
        }
      }
    }

    private Vector2 _scrollPosition;
    private int _logsToShow = 0;

    private void ShowLog(MainMachine mainMachine)
    {
      // OnGUI is called multiple times per loop
      // If output is different it crashes
      // http://answers.unity3d.com/questions/400454/argumentexception-getting-control-0s-position-in-a-1.html

      if (Event.current.type == EventType.Layout)
      {
        _logsToShow = _log.Count;
      }

      using (Layout.Vertical())
      {
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
        {
          for (var i = _logsToShow - 1; i >= 0; i--)
          {
            var log = _log[i];
            GUILayout.Label(log, _skin);
          }
        }
        GUILayout.EndScrollView();
      }
    }

    public void Dispose()
    {
      if (!_initialized)
      {
        return;
      }

      _backgroundTexture = null;
      _log.Clear();
      _logsToShow = 0;

      _initialized = false;
    }
  }
}
