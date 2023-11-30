using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using System.Threading;
using System.Collections.Generic;
using DG.Tweening;

public class HealthEffectHandler: BaseGameEventHandler
{
    public float Duration = 0.2f;
    public Color RecoveryColor = Color.green;
    public Color DamageColor = Color.red;
    private Dictionary<SpriteRenderer, Color> _originalColors = new Dictionary<SpriteRenderer, Color>();
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer)
        {
            GameEvents.AttributeEvents.OnEntityHealthChanged += OnEntityHealthChanged;
        }
    }

    public override void OnDestroy() {
        if (IsServer)
        {
            GameEvents.AttributeEvents.OnEntityHealthChanged -= OnEntityHealthChanged;
        }
        base.OnDestroy();
    }
    private void OnEntityHealthChanged(BaseEntity baseEntity, float oldHealth, float newHealth){
        OnEntityHealthChanged_ClientRpc(baseEntity.NetworkObjectId, oldHealth, newHealth);
    }
    [ClientRpc]
    private void OnEntityHealthChanged_ClientRpc(ulong entityID, float oldHealth, float newHealth){
        try{
            BaseEntity baseEntity = null;
            baseEntity = Utils.GetLocalGameObjectByNetworkID(entityID).GetComponent<BaseEntity>();
            if (newHealth > 0){
                if (newHealth > oldHealth){
                    ColorAnimation(baseEntity.NetworkObjectId, RecoveryColor);
                }
                if (newHealth < oldHealth){
                    ColorAnimation(baseEntity.NetworkObjectId, DamageColor);
                }
            }
        }catch{
            return;
        }
    }

    private void ColorAnimation(ulong entityID, Color startColor){
        var entity = Utils.GetLocalGameObjectByNetworkID(entityID).GetComponent<BaseEntity>();
        entity.BodyRenderers.Select(renderer => renderer as SpriteRenderer).
            Where(renderer => renderer != null)
                .ToList().ForEach(renderer => {
                    if (!_originalColors.ContainsKey(renderer)){
                        _originalColors.Add(renderer, renderer.color);
                    }
                    try{
                        renderer.color = startColor;
                        renderer.DOColor(_originalColors[renderer], Duration).SetEase(Ease.OutCubic);
                    }catch{
                        renderer.color = _originalColors[renderer];
                    }
                    
                }
        );
    }
}