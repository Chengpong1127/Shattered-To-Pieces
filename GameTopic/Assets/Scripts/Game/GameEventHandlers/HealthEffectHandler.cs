using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Unity.Netcode;
using System.Threading;
using System.Collections.Generic;

public class HealthEffectHandler: BaseGameEventHandler
{
    public float Duration = 0.2f;
    public Color RecoveryColor = Color.green;
    public Color DamageColor = Color.red;

    void OnEnable()
    {
        GameEvents.AttributeEvents.OnEntityHealthChanged += OnEntityHealthChanged;
    }
    void OnDisable()
    {
        GameEvents.AttributeEvents.OnEntityHealthChanged -= OnEntityHealthChanged;
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
            Where(renderer => renderer != null).
            ToList().ForEach(async renderer => {
                renderer.color = startColor;
                float elapsedTime = 0f;
                Color endColor = Color.white;
                while (elapsedTime < Duration)
                {
                    renderer.color = Color.Lerp(startColor, endColor, elapsedTime / Duration);
                    elapsedTime += Time.deltaTime;
                    await UniTask.Yield();
                }
                renderer.color = endColor;
            });
    }
}