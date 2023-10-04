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
    private Dictionary<BaseEntity, CancellationTokenSource> EntityToken = new();

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
        BaseEntity baseEntity = null;
        try{
            baseEntity = Utils.GetLocalGameObjectByNetworkID(entityID).GetComponent<BaseEntity>();
        }catch{
            return;
        }
        if (newHealth > 0){
            if (newHealth > oldHealth){
                if (EntityToken.ContainsKey(baseEntity)){
                    var tokenSource = EntityToken[baseEntity];
                    tokenSource.Cancel();
                    EntityToken.Remove(baseEntity);
                }
                EntityToken.Add(baseEntity, new CancellationTokenSource());
                ColorAnimation(baseEntity.NetworkObjectId, RecoveryColor, EntityToken[baseEntity].Token);
            }
            if (newHealth < oldHealth){
                if (EntityToken.ContainsKey(baseEntity)){
                    var tokenSource = EntityToken[baseEntity];
                    tokenSource.Cancel();
                    EntityToken.Remove(baseEntity);
                }
                EntityToken.Add(baseEntity, new CancellationTokenSource());
                ColorAnimation(baseEntity.NetworkObjectId, DamageColor, EntityToken[baseEntity].Token);
            }
        }
    }

    private void ColorAnimation(ulong entityID, Color startColor, CancellationToken token){
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
                    await UniTask.Yield(token);
                }
                renderer.color = endColor;
            });
    }
}