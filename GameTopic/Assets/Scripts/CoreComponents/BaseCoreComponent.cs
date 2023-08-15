using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
public class BaseCoreComponent : AbilityEntity, ICoreComponent
{

    /// <summary>
    /// The game component that owns this core component. This will be automatically set when the game component is created.
    /// </summary>
    public IGameComponent OwnerGameComponent { get; set; }
    /// <summary>
    /// Get the controllable transform of the body of the game component.
    /// </summary>
    public Transform BodyTransform => OwnerGameComponent.BodyTransform;
    /// <summary>
    /// Get the controllable rigidbody of the game component.
    /// </summary>
    public Rigidbody2D BodyRigidbody => OwnerGameComponent.BodyRigidbody;
    /// <summary>
    /// Get the controllable collider of the game component.
    /// </summary>
    public Collider2D BodyCollider => OwnerGameComponent.BodyCollider;

    public GameComponentAbility[] GameComponentAbilities {
        get{
            Debug.Log("Get GameComponentAbilities, Ability count: " + Abilities.Length);
            var gameComponentAbilities = new GameComponentAbility[Abilities.Length];
            var abilitySpecs = GetAbilitySpecs();
            for (int i = 0; i < Abilities.Length; i++)
            {
                gameComponentAbilities[i] = new GameComponentAbility(i, this, Abilities[i], abilitySpecs[i]);
            }
            return gameComponentAbilities;
        }
    }

    /// <summary>
    /// Determine whether the other game component has the same root game component as this game component.
    /// </summary>
    /// <param name="other"> The other game component. </param>
    /// <returns> True if the other game component has the same root game component as this game component. </returns>
    public bool HasTheSameRootWith(ICoreComponent other){
        return OwnerGameComponent.GetRoot() == other.OwnerGameComponent.GetRoot();
    }
    /// <summary>
    /// Get all of the core components that are overlapped with a circle.
    /// </summary>
    /// <param name="radius"> The radius of the circle. </param>
    /// <param name="fromPosition"> The center of the circle. </param>
    /// <returns> All of the core components that are overlapped with a circle. </returns>
    protected ICoreComponent[] GetOverlapCircleCoreComponentsAll(float radius, Vector2 fromPosition){
        var colliders = Physics2D.OverlapCircleAll(fromPosition, radius);
        var coreComponents = new List<ICoreComponent>();
        foreach(var collider in colliders){
            var gameComponent = collider.GetComponentInParent<IGameComponent>();
            var coreComponent = gameComponent?.CoreComponent;
            if(coreComponent is BaseCoreComponent && coreComponent != null && !coreComponent.Equals(this)){
                coreComponents.Add(coreComponent);
            }
        }
        return coreComponents.ToArray();
    }
    /// <summary>
    /// Get all of the core components that are overlapped with the circle of the game component.
    /// </summary>
    /// <param name="radius"> The radius of the circle. </param>
    /// <returns> All of the core components that are overlapped with the circle of the game component. </returns>
    protected ICoreComponent[] GetOverlapCircleCoreComponentsAll(float radius){
        return GetOverlapCircleCoreComponentsAll(radius, BodyTransform.position);
    }

    protected virtual void Start() {
        Debug.Assert(OwnerGameComponent != null, "OwnerGameComponent is null");
    }
}