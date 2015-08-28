using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.UI.SideMenu.Pages
{
    public class UsageTab : UIPanel
    {

        #region settings

        private float _left = 5.0f;
        private float _top = 5.0f;
        private float _topNextLineIndention = 2.0f;
        private float _leftRowIndention = 5.0f;
        private float _minIndentionBetweenTitleAndSeparator = 2.0f;
        private float _indentionBetweenSeparatorAndContent = 2.0f;
        private string _rowSeparator = ":";
        private float _rowTextScale = 0.8125f;
        #endregion

        #region Overrides of UIComponent

        public override void Start()
        {
            base.Start();

            name = "UsagePanel";

            relativePosition = new Vector3(0.0f, 0.0f);
            size = parent.size;

//            var top = 5.0f;
//            var left = 2.0f;
//            var next = 3.0f;
//
//            var indention = 10.0f;
//            var titleLeft = left + indention;
//            var contentLeft = 150.0f;

            var rows = new List<Tuple<UILabel, UILabel, UILabel>>();
            {
                AddHeader("Menus");
                AddRow("Left Click on button", "Activate Toggle tool").AddTo(rows);
                AddRow("Right Click on button", "Activate Toggle tool and show this menu").AddTo(rows);

                _top += _topNextLineIndention*1.75f;

                AddHeader("Toggle intersections");
                AddRow("Left Click", "Toggle Traffic Lights").AddTo(rows);
                AddRow("Right Click", "Reset to default").AddTo(rows);

                _top += _topNextLineIndention*1.75f;

                AddHeader("Shortcuts");
                AddRow("Ctrl+T", "(De)Activate TTL (acts like clicking on TTL button)").AddTo(rows);
                AddRow("Ctrl+Shift+T", "(De)Activate TTL without opening the Roads Menu").AddTo(rows);
                AddRow(Options.InputKeys.ElevationDown.ToString(), "Activate TTL (acts like clicking on TTL button)").AddTo(rows);
                _top += _topNextLineIndention*0.5f;
                AddRow(Options.InputKeys.ElevationDown.ToString(), "Only Underground").AddTo(rows);
                AddRow(Options.InputKeys.ElevationUp.ToString(), "Only Overground").AddTo(rows);
                AddRow($"{Options.InputKeys.ElevationDown}+{Options.InputKeys.ElevationUp}", "Both Overground & Underground").AddTo(rows);

                _top += _topNextLineIndention*5.0f;

                AddHeader("A detailed description is available on the GitHub page of this mod: https://github.com/Craxy/ToggleTrafficLights");
            }
            AlignRows(rows);
        }

        #endregion

        private UILabel AddHeader([NotNull] string header)
        {
            var lbl = AddUIComponent<UILabel>();
            lbl.text = header;
            lbl.textScale = 1.0f;
            lbl.relativePosition = new Vector3(_left, _top);

            _top += lbl.height + (_topNextLineIndention * 1.25f);

            return lbl;
        }

        private Tuple<UILabel, UILabel, UILabel> AddRow([NotNull] string title, [NotNull] string content)
        {
            var lblTitle = AddUIComponent<UILabel>();
            lblTitle.text = title;
            lblTitle.textScale = _rowTextScale;
            lblTitle.relativePosition = new Vector3(_left + _leftRowIndention, _top);

            var lblSeparator = AddUIComponent<UILabel>();
            lblSeparator.text = _rowSeparator;
            lblSeparator.textScale = _rowTextScale;
            lblSeparator.relativePosition = new Vector3(lblTitle.relativePosition.x + lblTitle.width + _minIndentionBetweenTitleAndSeparator, _top);

            var lblContent = AddUIComponent<UILabel>();
            lblContent.text = content;
            lblContent.textScale = _rowTextScale;
            lblContent.wordWrap = true;
            //position is later updated to align nicely with others
            lblContent.relativePosition = new Vector3(lblSeparator.relativePosition.x + lblSeparator.width + _indentionBetweenSeparatorAndContent, _top);

            _top += Mathf.Max(lblTitle.height, lblContent.height) + _topNextLineIndention;

            return Tuple.Create(lblTitle, lblSeparator, lblContent);
        }

        private void AlignRows([NotNull] IList<Tuple<UILabel, UILabel, UILabel>> rows)
        {
            // first: find longest title
            // no .Max(..): selectes just the selector and not the label
            var max = rows.Select(t => t.Item1)
                          .OrderByDescending(lbl => lbl.width)
                          .First();
            //then align contents to this width
            var start = max.relativePosition.x + max.width + _minIndentionBetweenTitleAndSeparator;
            foreach (var r in rows)
            {
                var separator = r.Item2;
                var content = r.Item3;

                separator.relativePosition = new Vector3(start, separator.relativePosition.y);
                content.relativePosition = new Vector3(separator.relativePosition.x + separator.width + _indentionBetweenSeparatorAndContent, separator.relativePosition.y);
            }
        }

    }

}