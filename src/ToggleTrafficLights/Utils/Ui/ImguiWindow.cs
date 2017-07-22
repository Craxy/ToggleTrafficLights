using System;
using ColossalFramework.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Ui
{
  public abstract class ImguiWindow
  {
    private static Texture2D _backgroundTexture = null;

    /// <summary>
    /// Used for Clickcatcher -> UIPanel
    /// </summary>
    private static Texture2D BackgroundTexture
    {
      get
      {
        if (_backgroundTexture == null)
        {
          _backgroundTexture = new Texture2D(1, 1);
          _backgroundTexture.SetPixel(0, 0, new Color(0.321f, 0.321f, 0.321f, 1.0f));
          _backgroundTexture.Apply();
        }
        return _backgroundTexture;
      }
    }

    public WindowSize Size { get; } = new WindowSize();
    private UIPanel _clickCatcher = null;
    private Vector2 _scrollPosition;

    private GUIStyle _style = null;
    protected GUIStyle Style => _style ?? (_style = new GUIStyle(GUI.skin.label) {fontSize = 11});

//    public bool Active { get; set; } = true;
//    public void Show()
//    {
//      Active = true;
//    }
//
//    public void Hide()
//    {
//      Active = false;
//    }

    public void OnEnable()
    {
      DebugLog.Info($"Enable window {this.GetType().Namespace}");

      if (_clickCatcher == null)
      {
        var uiView = UIView.GetAView();
        _clickCatcher = (UIPanel) uiView.AddUIComponent(typeof(UIPanel));
        _clickCatcher.name = $"{Guid.NewGuid().ToString()}Background";
        //without background sprite it's invisible
//        _clickCatcher.backgroundSprite = "GenericPanel";
      }
      UpdateClickCatcher();
      Size.SizeChanged += UpdateClickCatcher;
      _clickCatcher.isVisible = true;
      _clickCatcher.isEnabled = true;
    }

    public void OnDisable()
    {
      DebugLog.Info($"Disable window {this.GetType().Namespace}");

      if (_clickCatcher != null)
      {
        Size.SizeChanged -= UpdateClickCatcher;
        _clickCatcher.isVisible = false;
        _clickCatcher.isEnabled = false;
      }
    }

    public void OnDestroy()
    {
      DebugLog.Info($"Destroy window {this.GetType().Namespace}");

      if (_clickCatcher != null)
      {
        Object.Destroy(_clickCatcher.gameObject);
      }
      _clickCatcher = null;
    }

    public void UpdateClickCatcher()
    {
      if (_clickCatcher == null)
      {
        return;
      }

      var uiView = UIView.GetAView();
      //adjust _size from unity pixels to C:S pixels via GetUIView().ratio
      var ratio = uiView.ratio;

      _clickCatcher.absolutePosition = new Vector3(Size.Left * ratio, Size.Top * ratio);
      _clickCatcher.size = new Vector2(Size.Width * ratio, Size.Height * ratio);
      _clickCatcher.zOrder = int.MaxValue;
    }

    public void Draw()
    {
      GUILayout.BeginArea(new Rect(Size.Left, Size.Top, Size.Width, Size.Height));
      {
        GUI.Box(new Rect(0.0f, 0.0f, Size.Width, Size.Height), BackgroundTexture);
        GUILayout.BeginArea(new Rect(Size.Padding, Size.Padding, Size.Width - 2 * Size.Padding,
          Size.Height - 2 * Size.Padding));
        {
          using (Layout.Vertical())
          {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            {
              DrawContent();
            }
            GUILayout.EndScrollView();
          }
        }
        GUILayout.EndArea();
      }
      GUILayout.EndArea();
    }

    protected abstract void DrawContent();

    public sealed class WindowSize
    {
      private float _left;

      public float Left
      {
        get => _left;
        set
        {
          if (value != _left)
          {
            _left = value;
            OnSizeChanged();
          }
        }
      }

      private float _top;

      public float Top
      {
        get => _top;
        set
        {
          if (value != _top)
          {
            _top = value;
            OnSizeChanged();
          }
        }
      }

      private float _width;

      public float Width
      {
        get => _width;
        set
        {
          if (value != _width)
          {
            _width = value;
            OnSizeChanged();
          }
        }
      }

      private float _height;

      public float Height
      {
        get => _height;
        set
        {
          if (value != _height)
          {
            _height = value;
            OnSizeChanged();
          }
        }
      }

      private float _padding;

      public float Padding
      {
        get => _padding;
        set
        {
          if (value != _padding)
          {
            _padding = value;
            OnSizeChanged();
          }
        }
      }

      public event Action SizeChanged;

      private void OnSizeChanged()
      {
        SizeChanged?.Invoke();
      }
    }
  }

  public sealed class SetupableImguiWindow : ImguiWindow
  {
    public Action Content { get; set; }

    protected override void DrawContent()
    {
      Content?.Invoke();
    }
  }
}
