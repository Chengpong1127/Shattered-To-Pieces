using System.Collections.Generic;
using UnityEngine;
public abstract class BaseCoreComponent : MonoBehaviour, ICoreComponent
{
    public Dictionary<string, Ability> AllAbilities { get; set; } = new Dictionary<string, Ability>();
    public IGameComponent OwnerGameComponent { get; set; }
    public bool HasTheSameRootWith(ICoreComponent other){
        return OwnerGameComponent.GetRoot() == other.OwnerGameComponent.GetRoot();
    }
    protected void Start() {
        Debug.Assert(OwnerGameComponent != null, "OwnerGameComponent is null");
    }
}