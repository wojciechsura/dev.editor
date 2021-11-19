using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Spooksoft.VisualStateManager.Conditions
{
    internal class NotificationRegistry
    {
        private class NotificationEntry
        {
            public NotificationEntry(MemberAccessChainNode node, string propertyName)
            {
                Node = node;
                PropertyName = propertyName;
            }

            public MemberAccessChainNode Node { get; }
            public string PropertyName { get; }
        }

        private Dictionary<INotifyPropertyChanged, List<NotificationEntry>> notificationRegistrations = new Dictionary<INotifyPropertyChanged, List<NotificationEntry>>();

        private void HandleTargetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (notificationRegistrations.TryGetValue(sender as INotifyPropertyChanged, out List<NotificationEntry> registrations))
            {
                bool updated = false;

                foreach (var registration in registrations.Where(r => r.PropertyName == e.PropertyName))
                {
                    registration.Node.NotifyPropertyChanged();
                    updated = true;
                }

                if (updated)
                    ValueChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void Register(MemberAccessChainNode node, string property, INotifyPropertyChanged target)
        {
            if (notificationRegistrations.TryGetValue(target, out List<NotificationEntry> registrations))
            {
                registrations = notificationRegistrations[target];
            }
            else
            {
                registrations = new List<NotificationEntry>();
                notificationRegistrations.Add(target, registrations);
                target.PropertyChanged += HandleTargetPropertyChanged;
            }

            registrations.Add(new NotificationEntry(node, property));
        }

        public void Unregister(MemberAccessChainNode node, string property, INotifyPropertyChanged target)
        {
            if (notificationRegistrations.ContainsKey(target))
            {
                var registrations = notificationRegistrations[target];
                var registration = registrations.FirstOrDefault(r => r.Node == node && r.PropertyName == property);
                if (registration != null)
                    registrations.Remove(registration);

                if (registrations.Count == 0)
                {
                    target.PropertyChanged -= HandleTargetPropertyChanged;
                    notificationRegistrations.Remove(target);
                }
            }
        }

        public event EventHandler ValueChanged;
    }
}
