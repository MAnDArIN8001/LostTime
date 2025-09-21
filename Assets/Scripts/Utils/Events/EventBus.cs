using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.Events
{
    public class EventBus : IDisposable
    {
        private readonly Dictionary<Type, object> _events = new();

        public void Subscribe<T>(Action<T> action)
        {
            if (!_events.TryGetValue(typeof(T), out var events))
            {
                events = new List<Action<T>>();
                
                _events[typeof(T)] = events;
            }

            ((List<Action<T>>)events).Add(action);
        }

        public void Unsubscribe<T>(Action<T> action)
        {
            if (!_events.TryGetValue(typeof(T), out var events))
            {
                Debug.LogWarning($"The Event Bus {this} doesn't contains any handler of type {typeof(T)}:{action}");
                
                return;
            }
            
            if (events is not List<Action<T>> eventsList)
            {
                Debug.LogWarning($"The Event Bus {this} cant cast {events} to Action<{typeof(T)}>");
                
                return;
            }

            eventsList.Remove(action);
        }

        public void Publish<T>(T eventData)
        {
            if (!_events.TryGetValue(typeof(T), out var events))
            {
                return;
            }
            
            var listeners = ((List<Action<T>>)events);

            for (var i = 0; i < listeners.Count; i++)
            {
                try
                {
                    listeners[i].Invoke(eventData);
                }
                catch (Exception exception)
                {
                    UnityEngine.Debug.LogError($"EventBus: exception in event handler {typeof(T)}: {exception}");
                }
            }
        }

        public void Dispose()
        {
            _events.Clear();
        }
    }
}