using System;
using UnityEngine;  


public class Ability{
    /// <summary>
    /// The name of the ability.
    /// </summary>
    public string AbilityName;
    /// <summary>
    /// The action that will be executed when the ability is triggered.
    /// </summary>
    public Action action;
    /// <summary>
    /// The game component that own this ability.
    /// </summary>
    /// <value></value>
    public IGameComponent OwnerGameComponent;

    public Ability(string name){
        this.AbilityName = name;
        this.action = () => {};
    }

    public Ability(string name, Action action, IGameComponent ownerGameComponent){
        this.AbilityName = name;
        this.action = action;
        this.OwnerGameComponent = ownerGameComponent;
    }
    public Ability(string name, Action action){
        this.AbilityName = name;
        this.action = action;
    }
    public void Run(){
        action();
    }
}
