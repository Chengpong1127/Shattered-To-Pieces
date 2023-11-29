using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Unity.Netcode;
using DG.Tweening.Core;
using DG.Tweening;
public class GameComponentSelectedHandler : BaseGameEventHandler
{
    private Dictionary<SpriteRenderer, Tween> _colorTweenMap = new();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsClient)
        {
            GameEvents.GameComponentEvents.OnGameComponentSelected += OnGameComponentSelected;
        }
    }

    public void OnGameComponentSelected(GameComponent gameComponent, bool selected)
    {
        OnGameComponentSelected_ClientRpc(gameComponent.NetworkObjectId, selected);
    }
    [ClientRpc]
    private void OnGameComponentSelected_ClientRpc(ulong gameComponentID, bool selected){
        var gameComponent = Utils.GetLocalGameObjectByNetworkID(gameComponentID).GetComponent<GameComponent>();
        switch (selected)
        {
            case true:
                gameComponent.BodyRenderers.ToList().ForEach(renderer => {
                    if (renderer is SpriteRenderer spriteRenderer){
                        if (!_colorTweenMap.ContainsKey(spriteRenderer)) {
                            _colorTweenMap[spriteRenderer] = (renderer as SpriteRenderer)
                                .DOFade(0.5f, 0.3f)
                                .SetEase(Ease.InOutSine)
                                .SetLoops(-1, LoopType.Yoyo);
                        }
                    }
                });
                break;
            case false:
                gameComponent.BodyRenderers.ToList().ForEach(renderer => {
                    if (renderer is SpriteRenderer spriteRenderer){
                        if (_colorTweenMap.ContainsKey(spriteRenderer)) {
                            _colorTweenMap[spriteRenderer].SmoothRewind();
                            _colorTweenMap.Remove(spriteRenderer);
                        }
                    }
                });
                break;
        }
    }
    public override void OnDestroy() {
        if (IsClient)
        {
            GameEvents.GameComponentEvents.OnGameComponentSelected -= OnGameComponentSelected;
        }
        base.OnDestroy();
    }



}