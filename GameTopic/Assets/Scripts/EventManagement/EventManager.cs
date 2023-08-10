using System.Collections.Generic;
using System;

public class EventManager: Singleton<EventManager>{
    private readonly Dictionary<string, Delegate> _eventDictionary = new();
    /// <summary>
    /// Adds a listener to the event. If the event does not exist, it creates it.
    /// </summary>
    /// <param name="eventName"> The name of the event to listen to.</param>
    /// <param name="listener"> The listener to add to the event.</param>
    /// <exception cref="Exception"> Throws an exception if the event already has a listener of a different type.</exception>
    public void AddListener(string eventName, Delegate listener){
        if (_eventDictionary.TryGetValue(eventName, out var thisEvent)) {
            if (listener.GetType() != thisEvent.GetType()) {
                throw new ArgumentException($"Event {eventName} has a listener of type {thisEvent.GetType()} and cannot add a listener of type {listener.GetType()}.");
            }
            _eventDictionary[eventName] = Delegate.Combine(thisEvent, listener);
            
        } else {
            _eventDictionary.Add(eventName, listener);
        }
    }
    public void RemoveListener(string eventName, Delegate listener){
        if (Instance._eventDictionary.TryGetValue(eventName, out var thisEvent)) {
            if (listener.GetType() != thisEvent.GetType()) {
                throw new ArgumentException($"Event {eventName} has a listener of type {thisEvent.GetType()} and cannot remove a listener of type {listener.GetType()}.");
            }
            var newDelegate = Delegate.Remove(thisEvent, listener);
            if (newDelegate == null) {
                Instance._eventDictionary.Remove(eventName);
            } else {
                Instance._eventDictionary[eventName] = newDelegate;
            }
        }
    }
    public void TriggerEvent(string eventName, params object[] args) {
        if (_eventDictionary.TryGetValue(eventName, out var thisEvent)) {
            try{
                thisEvent.DynamicInvoke(args);
            }catch(Exception){
                throw new ArgumentException($"Event {eventName} has a listener of type {thisEvent.GetType()} and cannot be invoked with args of type {args.GetType()}.");
            }
            
        }
    }
    public void ClearAllListeners(){
        _eventDictionary.Clear();
    }
}