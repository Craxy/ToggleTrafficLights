using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using ColossalFramework.Globalization;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using ICities;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
  public sealed class SettingsBuilder
  {
    public static void MakeSettings(UIHelper helper, Options options)
    {
      {
        var group = helper.AddGroup("Settings");
        {
          var cb = (UICheckBox) group.AddCheckbox("Keep Info Mode", options.KeepInfoMode.value, c => options.KeepInfoMode.value = c);
          cb.tooltip = "If enabled, activating the junction tool keeps the current view (underground or overground). Otherwise the last used view is used.";
        }
      }
      {
        var group = helper.AddGroup("Keymapping");
        var pnl = UISettingsPanel.AddTo((UIComponent) ((UIHelper) group).self, options);
      }
    }
  }

  public class UISettingsPanel : UIPanel
  {
    public LocaleManager LocaleManager = LocaleManager.instance;

    public static UISettingsPanel AddTo(UIComponent parent, Options options)
    {
      return parent.AddUIComponent<UISettingsPanel>().Setup(options);
    }

    private Options _options;

    private UISettingsPanel Setup(Options options)
    {
      _options = options;

      var pnt = (UIPanel) parent;
      this.width = parent.width - pnt.padding.left - pnt.padding.right;

      this.autoLayout = true;
      this.autoFitChildrenVertically = true;
      this.autoLayoutDirection = LayoutDirection.Vertical;
      this.autoLayoutPadding = new RectOffset(10, 10, 5, 0);

      var i = 0;

      AddShortcut(i++, Options.GetShortcutName(nameof(options.ShortcutActivateTTLWithMenu)),
        options.ShortcutActivateTTLWithMenu);
      AddShortcut(i++, Options.GetShortcutName(nameof(options.ShortcutActivateTTLWithoutMenu)),
        options.ShortcutActivateTTLWithoutMenu);
      AddShortcut(i++, Options.GetShortcutName(nameof(options.ShortcutActivateTrafficRoutesJunctions)),
        options.ShortcutActivateTrafficRoutesJunctions);

      AddSpace(25);

      AddShortcutDisplay(i++, "Elevation Up",
        new SavedInputKey(Settings.buildElevationUp, Settings.gameSettingsFile, DefaultSettings.buildElevationUp,
          true));
      AddShortcutDisplay(i++, "Elevation Down",
        new SavedInputKey(Settings.buildElevationDown, Settings.gameSettingsFile, DefaultSettings.buildElevationDown,
          true));

      AddSpace(50);

      AddRightButton("Reset shortcuts",
        "Reset TTL shortcuts to their default value. Doesn't reset the Elevation shortcuts.",
        (_, __) => ResetShortcuts());

      return this;
    }

    private void AddSpace(int space)
    {
      new UIHelper(this).AddSpace(space);
    }

    private void AddCheckbox(string text, bool initialValue, OnCheckChanged onChanged)
    {
      new UIHelper(this).AddCheckbox(text, initialValue, onChanged);
    }

    private void AddRightButton(string text, string tooltip, MouseEventHandler onClick)
    {
      var pnl = this.AddUIComponent<UIPanel>();
      pnl.autoLayout = true;
      pnl.autoLayoutDirection = LayoutDirection.Horizontal;
      pnl.autoLayoutStart = LayoutStart.TopRight;
      pnl.width = this.width - this.padding.left - this.padding.right - 50.0f;

      UIButton btn = (UIButton) pnl.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsButtonTemplate"));
      btn.text = text;
      btn.tooltip = tooltip;
      btn.eventClick += onClick;
      pnl.height = btn.height;
    }

    private void AddShortcut(int i, string name, SavedInputKey shortcut)
    {
      //Source: OptionsKeymappingPanel.CreateBindableInputs
      const string keyBindingTemplate = "KeyBindingTemplate";
      var pnl = (UIPanel) this.AttachUIComponent(UITemplateManager.GetAsGameObject(keyBindingTemplate));
      if (i % 2 == 0)
      {
        pnl.backgroundSprite = string.Empty;
      }
      var lbl = pnl.Find<UILabel>("Name");
      lbl.text = name;

      var btn = pnl.Find<UIButton>("Binding");
      btn.objectUserData = shortcut;
      btn.text = shortcut.ToLocalizedString("KEYNAME");
      btn.eventKeyDown += OnBindingKeyDown;
      btn.eventMouseDown += OnBindingMouseDown;
    }

    private void AddShortcutDisplay(int i, string name, SavedInputKey shortcut)
    {
      const string keyBindingTemplate = "KeyBindingTemplate";
      var pnl = (UIPanel) this.AttachUIComponent(UITemplateManager.GetAsGameObject(keyBindingTemplate));
      if (i % 2 == 0)
      {
        pnl.backgroundSprite = string.Empty;
      }

      var lbl = pnl.Find<UILabel>("Name");
      lbl.text = name;
      lbl.tooltip = "C:S shortcut. Change in Cities:Skylines Keymapping settings";

      var btn = pnl.Find<UIButton>("Binding");
      btn.objectUserData = shortcut;
      btn.text = shortcut.ToLocalizedString("KEYNAME");
      btn.tooltip = lbl.tooltip;
      btn.canFocus = false;
      btn.hoveredColor = btn.color;
      btn.pressedColor = btn.color;
      btn.focusedColor = btn.color;
    }

    private SavedInputKey _editingBinding = null;

    private void OnBindingMouseDown(UIComponent comp, UIMouseEventParameter e)
    {
      if (_editingBinding == null)
      {
        var btn = (UIButton) comp;
        _editingBinding = (SavedInputKey) btn.objectUserData;
        btn.buttonsMask = UIMouseButton.Left | UIMouseButton.Right | UIMouseButton.Middle | UIMouseButton.Special0 |
                          UIMouseButton.Special1 | UIMouseButton.Special2 | UIMouseButton.Special3;
        btn.text = Locale.Get("KEYMAPPING_PRESSANYKEY");
        btn.Focus();
        UIView.PushModal(btn);
      }
      // I don't want Mouse buttons to be asignable as shortcut
    }

    private void OnBindingKeyDown(UIComponent comp, UIKeyEventParameter e)
    {
      var key = e.keycode;
      if (_editingBinding == null || KeyHelper.IsModifierKey(key))
      {
        return;
      }

      e.Use();
      UIView.PopModal();

      InputKey newKey;
      switch (key)
      {
        case KeyCode.Backspace:
          newKey = SavedInputKey.Empty;
          break;
        case KeyCode.Escape:
          newKey = _editingBinding.value;
          break;
        default:
          newKey = SavedInputKey.Encode(key, e.control, e.shift, e.alt);
          break;
      }

      if (IsAlreadyBound(_editingBinding, newKey, out var currentAssigned))
      {
        var text = currentAssigned.Count <= 1
          ? (
            Locale.Exists("KEYMAPPING", currentAssigned[0].name)
              ? Locale.Get("KEYMAPPING", currentAssigned[0].name)
              : Options.GetShortcutName(currentAssigned[0].name)
          )
          : Locale.Get("KEYMAPPING_MULTIPLE");
        var message = StringUtils.SafeFormat(Locale.Get("CONFIRM_REBINDKEY", "Message"),
          SavedInputKey.ToLocalizedString("KEYNAME", newKey), text);

        ConfirmPanel.ShowModal(Locale.Get("CONFIRM_REBINDKEY", "Title"), message, (c, ret) =>
        {
          var btn = (UIButton) comp;
          if (ret == 1)
          {
            _editingBinding.value = newKey;
            foreach (var asigned in currentAssigned)
            {
              asigned.value = SavedInputKey.Empty;
            }
            UpdateKeyBinding(newKey, btn, false);
            RefreshKeyMapping();
          }
          _editingBinding = null;
          btn.text = ((SavedInputKey) btn.objectUserData).ToLocalizedString("KEYNAME");
        });
      }
      else
      {
        UpdateKeyBinding(newKey, (UIButton) comp, false);
      }
    }

    private void UpdateKeyBinding(InputKey newKey, UIButton button, bool isMouseInput)
    {
      if (_editingBinding == null)
      {
        return;
      }

      _editingBinding.value = newKey;
      button.text = _editingBinding.ToLocalizedString("KEYNAME");
      if (isMouseInput)
      {
        button.buttonsMask = UIMouseButton.Left;
      }
      _editingBinding = null;
    }

    private bool IsAlreadyBound(SavedInputKey target, InputKey inputKey, out IList<SavedInputKey> currentAssigned)
    {
      // first test Cities Skylines keys
      var bound = IsAlreadyBoundCitiesSkylines(target, inputKey, out var assigned);
      bound = IsAlreadyBoundTtl(target, inputKey, _options, ref assigned) || bound;
      // then test TTL keys
      //todo: implement

      currentAssigned = assigned;
      return bound;
    }

    private static bool IsAlreadyBoundCitiesSkylines(SavedInputKey target, InputKey inputKey,
      out List<SavedInputKey> currentAssigned)
    {
      const string category = "Game";
      currentAssigned = new List<SavedInputKey>();
      if (inputKey == SavedInputKey.Empty)
      {
        return false;
      }
      foreach (var field in typeof(Settings).GetFields(BindingFlags.Static | BindingFlags.Public))
      {
        var customAttributes =
          (RebindableKeyAttribute[]) field.GetCustomAttributes(typeof(RebindableKeyAttribute), false);
        if (customAttributes.Length > 0 && (category == customAttributes[0].category ||
                                            string.IsNullOrEmpty(customAttributes[0].category)))
        {
          var str = field.GetValue(null) as string;
          var savedInputKey = new SavedInputKey(str, Settings.gameSettingsFile, GetDefaultEntry(str), true);
          if (target != savedInputKey && inputKey == savedInputKey.value)
          {
            currentAssigned.Add(savedInputKey);
          }
        }
      }
      return currentAssigned.Count > 0;
    }

    private static InputKey GetDefaultEntry(string entryName)
    {
      var field = typeof(DefaultSettings).GetField(entryName, BindingFlags.Static | BindingFlags.Public);
      if (field == null)
      {
        return 0;
      }
      var obj = field.GetValue(null);
      if (obj is InputKey)
      {
        return (InputKey) obj;
      }
      return 0;
    }

    private static bool IsAlreadyBoundTtl(SavedInputKey target, InputKey inputKey, Options options,
      ref List<SavedInputKey> currentAssigned)
    {
      if (inputKey == SavedInputKey.Empty)
      {
        return false;
      }

      bool newAssigned = false;
      foreach (var field in typeof(Options).GetFields(BindingFlags.Public | BindingFlags.Instance))
      {
        if (field.FieldType == typeof(SavedInputKey))
        {
          var savedInputKey = (SavedInputKey) field.GetValue(options);
          if (target != savedInputKey && inputKey == savedInputKey.value)
          {
            DebugLog.Info($"Already assigned: \n" +
                          $"\tSavedInputKey: {savedInputKey.name}-{savedInputKey.ToLocalizedString("KEYNAME")}\n" +
                          $"\tTarget: {target.name}-{target.ToLocalizedString("KEYNAME")}\n" +
                          $"\tInputKey: {inputKey.ToString()}");
            currentAssigned.Add(savedInputKey);
            newAssigned = true;
          }
        }
      }
      return newAssigned;
    }

    private void ResetShortcuts()
    {
      _options.ResetShortcuts();
      RefreshKeyMapping();
    }

    private void RefreshKeyMapping()
    {
      if (_editingBinding != null)
      {
        return;
      }

      for (var i = 0; i < childCount; i++)
      {
        var btn = components[i].Find<UITextComponent>("Binding");
        if (btn != null && btn.objectUserData is SavedInputKey)
        {
          var shortcut = (SavedInputKey) btn.objectUserData;
          btn.text = shortcut.ToLocalizedString("KEYNAME");
        }
      }
    }
  }
}
