using System;
using System.Collections;
using System.Collections.Generic;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTagNamespace.Authoring;
using UnityEngine;
using Unity.Netcode;
public class Rubber :MonoBehaviour
{
    [SerializeField]
    public GameplayEffectScriptableObject SlowDown;
    [SerializeField]
    public GameplayEffectScriptableObject UpSpeed;
    [SerializeField]
    public AttributeScriptableObject MovingVelocity;
    private bool isTriggered = false;

    public BaseCoreComponent body { get; private set; }

    public void Awake()
    {
        StartCoroutine(Stay());
    }
    public IEnumerator Stay()
    {
        yield return new WaitForSeconds(3);
        if (!isTriggered)
        {
            Destroy(this.gameObject);
            this.transform.parent.GetComponent<NetworkObject>().Despawn();
        }
        yield return null;
    }
    public IEnumerator AddDeBuffToObject(BaseCoreComponent entity)
    {
        entity.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(entity.AbilitySystemCharacter.MakeOutgoingSpec(SlowDown));
        //GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(this, entity, SlowDown);
        this.GetComponent<SpriteRenderer>().enabled=false;
        yield return new WaitForSeconds(3);
        //GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(this, entity, UpSpeed);
        entity.AbilitySystemCharacter.ApplyGameplayEffectSpecToSelf(entity.AbilitySystemCharacter.MakeOutgoingSpec(UpSpeed));
        Debug.Log(1);
        Destroy(this.gameObject);
        this.transform.parent.GetComponent<NetworkObject>().Despawn();
        yield return null;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.gameObject.GetComponent<Entity>() as BaseCoreComponent;
        if (entity != null&&!isTriggered&&entity.AttributeSystemComponent.GetAttributeValue(MovingVelocity,out var s))
        {
            isTriggered = true;
            StartCoroutine(AddDeBuffToObject(entity));
        }
    }
}
