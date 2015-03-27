using System.Collections;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Game.UI
{
//    public abstract class SelectToolBase<TComponent, TTool>
//        where TComponent : UIComponent
//        where TTool : ToolBase
//    {
//        #region Properties
//        protected TComponent Component { get; private set; }
//
//        protected bool Initialized
//        {
//            get { return Component != null; }
//        }
//        #endregion
//
//        #region Initialize
//        public bool Initialize()
//        {
//            if (Component != null)
//            {
//                DebugLog.Info("Component already initialized");
//                return true;
//            }
//
//            if(!CanInitialize())
//            {
//                return false;
//            }
//
//            var parent = GetComponentParent();
//            if (parent == null)
//            {
//                DebugLog.Error("Parent is null");
//                return false;
//            }
//            var component = CreateComponent(parent);
//            if (component == null)
//            {
//                DebugLog.Error("Created Component is null");
//                return false;
//            }
//            SetupComponent(component);
//            //Setup Events
//            component.StartCoroutine()
//
//            Component = component;
//
//        }
//
//        protected virtual bool CanInitialize()
//        {
//            return true;
//        }
//        protected abstract UIComponent GetComponentParent();
//
//        protected virtual TComponent CreateComponent(UIComponent parent)
//        {
//            return parent.AddUIComponent<TComponent>();
//        }
//        /// <summary>
//        /// Is called BEFORE setting the object passed in <paramref name="component"/> to <see cref="Component"/>
//        /// 
//        /// Evens are created extra
//        /// </summary>
//        protected abstract void SetupComponent(TComponent component);
//
//        private IEnumerator SubscribeToEvents(TComponent component)
//        {
//            yield return null;
//            AddEvents(component);
//            DebugLog.Info("Events added");
//        }
//
//        protected virtual void AddEvents(TComponent component)
//        {
//            component.eventClick
//        }
//        #endregion
//    }
}