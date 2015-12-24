using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ColossalFramework.UI;
using Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Extensions;
using JetBrains.Annotations;

namespace Craxy.CitiesSkylines.ToggleTrafficLights.Utils.Messaging
{
    //https://mvvmlight.codeplex.com/SourceControl/latest#GalaSoft.MvvmLight/GalaSoft.MvvmLight (PCL)/Messaging/Messenger.cs
    public sealed class Messenger
    {
        #region Default
        private static Messenger _default;
        private static readonly object Lock = new object();

        public static Messenger Default
        {
            get
            {
                if (_default == null)
                {
                    lock (Lock)
                    {
                        if (_default == null)
                        {
                            _default = new Messenger();
                        }
                    }
                }

                return _default;
            }
        }
        #endregion

        #region fields
        private readonly ReaderWriterLockSlim _listenersLock = new ReaderWriterLockSlim();
        private readonly IList<RegisteredListener> _listeners = new List<RegisteredListener>(0);
        #endregion

        #region listeners
        #region register
        public void Register<TMessage>([NotNull] Action<TMessage> action, bool receiveDerivedMessagesToo)
        {
            Register(Option.None<object>(), action, receiveDerivedMessagesToo);
        }
        public void Register<TMessage>([NotNull] object recipient, [NotNull] Action<TMessage> action, bool receiveDerivedMessagesToo)
        {
            Register(Option.Some(recipient), action, receiveDerivedMessagesToo);
        }
        public void Register<TMessage>(Option<object> recipient, [NotNull] Action<TMessage> action, bool receiveDerivedMessagesToo)
        {
            
            _listenersLock.EnterWriteLock();
            try
            {
                _listeners.Add(new RegisteredListener<TMessage>(Option.None<object>(), action, receiveDerivedMessagesToo));
            }
            finally
            {
                _listenersLock.ExitWriteLock();
            }
        }
        #endregion

        #region unregister
        public void UnregisterAllForRecipient(object recipient)
        {
            _listenersLock.EnterWriteLock();
            try
            {
                if (_listeners.IsEmpty())
                {
                    return;
                }

                _listeners.RemoveAll(l => l.Recipient.IsSome() && Object.ReferenceEquals(l.Recipient.GetValue(), recipient));
            }
            finally
            {
                _listenersLock.ExitWriteLock();
            }
        }
        public void Unregister<TMessage>(Action<TMessage> action, bool onlyFirst = false)
        {
            _listenersLock.EnterWriteLock();
            try
            {
                if (_listeners.IsEmpty())
                {
                    return;
                }

                for (var i = _listeners.Count - 1; i >= 0; i--)
                {
                    var v = _listeners[i] as RegisteredListener<TMessage>;
                    if (v != null && v.Action == action)
                    {
                        _listeners.RemoveAt(i);
                    }
                }
            }
            finally
            {
                _listenersLock.ExitWriteLock();
            }
        }
        #endregion
        //        public void Unregister<TMessage>([NotNull] Action<TMessage> action, bool onlyFirst = false)
        //        {
        //            lock (_listenersLock)
        //            {
        //                if (_listeners.IsEmpty())
        //                {
        //                    return;
        //                }
        //
        //                var ls = _listeners.Where(l => IsOfType<TMessage>(l.MessageType, l.ReceiveDerivedMessagesToo)).ToList();
        //
        //                foreach (var l in ls)
        //                {
        //                    _listeners.Remove(l);
        //
        //                    if (onlyFirst)
        //                    {
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        #endregion

        #region send
        public void Send<TMessage>([NotNull] TMessage message)
        {
            _listenersLock.EnterReadLock();
            try
            {
                _listeners.Where(l => IsOfType<TMessage>(l.MessageType, l.ReceiveDerivedMessagesToo))
                          .ForEach(l => l.ExecuteFromObject(message));
            }
            finally
            {
                _listenersLock.ExitReadLock();
            }
        }
        #endregion

        #region helper

        public static bool IsOfType<T>(Type t, bool derivedToo)
        {
            return t == typeof (T) || (derivedToo && typeof(T).IsSubclassOf(t));
        }
        #endregion

        #region registered recipients

        internal abstract class RegisteredListener : IImmutable
        {
            protected RegisteredListener([NotNull] Option<object> recipient, bool receiveDerivedMessagesToo)
            {
                Recipient = recipient;
                ReceiveDerivedMessagesToo = receiveDerivedMessagesToo;
            }

            public abstract Type MessageType { get; }
            public bool ReceiveDerivedMessagesToo { get; }
            /// <summary>
            /// Optional reference to the recipient. 
            /// Can be used to unregister all from a recipient without remembering the actions themself (which therefore can be anonymous methods)
            /// </summary>
            public Option<object> Recipient { get; } 

            public abstract void ExecuteFromObject(object parameter);
        }
        internal sealed class RegisteredListener<TMessage> : RegisteredListener
        {
            public RegisteredListener([NotNull] Action<TMessage> action, bool receiveDerivedMessagesToo) 
                : this(Option.None<object>(), action, receiveDerivedMessagesToo)
            {
            }

            public RegisteredListener([NotNull] object recipient, [NotNull] Action<TMessage> action, bool receiveDerivedMessagesToo)
                : this(Option.Some(recipient), action, receiveDerivedMessagesToo)
            {
            }
            public RegisteredListener(Option<object> recipient, [NotNull] Action<TMessage> action, bool receiveDerivedMessagesToo)
                : base(recipient, receiveDerivedMessagesToo)
            {
                Action = action;
            }

            public override Type MessageType => typeof (TMessage);
            public Action<TMessage> Action { get; }

            public override void ExecuteFromObject(object parameter)
            {
                var p = (TMessage)parameter;
                Action.Invoke(p);
            }
        }
        #endregion
    }
}