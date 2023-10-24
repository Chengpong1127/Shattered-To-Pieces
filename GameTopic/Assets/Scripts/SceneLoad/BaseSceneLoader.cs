using UnityEngine;

public abstract class BaseSceneLoader: MonoBehaviour{
    public abstract void LoadScene(AsyncOperation asyncOperation);
    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}