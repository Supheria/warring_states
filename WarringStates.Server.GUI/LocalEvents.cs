using System;
using System.Collections.Concurrent;
using WarringStates.Events;

namespace WarringStates.Server.GUI;

internal partial class LocalEvents
{
    static ConcurrentDictionary<string, Delegate> EventMap { get; } = [];

    public static bool TryAddListener(string eventType, Callback callback)
    {
        try
        {
            var success = EventMap.TryAdd(eventType, callback);
            if (!EventMap.TryGetValue(eventType, out var exist))
                return false;
            if (success)
                return true;
            EventMap[eventType] = (Callback)exist + callback;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryAddListener<TArgs>(string eventType, Callback<TArgs> callback) where TArgs : EventArgs
    {
        try
        {
            var success = EventMap.TryAdd(eventType, callback);
            if (!EventMap.TryGetValue(eventType, out var exist))
                return false;
            if (success)
                return true;
            EventMap[eventType] = (Callback<TArgs>)exist + callback;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryRemoveListener(string eventType, Callback callback)
    {
        try
        {
            if (!EventMap.TryGetValue(eventType, out var exist))
                return false;
            exist = (Callback)exist - callback;
            if (exist is null)
                return EventMap.TryRemove(eventType, out _);
            EventMap[eventType] = exist;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryRemoveListener<TArgs>(string eventType, Callback<TArgs> callback) where TArgs : EventArgs
    {
        try
        {
            if (!EventMap.TryGetValue(eventType, out var exist))
                return false;
            exist = (Callback<TArgs>)exist - callback;
            if (exist is null)
                return EventMap.TryRemove(eventType, out _);
            EventMap[eventType] = exist;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryBroadcast(string eventType)
    {
        try
        {
            if (!EventMap.TryGetValue(eventType, out var callback))
                return false;
            callback.DynamicInvoke();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static bool TryBroadcast<TArgs>(string eventType, TArgs args) where TArgs : EventArgs
    {
        try
        {
            if (!EventMap.TryGetValue(eventType, out var callback))
                return false;
            callback.DynamicInvoke(args);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
