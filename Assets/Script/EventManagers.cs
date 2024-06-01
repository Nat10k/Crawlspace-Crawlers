using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManagers
{
    private static Dictionary<string, List<Listener>> eventDict = new Dictionary<string, List<Listener>>();
    
    public static void Register(string name, Listener listener)
    {
        if (!eventDict.ContainsKey(name))
        {
            eventDict.Add(name, new List<Listener>());
        }
        eventDict[name].Add(listener);
    }

    public static void Unregister(string name, Listener listener)
    {
        if (eventDict.ContainsKey(name))
        {
            if (eventDict[name].Contains(listener))
            {
                eventDict[name].Remove(listener);
                if (eventDict[name].Count == 0)
                {
                    eventDict.Remove(name);
                }
            }
        }
    }

    public static void InvokeEvent(string eventName)
    {
        if (eventDict.ContainsKey(eventName))
        {
            foreach (Listener listener in eventDict[eventName])
            {
                listener.invoke();
            }
        }
    }
}

public class Listener
{
    public delegate void InvokeMessage();
    public InvokeMessage invoke;
}
