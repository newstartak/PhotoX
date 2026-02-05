using System;
using System.Collections.Generic;

public class GenericListener<T>
{
    public string Channel;
    public Action<T> Callback;
}

public class GenericEvent<T>
{
    private List<GenericListener<T>> _listeners = new List<GenericListener<T>>();

    public void Subscribe(Action<T> callback, string channel)
    {
        _listeners.Add(new GenericListener<T> { Channel = channel, Callback = callback });
    }

    public void Unsubscribe(Action<T> callback, string channel)
    {
        _listeners.RemoveAll(l => l.Channel == channel && l.Callback == callback);
    }

    public void Publish(T obj, string channel)
    {
        foreach (var listener in _listeners)
        {
            if (listener.Channel == channel)
            {
                listener.Callback?.Invoke(obj);
            }
        }
    }

    public void PublishToAll(T obj)
    {
        foreach (var listener in _listeners)
        {
            listener.Callback?.Invoke(obj);
        }
    }
}