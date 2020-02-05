using Dev.Editor.BusinessLogic.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.EventBus
{
    class EventBus : IEventBus
    {
        private class ListenerInfo<T>
            where T : BaseEvent
        {
            public ListenerInfo(IEventListener<T> listener, bool notifySelf)
            {
                Listener = listener;
                NotifySelf = notifySelf;
            }

            public IEventListener<T> Listener { get; }
            public bool NotifySelf { get; }
        }

        private readonly Dictionary<Type, object> listeners = new Dictionary<Type, object>();

        public void Register<T>(IEventListener<T> listener, bool notifySelf = false) where T : BaseEvent
        {
            if (listener == null)
                throw new ArgumentNullException(nameof(listener));

            Type t = typeof(T);

            List<ListenerInfo<T>> eventListeners;
            if (!listeners.ContainsKey(t))
            {
                eventListeners = new List<ListenerInfo<T>>();
                listeners[t] = eventListeners;
            }
            else
            {
                eventListeners = listeners[t] as List<ListenerInfo<T>>;
                if (eventListeners == null)
                    throw new InvalidOperationException("Invalid list type in dictionary!");
            }

            if (!eventListeners.Any(li => li.Listener == listener))
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

            List<ListenerInfo<T>> eventListeners = listeners[t] as List<ListenerInfo<T>>;
            
            var info = eventListeners.FirstOrDefault(el => el.Listener == listener);
            if (info != null)
                eventListeners.Remove(info);

            if (eventListeners.Count == 0)
                listeners.Remove(t);
        }

        public void Send<T>(object sender, T @event) where T : BaseEvent
        {
            Type t = typeof(T);
            if (listeners.ContainsKey(t))
            {
                List<ListenerInfo<T>> eventListeners = listeners[t] as List<ListenerInfo<T>>;
                
                foreach (var info in eventListeners)
                {
                    if (!info.NotifySelf && info.Listener == sender)
                        continue;

                    info.Listener.Receive(@event);
                }                    
            }
        }
    }
}
