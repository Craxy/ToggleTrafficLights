using System;
using System.Threading;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Disposable
{
    public static class DisposableExtensions
    {
        public static IDisposable ToUsing<T>(T obj, Action<T> onInitialize, Action<T> onDispose)
        {
            return new Disposable<T>(obj, onInitialize, onDispose);
        }
        public static IDisposable ToUsing<T>(T obj, Action<T> onDispose)
        {
            return new Disposable<T>(obj, Disposable<T>.DoNothing, onDispose);
        }

        private class Disposable<T> : IDisposable
        {
            public static readonly Action<T> DoNothing = delegate { };

            public Disposable([NotNull] T obj, [NotNull] Action<T> onInitialize, [NotNull] Action<T> onDispose)
            {
                _obj = obj;
                _onDispose = onDispose;

                onInitialize(obj);
            }

            private readonly Action<T> _onDispose;
            private readonly T _obj;

            #region Implementation of IDisposable
            public void Dispose()
            {
                _onDispose(_obj);
            }
            #endregion
        }
    }
}