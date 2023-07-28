using System;
using UnityEngine;  


public class Ability{
    /// <summary>
    /// The name of the ability.
    /// </summary>
    public readonly string AbilityName;
    /// <summary>
    /// The action that will be executed when the ability is triggered.
    /// </summary>
    public Action ActionStarted = () => {};
    /// <summary>
    /// The action that will be executed each frame when the ability is running.
    /// </summary>
    public Action ActionRunning = () => {};
    /// <summary>
    /// The action that will be executed when the ability is ended.
    /// </summary>
    public Action ActionEnded = () => {};

    /// <summary>
    /// The game component that own this ability.
    /// </summary>
    /// <value></value>
    public readonly ICoreComponent OwnerGameComponent;

    public Ability(string name){
        this.AbilityName = name;
    }

    public Ability(string name, Action actionStarted, ICoreComponent ownerGameComponent){
        this.AbilityName = name;
        this.ActionStarted = actionStarted;
        this.OwnerGameComponent = ownerGameComponent;
    }

    public Ability(string name, Action actionStarted, Action actionRunning, Action actionEnded, ICoreComponent ownerGameComponent){
        this.AbilityName = name;
        this.ActionStarted = actionStarted;
        this.ActionRunning = actionRunning;
        this.ActionEnded = actionEnded;
        this.OwnerGameComponent = ownerGameComponent;
    }
    public Ability(string name, Action actionStarted){
        this.AbilityName = name;
        this.ActionStarted = actionStarted;
    }
    public void StartAbility(){
        ActionStarted();
    }

    public void RunAbility(){
        ActionRunning();
    }

    public void EndAbility(){
        ActionEnded();
    }

    public override bool Equals(object obj)
    {
        return obj is Ability ability 
            && AbilityName == ability.AbilityName 
            && OwnerGameComponent == ability.OwnerGameComponent;
    }

    public override int GetHashCode()
    {
        if (OwnerGameComponent == null){
            return AbilityName.GetHashCode();
        }else{
            return AbilityName.GetHashCode() ^ OwnerGameComponent.GetHashCode();
        }
        
    }
}
