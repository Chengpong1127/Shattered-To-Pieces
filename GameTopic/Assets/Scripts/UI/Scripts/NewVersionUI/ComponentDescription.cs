using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComponentDescription : MonoBehaviour
{
    [SerializeField] Image displayImage;
    [SerializeField] TMP_Text descriptionText;

    Vector2 maxSpriteSize = new Vector2(400f, 256f);

    private void Awake() {
        // maxSpriteSize = displayImage.rectTransform.sizeDelta;
    }

    private void Start() {
        gameObject.SetActive(false);
    }

    public void SetDisplay(Sprite sprite, string description) {
        if (sprite == null) {
            displayImage.color = Color.clear;
            descriptionText.text = "";
            gameObject.SetActive(false);
            return;
        }

        // resize Image for new sprite.
        Vector2 newSpriteSize = sprite.rect.size / displayImage.pixelsPerUnit;
        float sizeScale = maxSpriteSize.x / newSpriteSize.x;
        if (maxSpriteSize.y < newSpriteSize.y * sizeScale) {
            sizeScale = maxSpriteSize.y / newSpriteSize.y;
        }
        displayImage.rectTransform.sizeDelta = newSpriteSize * sizeScale;

        displayImage.color = Color.white;
        displayImage.sprite = sprite;
        descriptionText.text = description;
    }
}
