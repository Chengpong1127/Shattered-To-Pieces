using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using System.Linq;
using DG.Tweening.Core;
public class AbilitySelectHandler: BaseGameEventHandler{
    private Dictionary<SpriteRenderer, TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions>> _colorTweenMap = new();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsClient)
        {
            GameEvents.UIEvents.OnGameComponentAbilitySelected += GameComponentAbilitySelectedHandler;
            GameEvents.UIEvents.OnGameComponentAbilitySelectedEnd += GameComponentAbilitySelectedEndHandler;
        }
    }
    public override void OnDestroy() {
        if (IsClient)
        {
            GameEvents.UIEvents.OnGameComponentAbilitySelected -= GameComponentAbilitySelectedHandler;
            GameEvents.UIEvents.OnGameComponentAbilitySelectedEnd -= GameComponentAbilitySelectedEndHandler;
        }
        base.OnDestroy();
    }

    private void GameComponentAbilitySelectedHandler(GameComponent gameComponent) {
        StartOutlineAnimation(gameComponent);
    }

    private void GameComponentAbilitySelectedEndHandler(GameComponent gameComponent) {
        StopOutlineAnimation(gameComponent);
    }

    private void StartColorAnimation(GameComponent gameComponent){
        gameComponent.BodyRenderers.ToList().ForEach(renderer => {
            if (renderer is SpriteRenderer spriteRenderer){
                if (!_colorTweenMap.ContainsKey(spriteRenderer)) {
                    _colorTweenMap[spriteRenderer] = (renderer as SpriteRenderer).DOColor(Color.yellow, 0.5f).SetLoops(-1, LoopType.Yoyo);
                }
            }

        });
    }

    private void StopColorAnimation(GameComponent gameComponent){
        gameComponent.BodyRenderers.ToList().ForEach(renderer => {
            if (renderer is SpriteRenderer spriteRenderer){
                if (_colorTweenMap.ContainsKey(spriteRenderer)) {
                    _colorTweenMap[spriteRenderer].SmoothRewind();
                    _colorTweenMap.Remove(spriteRenderer);
                }
            }

        });
    }

    private void StartOutlineAnimation(GameComponent gameComponent){
        var outlineController = gameComponent.GetComponent<OutlineController>();
        if (outlineController != null){
            outlineController.SetOutline(true);
        }
    }

    private void StopOutlineAnimation(GameComponent gameComponent){
        var outlineController = gameComponent.GetComponent<OutlineController>();
        if (outlineController != null){
            outlineController.SetOutline(false);
        }
    }

}