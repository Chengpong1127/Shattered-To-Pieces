using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using UnityEditor.UI;

public class BaseCoreComponent : AbilityEntity, ICoreComponent
{

    /// <summary>
    /// The game component that owns this core component. This will be automatically set when the game component is created.
    /// </summary>
    public IGameComponent OwnerGameComponent { get; set; }
    /// <summary>
    /// Get the controllable transform of the body of the game component.
    /// </summary>
    public override Transform BodyTransform => OwnerGameComponent.BodyTransform;
    /// <summary>
    /// Get the controllable rigidbody of the game component.
    /// </summary>
    public override Rigidbody2D BodyRigidbody => OwnerGameComponent.BodyRigidbody;
    /// <summary>
    /// Get the controllable collider of the game component.
    /// </summary>
    public override Collider2D BodyCollider => OwnerGameComponent.BodyCollider;
    /// <summary>
    /// Get the camera of the game component.
    /// </summary>
    public Camera PlayerCamera => Camera.main;

    /// <summary>
    /// Get the root core component of the game component in the device.
    /// </summary>
    /// <returns></returns>
    public BaseCoreComponent Root => (OwnerGameComponent.GetRoot() as GameComponent).CoreComponent as BaseCoreComponent;
    /// <summary>
    /// Get the parent core component of the game component in the device.
    /// </summary>
    /// <returns></returns>
    public BaseCoreComponent Parent => (OwnerGameComponent.Parent as GameComponent)?.CoreComponent as BaseCoreComponent;
    /// <summary>
    /// Get the children core components of the game component in the device.
    /// </summary>
    /// <returns></returns>
    public BaseCoreComponent[] GetAllChildren(){
        var children = new List<BaseCoreComponent>();
        var tree = new Tree(OwnerGameComponent);
        tree.TraverseBFS((node) => {
            if(node is GameComponent gameComponent){
                children.Add(gameComponent.CoreComponent as BaseCoreComponent);
            }
        });
        return children.ToArray();
    }

    public GameComponentAbility[] GameComponentAbilities {
        get{
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
    public ICoreComponent[] GetOverlapCircleCoreComponentsAll(float radius, Vector2 fromPosition){
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
    public ICoreComponent[] GetOverlapCircleCoreComponentsAll(float radius){
        return GetOverlapCircleCoreComponentsAll(radius, BodyTransform.position);
    }

    protected override void Start() {
        Debug.Assert(OwnerGameComponent != null, "OwnerGameComponent is null");
    }

    public override void Repel(Vector2 force){
        if (this == Root){
            base.Repel(force);
        }else{
            Root.Repel(force);
        }
    } 
}