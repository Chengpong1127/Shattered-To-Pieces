using AbilitySystem.Authoring;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Egg : BaseEntity, ICreated {
    public BaseCoreComponent Owner { get; set; } = null;

    [SerializeField] public GameplayEffectScriptableObject DamageEffect;
    [SerializeField] GameObject ExplosionObj;

    bool hit = false;

    private void OnCollisionEnter2D(Collision2D collision) {
        if (IsServer){
            if (hit) { return; }
            if (Owner == null) { hit = true; }

            var hitComponent = collision.gameObject.GetComponentInChildren<BaseCoreComponent>();
            if (hitComponent == null) { StartCoroutine(ExplosionAnimation()); return; }
            if (Owner.HasTheSameRootWith(hitComponent)) { return; }


            hit = true;
            var entity = hitComponent as Entity;
            GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(Owner, entity, DamageEffect);
            StartCoroutine(ExplosionAnimation());
        }
        
    }

    IEnumerator ExplosionAnimation() {
        foreach (var collider in BodyColliders) { collider.enabled = false; }
        foreach (var renderer in BodyRenderers) { renderer.enabled = false; }
        BodyRigidbody.velocity = Vector3.zero;
        BodyRigidbody.isKinematic = true;

        ExplosionObj.SetActive(true);
        ExplosionObj.GetComponent<Animator>().enabled = true;

        yield return new WaitForSeconds(.6f);

        Destroy(gameObject.transform.root.gameObject);
        this.NetworkObject.Despawn();

        yield return null;
    }
}
