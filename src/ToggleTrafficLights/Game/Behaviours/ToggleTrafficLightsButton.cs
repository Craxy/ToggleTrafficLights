using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.Behaviours
{
  public sealed class ToggleTrafficLightsButton : UIButton
  {
    private Level _level;

    private bool _initialized = false;

    public override void Start()
    {
      base.Start();

      // can't use Awake because no parent at that point
      if (!_initialized)
      {
        DebugLog.Info($"Start->Initialize with parent: {this.parent.name}");

        Setup(this);
        _level = GameObject.FindObjectOfType<Level>();
        UpdateButtonState(_level.Current);
        _level.StateChanged += OnStateChanged;
        this.eventClick += OnButtonClick;
      }
    }

    public override void OnDestroy()
    {
      this.eventClick -= OnButtonClick;
      if (_level != null)
      {
        _level.StateChanged -= OnStateChanged;
        _level = null;
      }

      base.OnDestroy();
    }

    private void OnStateChanged(State _, State newState)
    {
      UpdateButtonState(newState);
    }

    private void UpdateButtonState(State state)
    {
      switch (state)
      {
        case State.Off:
          SetDeactivatedStateSprites(this);
          break;
        case State.Hidden:
          SetActiveStateSprites(this);
          break;
        case State.Active:
          SetActiveStateSprites(this);
          break;
        case State.TrafficRoutes:
          SetActiveUiStateSprites(this);
          break;
      }
      DebugLog.Info($"Button state changed to {state}");
    }

    protected override void OnMouseUp(UIMouseEventParameter p)
    {
      // OnClick is only called for left click but not right click
      if (p.buttons == UIMouseButton.Right)
      {
        OnClick(p);
      }

      base.OnMouseUp(p);
    }

    private void OnButtonClick(UIComponent _, UIMouseEventParameter e)
    {
      if (e.used)
      {
        return;
      }

      if (e.buttons == UIMouseButton.Left)
      {
        _level.Send(Message.ButtonLeft);
      }
      else if (e.buttons == UIMouseButton.Right)
      {
        _level.Send(Message.ButtonRight);
      }
    }

    #region Button

    public const string ButtonName = "ToggleTrafficLightsButton";

    private static void Setup(ToggleTrafficLightsButton button)
    {
      const int spriteWidth = 31;
      const int spriteHeight = 31;

      button.name = ButtonName;
      button.tooltip = "Left Click: Junction Tool\nRight Click: Traffic Routes Junctions Info Vie.";
      button.size = new Vector2(spriteWidth, spriteHeight);
      //add sprites
      button.atlas = CreateAtlas("icons.png", "ToggleTrafficLightsUI",
        UIView.Find<UITabstrip>("ToolMode").atlas.material,
        spriteWidth, spriteHeight, new[]
        {
          "OptionBase",
          "OptionBaseDisabled",
          "OptionBaseFocused",
          "OptionBaseHovered",
          "OptionBasePressed",
          "Selected",
          "Unselected",
          "OptionBaseFocusedRed",
        });
      button.playAudioEvents = true;
      button.relativePosition = new Vector3(131, 38);

      SetDeactivatedStateSprites(button);
    }

    //todo: read OptionBases from atlas template
    private static UITextureAtlas CreateAtlas(string file, string name, Material baseMaterial, int spriteWidth,
      int spriteHeight, string[] spriteNames)
    {
      var tex = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false)
      {
        filterMode = FilterMode.Bilinear,
      };

      //load texture
      var assembly = System.Reflection.Assembly.GetExecutingAssembly();
      using (var textureStream =
        assembly.GetManifestResourceStream("Craxy.CitiesSkylines.ToggleTrafficLights.Assets." + file))
      {
        var buf = new byte[textureStream.Length]; //declare arraysize
        textureStream.Read(buf, 0, buf.Length); // read from stream to byte array
        tex.LoadImage(buf);
        tex.Apply(true, false);
      }

      var atlas = ScriptableObject.CreateInstance<UITextureAtlas>();
      // Setup atlas
      var material = UnityEngine.Object.Instantiate(baseMaterial);
      material.mainTexture = tex;

      atlas.material = material;
      atlas.name = name;

      //add sprites
      for (var i = 0; i < spriteNames.Length; ++i)
      {
        var uw = 1.0f / spriteNames.Length;

        var spriteInfo = new UITextureAtlas.SpriteInfo
        {
          name = spriteNames[i],
          texture = tex,
          region = new Rect(i * uw, 0, uw, 1),
        };

        atlas.AddSprite(spriteInfo);
      }

      return atlas;
    }

    #endregion Button

    #region Sprites

    private static void SetActiveStateSprites(UIButton btn)
    {
      btn.normalFgSprite = "Selected";
      btn.disabledFgSprite = "Selected";
      btn.hoveredFgSprite = "Selected";
      btn.pressedFgSprite = "Selected";
      btn.focusedFgSprite = "Selected";

      btn.normalBgSprite = "OptionBaseFocused";
      btn.disabledBgSprite = "OptionBaseFocused";
      btn.hoveredBgSprite = "OptionBaseFocused";
      btn.pressedBgSprite = "OptionBaseFocused";
      btn.focusedBgSprite = "OptionBaseFocused";
    }

    private static void SetDeactivatedStateSprites(UIButton btn)
    {
      btn.normalFgSprite = "Unselected";
      btn.disabledFgSprite = "Unselected";
      btn.hoveredFgSprite = "Unselected";
      btn.pressedFgSprite = "Unselected";
      btn.focusedFgSprite = "Unselected";

      btn.normalBgSprite = "OptionBase";
      btn.disabledBgSprite = "OptionBase";
      btn.hoveredBgSprite = "OptionBaseHovered";
      btn.pressedBgSprite = "OptionBasePressed";
      btn.focusedBgSprite = "OptionBase";
    }

    private static void SetActiveUiStateSprites(UIButton btn)
    {
      btn.normalFgSprite = "Selected";
      btn.disabledFgSprite = "Selected";
      btn.hoveredFgSprite = "Selected";
      btn.pressedFgSprite = "Selected";
      btn.focusedFgSprite = "Selected";

      btn.normalBgSprite = "OptionBaseFocusedRed";
      btn.disabledBgSprite = "OptionBaseFocusedRed";
      btn.hoveredBgSprite = "OptionBaseFocusedRed";
      btn.pressedBgSprite = "OptionBaseFocusedRed";
      btn.focusedBgSprite = "OptionBaseFocusedRed";
    }

    #endregion Sprites

    #region Panels

    private static UIPanel GetOptionsBar()
    {
      return UIView.Find<UIPanel>("OptionsBar");
    }

    private static UIPanel GetRoadsOptionPanel()
    {
      return GetOptionsBar().Find<UIPanel>("RoadsOptionPanel");
    }

    /// <summary>
    /// When Roads Menu is opened RoadsOptionPanel is copied and that copy is used.
    /// 
    /// -> Is null prior to the first use of the roads menu
    /// </summary>
    private static UIPanel GetRoadsOptionPanelInRoadsPanel()
    {
      return GetOptionsBar().Find<UIPanel>("RoadsOptionPanel(RoadsPanel)");
    }

    /// <summary>
    /// If available return <see cref="GetRoadsOptionPanel"/> else <see cref="GetRoadsOptionPanel"/>
    /// </summary>
    private static UIPanel GetCurrentRoadsOptionPanel()
    {
      return GetRoadsOptionPanelInRoadsPanel() ?? GetRoadsOptionPanel();
    }

    public static void InitialAdd()
    {
      var pnl = GetCurrentRoadsOptionPanel();
      pnl.AddUIComponent<ToggleTrafficLightsButton>();
    }

    public static void DestroyAll()
    {
      foreach (var btn in GameObject.FindObjectsOfType<ToggleTrafficLightsButton>())
      {
        var parent = btn.parent;
        if (parent != null)
        {
          parent.RemoveUIComponent(btn);
        }
        GameObject.Destroy(btn);
      }
    }

    #endregion
  }
}
