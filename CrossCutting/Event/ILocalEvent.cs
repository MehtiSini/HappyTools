using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HappyTools.CrossCutting.Event
{

    // Event marker interface
    public interface ILocalEvent { }

    // Event handler interface
    public interface ILocalEventHandler<in TEvent> where TEvent : ILocalEvent
    {
        Task HandleAsync(TEvent eventData);
    }

    // Local Event Bus interface
    public interface ILocalEventBus
    {
        Task PublishAsync<TEvent>(TEvent eventData) where TEvent : ILocalEvent;
        void Subscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : ILocalEvent;
    }

    // Simple in-memory implementation
    public class LocalEventBus : ILocalEventBus
    {
        private readonly ConcurrentDictionary<Type, List<object>> _handlers = new();

        public void Subscribe<TEvent>(ILocalEventHandler<TEvent> handler) where TEvent : ILocalEvent
        {
            var type = typeof(TEvent);
            var handlers = _handlers.GetOrAdd(type, _ => new List<object>());
            lock (handlers)
            {
                handlers.Add(handler);
            }
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : ILocalEvent
        {
            var type = typeof(TEvent);
            if (_handlers.TryGetValue(type, out var handlers))
            {
                List<Task> tasks = new();
                lock (handlers)
                {
                    foreach (var handler in handlers)
                    {
                        if (handler is ILocalEventHandler<TEvent> h)
                        {
                            tasks.Add(h.HandleAsync(@event));
                        }
                    }
                }
                await Task.WhenAll(tasks);
            }
        }
    }
}