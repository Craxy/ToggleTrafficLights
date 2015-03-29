using System;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Tools;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using ICities;
using UnityEngine;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game
{
    public class Simulation : ThreadingExtensionBase
    {
        #region members

        private SelectToggleTrafficLightsToolButton _selectToolButton = null;
        private OpenViaKey _openViaKey = new OpenViaKey();
        #endregion

        public override void OnCreated(IThreading threading)
        {
            base.OnCreated(threading);
        }

        public override void OnReleased()
        {
            base.OnReleased();

            if (_selectToolButton != null)
            {
                _selectToolButton.Destroy();
                _selectToolButton = null;
            }
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            base.OnUpdate(realTimeDelta, simulationTimeDelta);

            if (managers.loading.IsGameMode())
            {
                if (_selectToolButton == null)
                {
                    _selectToolButton = new SelectToggleTrafficLightsToolButton();
                    DebugLog.Info("SelectToolButton created");
                }
                if (!_selectToolButton.Initialized)
                {
                    if (_selectToolButton.Initialize())
                    {
                        DebugLog.Info("SelectToolButton initialized");
                    }
                }

                _openViaKey.OpenIfAppropriate(_selectToolButton);
            }
        }

        private class OpenViaKey
        {
            private bool ShouldOpen()
            {
                return (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.T);
            }

            private bool _isOpening = false;
            public void OpenIfAppropriate(SelectToggleTrafficLightsToolButton button)
            {
                if(!_isOpening && ShouldOpen())
                {
                    _isOpening = true;
                }

                if (_isOpening)
                {
                    //tool might not yet be activated -> try activate until activated
                    Open(button);
                }
            }

            private void Open(SelectToggleTrafficLightsToolButton button)
            {
                if (button.Initialized)
                {
                    //hier findet eigentlicher toggle statt
                    button.ToggleShow();
                    _isOpening = false;
                }
                else
                {
                    //must be initialized first
                    //-> Click on button to show roadspanel
                    //but only of panel is not visible
                    var roadsPanel = UiHelper.FindComponent<UIComponent>("RoadsPanel");
                    if (!roadsPanel.isVisible)
                    {
                        SelectToggleTrafficLightsToolButton.ClickOnRoadsButton();
                    }
                }
            }
        }
    }
}