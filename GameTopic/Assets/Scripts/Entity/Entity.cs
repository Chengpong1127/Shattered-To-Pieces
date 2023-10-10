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
using AttributeSystem.Authoring;

/// <summary>
/// Entity is the base class for all entities in the game. An entity has attributes and the init abilities to set up the init state of the entity.
/// </summary>
[RequireComponent(typeof(AttributeSystemComponent)), RequireComponent(typeof(AbilitySystemCharacter))]
public class Entity: BaseEntity{
    //[HideInInspector]
    public float CollisionDamageThreshold = 400;
    [HideInInspector]
    public GameplayEffectScriptableObject CollisionDamageEffect;
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
        CollisionDamageEffect = ResourceManager.Instance.LoadGameplayEffect("CollisionDamageGE");
    }
    protected override void Start()
    {
        base.Start();
        BodyColliders.ToList().ForEach(SetColliderCollision);
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
        try{
            while(true){
                var collision = await collisionTrigger.OnCollisionEnter2DAsync();
                if (GetImpulse(collision) > CollisionDamageThreshold){
                    var damage = GetImpulse(collision) - CollisionDamageThreshold;
                    damage = Mathf.Pow(damage, 1.5f) / 10;
                    Debug.Log($"Collision damage: {damage}");
                    CollisionDamageEffect.gameplayEffect.Modifiers[0].Multiplier = -damage;
                    GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(this, this, CollisionDamageEffect);
                }

            }
        }catch(OperationCanceledException){
        }
    }
    private float GetImpulse(Collision2D collision){
        return 0.5f * BodyRigidbody.mass * Mathf.Pow(collision.relativeVelocity.magnitude, 2);
    }
}