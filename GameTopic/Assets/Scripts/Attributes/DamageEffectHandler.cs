using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DamageEffectHandler: MonoBehaviour, IGameEventHandler
{
    public float Duration = 0.5f;
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
        if (newHealth < oldHealth && newHealth > 0){
            var renderers = baseEntity.BodyRenderers;
            DamageAnimation(renderers);
        }
    }

    private void DamageAnimation(Renderer[] renderers){
        renderers.Select(renderer => renderer as SpriteRenderer).
            Where(renderer => renderer != null).
            ToList().ForEach(async renderer => {
                renderer.color = new Color(1, 0.5f, 0.5f);
                float elapsedTime = 0f;
                Color startColor = DamageColor;
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