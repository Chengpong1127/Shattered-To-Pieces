using Unity.Netcode;
using UnityEngine;


public class SingletonNetworkBehavior<T>: NetworkBehaviour where T : NetworkBehaviour {
    private static T _instance;
    public static T Instance {
        get {
            _instance ??= FindObjectOfType<T>();
            if (_instance == null) {
                Debug.LogError("No instance of " + typeof(T).Name + " found");
            }
            return _instance;
        }
    }
}