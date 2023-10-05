using UnityEngine;


public abstract class SingletonMonoBehavior<T>: MonoBehaviour where T : MonoBehaviour {
    private static T _instance;
    public static T Instance {
        get {
            _instance ??= FindObjectOfType<T>();
            if (_instance == null) {
                GameObject gameObject = new(typeof(T).Name);
                _instance = gameObject.AddComponent<T>();
            }
            return _instance;
        }
    }
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
}