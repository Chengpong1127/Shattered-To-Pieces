using System.Collections.Generic;
using System;

public class EventManager: Singleton<EventManager>{
    private readonly Dictionary<string, Action> _eventDictionary = new();
    public void StartListening(string eventName, Action listener) {
        if (Instance._eventDictionary.TryGetValue(eventName, out var thisEvent)) {
            thisEvent += listener;
            Instance._eventDictionary[eventName] = thisEvent;
        } else {
            thisEvent += listener;
            Instance._eventDictionary.Add(eventName, thisEvent);
        }
    }
    public void StopListening(string eventName, Action listener) {
        if (Instance._eventDictionary.TryGetValue(eventName, out var thisEvent)) {
            thisEvent -= listener;
            Instance._eventDictionary[eventName] = thisEvent;
        }
    }
    public void TriggerEvent(string eventName) {
        if (Instance._eventDictionary.TryGetValue(eventName, out var thisEvent)) {
            thisEvent?.Invoke();
        }
    }
}