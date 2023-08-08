using System.Collections.Generic;
using UnityEngine;
public abstract class BaseCoreComponent : MonoBehaviour, ICoreComponent
{
    /// <summary>
    /// All abilities of this game component.
    /// </summary>
    /// <typeparam name="string"> The name of the ability. </typeparam>
    /// <typeparam name="Ability"> The ability. </typeparam>
    /// <returns> All abilities of this game component. </returns>
    public Dictionary<string, Ability> AllAbilities { get; set; } = new Dictionary<string, Ability>();
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
    /// <summary>
    /// Determine whether the other game component has the same root game component as this game component.
    /// </summary>
    /// <param name="other"> The other game component. </param>
    /// <returns> True if the other game component has the same root game component as this game component. </returns>
    public bool HasTheSameRootWith(ICoreComponent other){
        return OwnerGameComponent.GetRoot() == other.OwnerGameComponent.GetRoot();
    }
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

    protected virtual void Start() {
        Debug.Assert(OwnerGameComponent != null, "OwnerGameComponent is null");
    }
}