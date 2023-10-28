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
using Unity.Netcode;

/// <summary>
/// Entity is the base class for all entities in the game. An entity has attributes and the init abilities to set up the init state of the entity.
/// </summary>
[RequireComponent(typeof(AttributeSystemComponent)), RequireComponent(typeof(AbilitySystemCharacter))]
public class Entity: BaseEntity{
    //[HideInInspector]
    public float CollisionDamageThreshold = 80;
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
        //BodyColliders.ToList().ForEach(SetColliderCollision);
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
                if (IsOwner){
                    var collision = await collisionTrigger.OnCollisionEnter2DAsync();
                    if (GetImpulse(collision) > CollisionDamageThreshold){
                        var damage = GetImpulse(collision) - CollisionDamageThreshold;
                        SetCollisionDamage_ServerRpc(damage);
                    }
                }else{
                    await UniTask.Yield();
                }
            }
        }catch(OperationCanceledException){
        }
    }
    [ServerRpc]
    private void SetCollisionDamage_ServerRpc(float damage){
        CollisionDamageEffect.gameplayEffect.Modifiers[0].Multiplier = -damage;
        GameEvents.GameEffectManagerEvents.RequestGiveGameEffect.Invoke(this, this, CollisionDamageEffect);
    }
    private float GetImpulse(Collision2D collision){
        return Mathf.Pow(collision.relativeVelocity.magnitude, 2);
    }
}