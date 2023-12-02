using System.Collections.Generic;
using UnityEngine;
using AbilitySystem.Authoring;
using Unity.Netcode;
using System;
using System.Linq;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class BaseCoreComponent : GameComponent, ICoreComponent
{
    public BaseCoreComponent Root => GetRoot() as BaseCoreComponent;

    /// <summary>
    /// Get the children core components of the game component in the device.
    /// </summary>
    /// <returns></returns>
    /// 
    public BaseCoreComponent[] GetAllChildren(){
        var children = new List<BaseCoreComponent>();
        var tree = new Tree(this);
        tree.TraverseBFS((node) => {
            if(node is GameComponent gameComponent){
                children.Add(gameComponent as BaseCoreComponent);
            }
        });
        return children.ToArray();
    }
    public List<BaseCoreComponent> GetAllChildrenList()
    {
        var children = new List<BaseCoreComponent>();
        var tree = new Tree(this);
        tree.TraverseBFS((node) => {
            if (node is GameComponent gameComponent)
            {
                children.Add(gameComponent as BaseCoreComponent);
            }
        });
        return children;
    }

    public GameComponentAbility[] GameComponentAbilities {
        get{
            var gameComponentAbilities = new GameComponentAbility[Abilities.Length];
            var abilitySpecs = GetAbilitySpecs();
            for (int i = 0; i < Abilities.Length; i++)
            {
                gameComponentAbilities[i] = new GameComponentAbility(i, this, Abilities[i], abilitySpecs[i] as RunnerAbilitySpec);
            }
            return gameComponentAbilities;
        }
    }

    /// <summary>
    /// Determine whether the other game component has the same root game component as this game component.
    /// </summary>
    /// <param name="other"> The other game component. </param>
    /// <returns> True if the other game component has the same root game component as this game component. </returns>
    public bool HasTheSameRootWith(BaseCoreComponent other){
        return GetRoot() == other.GetRoot();
    }

    public override void Repel(Vector2 force){
        if (Equals(GetRoot())){
            base.Repel(force);
        }else{
            (GetRoot() as BaseCoreComponent).Repel(force);
        }
    }

    [ClientRpc]
    public void SetAlpha_ClientRpc(float alpha, float time, ClientRpcParams clientRpcParams = default)
    {
        BodyRenderers.ToList().ForEach(async renderer => {
            if (renderer is SpriteRenderer spriteRenderer)
            {
                await spriteRenderer.DOFade(alpha, time).ToUniTask();
            }
        });
    }
}