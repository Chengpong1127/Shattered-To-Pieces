using AttributeSystem.Components;
using AbilitySystem;
using UnityEngine;
using AbilitySystem.Authoring;
using System;
using System.Collections;
using System.Linq;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using Cysharp.Threading.Tasks.Triggers;

/// <summary>
/// Entity is the base class for all entities in the game. An entity has attributes and the init abilities to set up the init state of the entity.
/// </summary>
[RequireComponent(typeof(AttributeSystemComponent)), RequireComponent(typeof(AbilitySystemCharacter))]
public class Entity: BaseEntity{
    [HideInInspector]
    public AttributeSystemComponent AttributeSystemComponent;
    [HideInInspector]
    public AbilitySystemCharacter AbilitySystemCharacter;
    public bool IsInitialized { get; private set;}
    [SerializeField]
    protected AbstractAbilityScriptableObject[] InitializationAbilities;
    protected override void Awake() {
        base.Awake();
        AttributeSystemComponent ??= GetComponent<AttributeSystemComponent>();
        AbilitySystemCharacter ??= GetComponent<AbilitySystemCharacter>();
        AbilitySystemCharacter.AttributeSystem = AttributeSystemComponent;
        ActivateInitializationAbilities();
    }
    private async void ActivateInitializationAbilities()
    {
        AbstractAbilitySpec[] specs = new AbstractAbilitySpec[InitializationAbilities.Length];
        for (int i = 0; i < InitializationAbilities.Length; i++)
        {
            if (InitializationAbilities[i] == null) continue;
            specs[i] = InitializationAbilities[i].CreateSpec(AbilitySystemCharacter);
            AbilitySystemCharacter.GrantAbility(specs[i]);
            StartCoroutine(specs[i].TryActivateAbility());
        }
        await UniTask.WaitUntil(() => specs.Any(spec => spec.isActive));
        await UniTask.WaitUntil(() => specs.All(spec => !spec.isActive));
        IsInitialized = true;
    }

    private async void SetColliderCollision(Collider2D collider){
        var collisionTrigger = collider.GetAsyncCollisionEnter2DTrigger();
        while(true){
            var collision = await collisionTrigger.OnCollisionEnter2DAsync();

            ContactPoint2D[] contacts = new ContactPoint2D[collision.contactCount];
            collision.GetContacts(contacts);
            float totalImpulse = 0;
            foreach (ContactPoint2D contact in contacts) {
                totalImpulse += contact.normalImpulse;
            }
            Debug.Log(totalImpulse);
        }
    }
}