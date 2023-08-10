using System;


public static class EventTriggerExtension{
    public static void TriggerEvent(this object sender, string eventName) {
        EventManager.Instance.TriggerEvent(eventName);
    }

    public static void StartListening(this object self, string eventName, Action action){
        EventManager.Instance.StartListening(eventName, action);
    }

    public static void StopListening(this object self, string eventName, Action action){
        EventManager.Instance.StopListening(eventName, action);
    }
}