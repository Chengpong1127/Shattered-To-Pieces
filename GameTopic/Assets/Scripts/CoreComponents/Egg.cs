using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : BaseCoreComponent, ICreated {
    public BaseCoreComponent Owner { get; set; } = null;

    [SerializeField] public GameplayEffectScriptableObject DamageEffect;

    bool hit = false;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (hit) { return; }
        if (Owner == null) { hit = true; }

        var hitComponent = collision.gameObject.GetComponentInChildren<BaseCoreComponent>();
        if (hitComponent == null) { Destroy(gameObject.transform.root.gameObject); return; }
        if (Owner.HasTheSameRootWith(hitComponent)) { return; }


        hit = true;
        var entity = hitComponent as Entity;
        GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(Owner, entity, DamageEffect);
        Destroy(gameObject.transform.root.gameObject);
    }
}
