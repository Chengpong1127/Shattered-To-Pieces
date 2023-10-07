using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] Image displayImage;
    [SerializeField] PriceCtrl priceCtrl;

    Vector2 maxSpriteSize;

    public int SellID = -1;
    public ISellElementSubmitable EventSubmitter;

    private void Awake() {
        maxSpriteSize = displayImage.rectTransform.sizeDelta;
        SetDisplay(null, 0); // Clear Display.
    }

    public void OnPointerClick(PointerEventData eventData) {
        EventSubmitter?.Buy?.Invoke(SellID);
    }
    public void OnPointerEnter(PointerEventData eventData) {
        EventSubmitter?.OpenDescription?.Invoke(SellID);
    }
    public void OnPointerExit(PointerEventData eventData) {
        EventSubmitter?.CloseDescription?.Invoke(SellID);
    }

    public void SetDisplay(Sprite sprite, int price) {
        if(sprite == null) {
            displayImage.color = Color.clear;
            priceCtrl.gameObject.SetActive(false);
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
        priceCtrl.gameObject.SetActive(true);
        priceCtrl.SetPrice(price);
    }
}
