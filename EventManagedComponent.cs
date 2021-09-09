using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class defined to describe an "EventList" which is essentially a dictionary of string, event action key-value pairs. 
/// </summary>
public class EventList : Dictionary<string, UnityAction<object>> {}

/// <summary>
/// Helper class that provides subscribing and unsubscribing to and from events behaviours in the form of overriding one method, "SetEvents"
/// </summary>
public class EventManagedComponent : MonoBehaviour
{
    /// <summary>
    /// EventList - a dictionary of all of the event names as keys and the actions that should be called upon event trigger as the values
    /// </summary>
    public EventList Events;

    /// <summary>
    /// Setup the subscription and unsubscription to events on awake
    /// </summary>
    protected virtual void Awake()
    {
        SetEvents();
    }

    /// <summary>
    /// Default method will alert the developer that no events have been set, should be overriden by sub classes
    /// </summary>
    protected virtual void SetEvents()
    {
        Events = new EventList();
        LogError();
    }

    /// <summary>
    /// Subscribe to all events on enable
    /// </summary>
    protected virtual void OnEnable()
    {
        if (Events == null || Events.Count < 1)
        {
            LogError();
            return;
        }

        foreach (var Event in Events)
        {
            EventManager.StartListening(Event.Key, Event.Value);
        }
    }

    /// <summary>
    /// Unsubscribe to all events on disable
    /// </summary>
    protected virtual void OnDisable()
    {
        foreach (var Event in Events)
        {
            EventManager.StopListening(Event.Key, Event.Value);
        }
    }

    /// <summary>
    /// Inform the developer that no events have been setup 
    /// </summary>
    void LogError()
    {
        Debug.LogError("Event Managed Component on " + gameObject.name + " doesn't have any events, add events by overriding the Awake method or change to MonoBehaviour");    
    }
}
