using System;


public static class EventTriggerExtension{
    public static void TriggerEvent(this object _, string eventName, params object[] args) {
        EventManager.Instance.TriggerEvent(eventName, args);
    }

    public static void StartListening(this object _, string eventName, Delegate action){
        EventManager.Instance.AddListener(eventName, action);
    }
    public static void StartListening(this object _, string eventName, Action action){
        EventManager.Instance.AddListener(eventName, action);
    }
    public static void StartListening<T>(this object _, string eventName, Action<T> action){
        EventManager.Instance.AddListener(eventName, action);
    }
    public static void StartListening<T1, T2>(this object _, string eventName, Action<T1, T2> action){
        EventManager.Instance.AddListener(eventName, action);
    }
    public static void StartListening<T1, T2, T3>(this object _, string eventName, Action<T1, T2, T3> action){
        EventManager.Instance.AddListener(eventName, action);
    }
    public static void StartListening<T1, T2, T3, T4>(this object _, string eventName, Action<T1, T2, T3, T4> action){
        EventManager.Instance.AddListener(eventName, action);
    }

    public static void StopListening(this object _, string eventName, Delegate action){
        EventManager.Instance.RemoveListener(eventName, action);
    }
    public static void StopListening(this object _, string eventName, Action action){
        EventManager.Instance.RemoveListener(eventName, action);
    }
    public static void StopListening<T>(this object _, string eventName, Action<T> action){
        EventManager.Instance.RemoveListener(eventName, action);
    }
    public static void StopListening<T1, T2>(this object _, string eventName, Action<T1, T2> action){
        EventManager.Instance.RemoveListener(eventName, action);
    }
    public static void StopListening<T1, T2, T3>(this object _, string eventName, Action<T1, T2, T3> action){
        EventManager.Instance.RemoveListener(eventName, action);
    }
    public static void StopListening<T1, T2, T3, T4>(this object _, string eventName, Action<T1, T2, T3, T4> action){
        EventManager.Instance.RemoveListener(eventName, action);
    }
}