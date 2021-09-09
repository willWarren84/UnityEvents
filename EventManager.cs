using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Defining what the signature of an event looks like, basically allow objects as parameters
/// </summary>
public class ManagedEvent : UnityEvent<object> { }

/// <summary>
/// Event manager class that manages tracking events and informing the correct listeners when events are triggered etc.
///
/// The benefit of using the Action<object> signature is that any object can be sent as a parameter and interpreted on the listener
/// One minor drawback of using this is the listener need to know the type of parameter it is expecting
/// On the whole it provides much more functionality than a system that does not allow parameters and is cleaner than writing different managers for each parameter type (or forcing all to one type)
/// </summary>
public class EventManager : MonoBehaviour
{
    /// <summary>
    /// Collection of all events and listeners
    /// </summary>
    private Dictionary<string, ManagedEvent> eventDictionary;
    /// <summary>
    /// Static reference to self, see singleton pattern
    /// </summary>
    private static EventManager eventManager;
    /// <summary>
    /// Static instance reference
    /// </summary>
    public static EventManager instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;
                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }
            return eventManager;
        }
    }
    
    /// <summary>
    /// Initialize the eventmanager with an empty event dictionary
    /// </summary>
    void Init()
    {
        DontDestroyOnLoad(gameObject);
        //gameObject.tag = "DontDestroy";
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, ManagedEvent>();
        }
    }
    
    /// <summary>
    /// Add a listener / event pair to the collection.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StartListening(string eventName, UnityAction<object> listener)
    {
        if (instance.eventDictionary.TryGetValue(eventName, out ManagedEvent thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new ManagedEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }
    
    /// <summary>
    /// Remove a listener / event pair from the collection
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="listener"></param>
    public static void StopListening(string eventName, UnityAction<object> listener)
    {
        if (eventManager == null) return;
        ManagedEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }
    
    /// <summary>
    /// Event has been triggered, inform the relevant listener.
    /// If the event is a networked one get the network code and raise it over the network.
    /// </summary>
    /// <param name="eventName"></param>
    /// <param name="param"></param>
    public static void TriggerEvent(string eventName, object param)
    {
        ManagedEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(param);
        }
    }
}