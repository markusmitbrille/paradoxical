using Paradoxical.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Paradoxical.Services;

public interface IMediatorService
{
    void Register<T>(IMessageHandler<T> handler) where T : IMessage;
    void Unregister<T>(IMessageHandler<T> handler) where T : IMessage;
    void Send<T>(T message) where T : IMessage;
}

public class MediatorService : IMediatorService
{
    private Dictionary<Type, ArrayList> Handlers { get; } = new();

    private ArrayList GetHandlers<T>() where T : IMessage
    {
        if (Handlers.TryGetValue(typeof(T), out var handlers) == false)
        {
            handlers = new();
            Handlers.Add(typeof(T), handlers);
        }

        return handlers;
    }

    public void Register<T>(IMessageHandler<T> handler) where T : IMessage
    {
        var handlers = GetHandlers<T>();
        if (handlers.Contains(handler) == false)
        {
            handlers.Add(handler);
        }
    }

    public void Unregister<T>(IMessageHandler<T> handler) where T : IMessage
    {
        var handlers = GetHandlers<T>();
        if (handlers.Contains(handler) == true)
        {
            handlers.Remove(handler);
        }
    }

    public void Send<T>(T message) where T : IMessage
    {
        var handlers = GetHandlers<T>().Cast<IMessageHandler<T>>();
        foreach (var handler in handlers)
        {
            handler.Handle(message);
        }
    }
}
