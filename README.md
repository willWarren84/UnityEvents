I find that these two classes have proven invaluable for development of several applications.
Typically Unity is taught to become a monolithic network of references between components and GameObjects. My issue with this is the lack of adaptability this provides.
By using an event manager to de-couple components from one-another we're able to raise, and respond to, events easily meaning that we can extend and replace functionality seamlessly.

# Getting started using the EventManager

In order to start seeing the benefit of the event manager create an empty GameObject in any scene and add the EventManager component to it (alternatively add the EventManager component to any existing GameObject in your scene).

With the EventManager added we simply need 2 more components, one to trigger Events and one to respond.

Raising events is as easy as naming an event and passing a parameter:

'''
EventManager.TriggerEvent(string eventName, object anyParameter);
'''
###e.g.
'''
EventManager.TriggerEvent("MyEventName", null);
'''

In the above example, any components that are listening for the event named "MyEventName" will now be triggered into responding.

To listen for an event we could simply start listening like so:

'''
EventManager.StartListening(string eventName, UnityAction<object> delegateMethod);
'''

A UnityAction<object> is a single object argument delegate method, so we could pass an existing method like so:
'''
public class MyExampleComponent : MonoBehaviour
{
  void OnEnable()
  {
    EventManager.StartListening("MyEventName", MyMethod);
  }

  void MyMethod(object eventParameter)
  {
      //Do some things
  }
}
'''

Now, when the MyEventName event is triggered the MyExampleComponent's MyMethod method will run.

One issue with this approach is that you have to both start **and stop** listening for events so, even in a small project, you will quickly begin to get fed up of typing "EventManager.StartListening(..." and "EventManager.StopListening(" in each component which you want to respond to events, this is why the EventManagedComponent class was created.


# Using EventManagedComponent

EventManagedComponent is simply a helper class which inherits from MonoBehaviour and will automatically start and stop listening for events which are in it's Events EventList. EventManagedComponent is intended to be inherited by your component and have its SetEvents method overridden (on a side note, if / when Unity support C# 8 this will probably be updated to an [interface with a default method implementation](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-8.0/default-interface-methods)).

Overriding the SetEvents Method can be done like so:
'''
protected override void SetEvents()
{
  Events = new EventList
  {
      { string eventName1, UnityAction<object> delegateMethod1 },
      { string eventName2, UnityAction<object> delegateMethod2 },
      ...
  };
}
'''
###e.g.
'''
public class MySimplerComponent : EventManagedComponent
{
  protected override void SetEvents()
  {
    Events = new EventList
    {
        { "MyFirstEvent", MyFirstMethod },
        { "MySecondEvent", MySecondMethod }        
    };
  }

  void MyFirstMethod(object eventParameter)
  {
      //Do some things
  }

  void MySecondMethod(object eventParameter)
  {
      //Do some other things
  }
}
'''

Typically other Unity EventManagers support providing a specific type of parameter, this one is much more flexible because it supports passing an object and all data types' root object is an object. This does mean that the listening component does need to know what type of data that it expects to receive and handle the cast but
a) if it didn't know what type of data it was going to receive then it wouldn't know how to deal with it, and
b) the cast is very straightforward using some C# syntactic sugar...

##Checking your parameter

To ensure that the method that's receiving an event can handle the parameter data and to cast that data to it's original data type I tend to use the following syntax:
'''
public class MySimplerComponent : EventManagedComponent
{
  protected override void SetEvents()
  {
    Events = new EventList
    {
        { "MyFirstEvent", MyFirstMethod }                
    };
  }

  void SomeMethodThatTriggersAnEvent()
  {
    string parameter = "Read this!";
    EventManager.TriggerEvent("MyFirstEvent", parameter);
  }


  void MyFirstMethod(object eventParameter)
  {
    if(!(eventParameter is string textMessage)) return;

    Debug.Log(textMessage);
  }
}
'''

Now, when the SomeMethodThatTriggersAnEvent method of the MySimplerComponent component is called we will see "Read this!" logged to the console.
