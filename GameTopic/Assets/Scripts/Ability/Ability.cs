using System;
using UnityEngine;  


public class Ability{
    public string AbilityName;
    public Action action;
    public Ability(string name, Action action){
        this.AbilityName = name;
        this.action = action;
    }
    public void Run(){
        action();
    }
    public static Ability EmptyAbility(){
        return new Ability("Empty", () => {});
    }
}
