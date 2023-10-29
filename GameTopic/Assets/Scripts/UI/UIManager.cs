using System.Linq;
using UnityEngine;

public class UIManager : MonoBehaviour {
    public Canvas GlobalCanvas;
    void Awake()
    {
        if (GlobalCanvas == null){
            Debug.LogError("No Global Canvas Found");
        }
    }
}