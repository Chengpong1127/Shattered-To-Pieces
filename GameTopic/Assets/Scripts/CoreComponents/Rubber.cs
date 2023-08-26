using System;
using System.Collections;
using System.Collections.Generic;
using AbilitySystem.Authoring;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTag.Authoring;
using UnityEngine;

public class Rubber :BaseCoreComponent
{
    [SerializeField]
    public GameplayEffectScriptableObject SlowDown;
    [SerializeField]
    public GameplayEffectScriptableObject UpSpeed;
    private bool isTriggered = false;

    public new void Awake()
    {
        StartCoroutine(Stay());
    }
    public IEnumerator Stay()
    {
        yield return new WaitForSeconds(3);
        if(!isTriggered)
        Destroy(this.gameObject);
        yield return null;
    }
    public IEnumerator AddDeBuffToObject(Entity entity)
    {
        this.TriggerEvent(EventName.GameEffectManagerEvents.RequestGiveGameEffect, this as Entity, entity, SlowDown);
        this.GetComponent<SpriteRenderer>().enabled=false;
        yield return new WaitForSeconds(3);
        this.TriggerEvent(EventName.GameEffectManagerEvents.RequestGiveGameEffect, this as Entity, entity, UpSpeed);
        Destroy(this.gameObject);
        yield return null;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        var entity = collision.gameObject.GetComponent<Entity>();
        if (entity != null&&!isTriggered)
        {
            isTriggered = true;
            StartCoroutine(AddDeBuffToObject(entity));
        }
    }
}
