using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AbilitySelectHandler: BaseGameEventHandler{
    private Dictionary<GameComponent, Vector3> _originalScaleMap = new();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsClient)
        {
            GameEvents.UIEvents.OnGameComponentAbilitySelected += GameComponentAbilitySelectedHandler;
            GameEvents.UIEvents.OnGameComponentAbilitySelectedEnd += GameComponentAbilitySelectedEndHandler;
        }
    }

    private void GameComponentAbilitySelectedHandler(GameComponent gameComponent) {
        _originalScaleMap[gameComponent] = gameComponent.BodyTransform.localScale;
        RunScaleAnimation(gameComponent.BodyTransform, 1.2f);
    }

    private void GameComponentAbilitySelectedEndHandler(GameComponent gameComponent) {
        StopScaleAnimation(gameComponent.BodyTransform);
        gameComponent.BodyTransform.localScale = _originalScaleMap[gameComponent];
        _originalScaleMap.Remove(gameComponent);
    }

    private void RunScaleAnimation(Transform targetTransform, float targetScale) {
        targetTransform
            .DOScale(targetTransform.localScale * targetScale, 0.5f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopScaleAnimation(Transform targetTransform) {
        targetTransform.DOKill();
    }


}