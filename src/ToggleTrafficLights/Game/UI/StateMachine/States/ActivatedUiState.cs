using System;
using System.Linq;
using ColossalFramework;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.Menu;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;
using Object = UnityEngine.Object;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI.StateMachine.States
{
    public class ActivatedUiState : ActivatedState
    {
        #region fields
        private BatchUi _ui = null;

        #endregion

        #region Overrides of StateBase

        public override State State
        {
            get { return State.ActivatedUiState; }
        }


        public override void OnEntry()
        {
            base.OnEntry();

            if (_ui == null)
            {
                //gets enabled automatically
                // after (including) second load in session Singleton<ToolManager>.instance is null. why?
                Singleton<ToolManager>.Ensure();
                var toolControl = Singleton<ToolManager>.instance;
                _ui = toolControl.GetComponent<BatchUi>() ?? toolControl.gameObject.AddComponent<BatchUi>();

                
                //Simulation error: Object reference not set to an instance of an object
                //  at TransportManager.SetPatchDirty (Int32 x, Int32 z, .TransportInfo info, Boolean canCreate) [0x00000] in <filename unknown>:0 
                //  at TransportManager.SetPatchDirty (UInt16 segmentID, .TransportInfo info, Boolean canCreate) [0x00000] in <filename unknown>:0 
                //  at FlightPathAI.SegmentLoaded (UInt16 segmentID, .NetSegment& data) [0x00000] in <filename unknown>:0 
                //  at NetManager+Data.AfterDeserialize (ColossalFramework.IO.DataSerializer s) [0x00000] in <filename unknown>:0 
                //  at ColossalFramework.IO.DataSerializer.Deserialize[Data] (System.IO.Stream stream, Mode mode, ColossalFramework.IO.LegacyResolverHandler customResolver) [0x00000] in <filename unknown>:0 
                //  at ColossalFramework.IO.DataSerializer.Deserialize[Data] (System.IO.Stream stream, Mode mode) [0x00000] in <filename unknown>:0 
                //  at LoadingManager+<LoadSimulationData>c__Iterator5B.MoveNext () [0x00000] in <filename unknown>:0 
                //  at AsyncTask.Execute () [0x00000] in <filename unknown>:0   [Core]
                //
                // Simulation error: Object reference not set to an instance of an object
                //  at NaturalResourceManager.SimulationStepImpl (Int32 subStep) [0x00000] in <filename unknown>:0 
                //  at SimulationManagerBase`2[Manager,Properties].SimulationStep (Int32 subStep) [0x00000] in <filename unknown>:0 
                //  at NaturalResourceManager.ISimulationManager.SimulationStep (Int32 subStep) [0x00000] in <filename unknown>:0 
                //  at SimulationManager.SimulationStep () [0x00000] in <filename unknown>:0 
                //  at SimulationManager.SimulationThread () [0x00000] in <filename unknown>:0   [Core]
                // after (incl) 2nd load in session
//                _ui = ToolsModifierControl.toolController.gameObject.GetComponent<BatchUi>()
//                      ?? ToolsModifierControl.toolController.gameObject.AddComponent<BatchUi>();

                // prevents 2nd load in session
//                var toolControl = Singleton<LoadingManager>.instance;
//                _ui = toolControl.GetComponent<BatchUi>() ?? toolControl.gameObject.AddComponent<BatchUi>();

            }
            _ui.enabled = true;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (Button != null)
            {
                SetActivedUiStateSprites(Button);
            }
        }

        public override void OnExit()
        {
            if (_ui != null)
            {
                _ui.enabled = false;
            }

            base.OnExit();
        }

        #region Overrides of ActivatedState

        public override void Destroy()
        {
            if (_ui != null)
            {
                _ui.enabled = false;
                Object.Destroy(_ui.gameObject);
            }
            _ui = null;

            base.Destroy();
        }

        #endregion

        public override Command? CheckCommand()
        {
            var cmd = base.CheckCommand();

            if (cmd.HasValue && cmd.Value != Command.RightClickToolButton)
            {
                return cmd;
            }

            if (LeftClick)
            {
                return Command.LeftClickOnToolButton;
            }

            return null;
        }

        #endregion

        #region Overrides of ActivatedState
        protected override string ButtonName
        {
            get { return ButtonBaseName + "ActivatedUi"; }
        }
        #endregion

        protected void SetActivedUiStateSprites(UIButton btn)
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

    }
}