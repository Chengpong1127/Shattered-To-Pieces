using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SellElement : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] Image reycastedImage;
    [SerializeField] Image displayImage;
    [SerializeField] PriceCtrl priceCtrl;
    [SerializeField] CanvasGroup canvasGroup;

    Vector2 maxSpriteSize;

    public int SellID = -1;
    public ISellElementSubmitable EventSubmitter;
    private int _price;

    private void Awake() {
        Debug.Assert(reycastedImage != null);
        Debug.Assert(displayImage != null);
        Debug.Assert(priceCtrl != null);
        Debug.Assert(canvasGroup != null);
        maxSpriteSize = displayImage.rectTransform.sizeDelta;
        GameRunner.OnRunGame += RunGameHandler;
    }
    private void RunGameHandler(GameRunner gameRunner) {
        if (gameRunner is AssemblyRoomRunner assemblyRoomRunner) {
            assemblyRoomRunner.OnMoneyChanged += OnMoneyChangedHandler;
        }
    }
    private void OnDestroy() {
        GameRunner.OnRunGame -= RunGameHandler;
        if(GameRunner.ServerGameRunnerInstance is AssemblyRoomRunner assemblyRoomRunner) {
            assemblyRoomRunner.OnMoneyChanged -= OnMoneyChangedHandler;
        }
    }

    private void OnMoneyChangedHandler(int money) {
        if (SellID == -1) { return; }
        if (money < _price) {
            priceCtrl.SetNotEnoughColor();
        } else {
            priceCtrl.SetNormalColor();
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (GameRunner.ServerGameRunnerInstance is AssemblyRoomRunner assemblyRoomRunner) {
            if (assemblyRoomRunner.GetPlayerRemainedMoney() < _price) {
                return;
            }
        }
        EventSubmitter?.Buy?.Invoke(SellID);
    }
    public void OnPointerEnter(PointerEventData eventData) {
        EventSubmitter?.OpenDescription?.Invoke(SellID);
    }
    public void OnPointerExit(PointerEventData eventData) {
        EventSubmitter?.CloseDescription?.Invoke(SellID);
    }

    public void SetDisplay(Sprite sprite, int price) {
        _price = price;
        canvasGroup.alpha = 1f;
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

        if (reycastedImage != null) {
            reycastedImage.raycastTarget = true;
        }
        if (GameRunner.ServerGameRunnerInstance is AssemblyRoomRunner assemblyRoomRunner) {
            OnMoneyChangedHandler(assemblyRoomRunner.GetPlayerRemainedMoney());
        }
    }
    public void SetEmpty() {
        canvasGroup.alpha = 0.5f;
        displayImage.color = Color.clear;
        priceCtrl.gameObject.SetActive(false);
        if(reycastedImage != null) {
            reycastedImage.raycastTarget = false;
        }
    }
}
