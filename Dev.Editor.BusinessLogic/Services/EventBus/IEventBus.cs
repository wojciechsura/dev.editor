using Dev.Editor.BusinessLogic.Models.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.BusinessLogic.Services.EventBus
{
    public interface IEventBus
    {
        void Register<T>(IEventListener<T> listener, bool notifySelf = false) where T : BaseEvent;
        void Send<T>(object sender, T @event) where T : BaseEvent;
        void Unregister<T>(IEventListener<T> listener) where T : BaseEvent;
    }
}
