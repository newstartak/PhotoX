using System;
using System.Collections.Generic;

public class EventManager
{
    private static Dictionary<Type, object> _events = new Dictionary<Type, object>();

    /// <returns>T에 해당하는 타입의 GenericEvent 반환</returns>
    public static GenericEvent<T> GetEvent<T>()
    {
        var type = typeof(T);

        if (!_events.TryGetValue(type, out var obj))
        {
            obj = new GenericEvent<T>();
            _events[type] = obj;
        }

        return (GenericEvent<T>)obj;
    }
}