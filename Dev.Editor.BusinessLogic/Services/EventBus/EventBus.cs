using Dev.Editor.BusinessLogic.Models.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.EventBus
{
    internal class EventBus : IEventBus
    {
        private abstract class BaseListenerInfo
        {
            public BaseListenerInfo(bool notifySelf)
            {
                NotifySelf = notifySelf;
            }

            public abstract bool ContainsListener(object listener);
            public abstract void Receive(object @event);

            public bool NotifySelf { get; }
        }

        private class ListenerInfo<T> : BaseListenerInfo
                where T : BaseEvent
        {
            private readonly IEventListener<T> listener;

            public ListenerInfo(IEventListener<T> listener, bool notifySelf)
              : base(notifySelf)
            {
                this.listener = listener;
            }

            public override bool ContainsListener(object listener)
            {
                return this.listener == listener;
            }

            public override void Receive(object @event)
            {
                listener.Receive((T)@event);
            }
        }

        private readonly Dictionary<Type, object> listeners = new Dictionary<Type, object>();

        public void Register<T>(IEventListener<T> listener, bool notifySelf = false) where T : BaseEvent
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            Type t = typeof(T);

            List<BaseListenerInfo> eventListeners;
            if (!listeners.ContainsKey(t))
            {
                eventListeners = new List<BaseListenerInfo>();
                listeners[t] = eventListeners;
            }
            else
            {
                eventListeners = listeners[t] as List<BaseListenerInfo>;
                if (eventListeners == null)
                    throw new InvalidOperationException("Invalid list type in dictionary!");
            }

            if (!eventListeners.Any(li => li.ContainsListener(listener)))
            {
                var listenerInfo = new ListenerInfo<T>(listener, notifySelf);
                eventListeners.Add(listenerInfo);
            }
        }

        public void Unregister<T>(IEventListener<T> listener) where T : BaseEvent
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            Type t = typeof(T);

            if (!listeners.ContainsKey(t))
                return;

            List<BaseListenerInfo> eventListeners = listeners[t] as List<BaseListenerInfo>;

            var info = eventListeners.FirstOrDefault(el => el.ContainsListener(listener));
            if (info != null)
                eventListeners.Remove(info);

            if (eventListeners.Count == 0)
                listeners.Remove(t);
        }

        public void UnregisterAll(object listener)
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            foreach (var pair in listeners)
            {
                var list = pair.Value as List<BaseListenerInfo>;

                int i = 0;
                while (i < list.Count)
                {
                    if ((list[i]).ContainsListener(listener))
                    {
                        list.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        public void Send<T>(object sender, T @event) where T : BaseEvent
        {
            Type t = typeof(T);

            while (t != typeof(object))
            {
                if (listeners.ContainsKey(t))
                {
                    List<BaseListenerInfo> eventListeners = listeners[t] as List<BaseListenerInfo>;

                    foreach (var info in eventListeners)
                    {
                        if (info.NotifySelf || !info.ContainsListener(sender))
                            info.Receive(@event);
                    }
                }

                t = t.BaseType;
            }
        }
    }
}
