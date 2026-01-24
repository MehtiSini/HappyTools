using HappyTools.DependencyInjection.Contracts;
using Microsoft.Extensions.DependencyInjection;
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

    public class LocalEventBus : ILocalEventBus , IScopedDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public LocalEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PublishAsync<TEvent>(TEvent eventData)
            where TEvent : ILocalEvent
        {
            using var scope = _serviceProvider.CreateScope();

            var handlers = scope.ServiceProvider
                .GetServices<ILocalEventHandler<TEvent>>();

            var tasks = handlers.Select(h => h.HandleAsync(eventData));

            await Task.WhenAll(tasks);
        }

        // Not needed anymore
        public void Subscribe<TEvent>(ILocalEventHandler<TEvent> handler)
            where TEvent : ILocalEvent
        {
            throw new NotSupportedException("Use DI-based handlers");
        }
    }

}