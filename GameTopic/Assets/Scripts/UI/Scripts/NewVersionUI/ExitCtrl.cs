using System;
using UnityEngine;

public class ExitCtrl : MonoBehaviour
{
    [SerializeField] GameObject GraHighlight;

    public Action<float> OnHever { get; set; }
    public Action<bool> OnClick { get; set; }
    private void Start() {
        OnHever += (posY) => {
            if (GraHighlight == null) { return; }
            RectTransform rect = GraHighlight.GetComponent<RectTransform>();
            var pos = rect.position;
            pos.y = posY;
            rect.position = pos;
        };
        OnClick += (b) => {
            if (b) {
                // call exit game function.
                var manager = FindObjectOfType<LocalPlayerManager>();
                if(manager == null) {
                    Debug.LogError("BaseLocalPlayerManager not found, can't exit game.");
                    gameObject.SetActive(false);
                    return;
                } else {
                    manager.ExitGame();
                }
            } else { gameObject.SetActive(false); }
        };
        gameObject.SetActive(false);
    }
}
