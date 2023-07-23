using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DragAndDrop;


public enum AbilityUIType{
    Idle,
    Active
}
public class TempAbilityInputUI : ObjectContainerList<Ability>
{
    public AbilityManager abilityInputManager;
    public AbilityUIType abilityUIType;
    private void Start() {
        if(abilityUIType == AbilityUIType.Active){
            List<Ability> abilities = new List<Ability>();
            for(int i = 0; i < 10; i++){
                abilities.AddRange(abilityInputManager.AbilityInputEntries[i].Abilities);
            }
            CreateSlots(abilities);
        }
    }
    public void UpdateAbility(){
        if(abilityUIType != AbilityUIType.Active){
            return;
        }
        List<Ability> abilities = objects;
        for(int i = 0; i < abilities.Count; i++){
            int entryIndex = i / AbilityInputEntry.AbilityNumber;
            int abilityIndex = i % AbilityInputEntry.AbilityNumber;
            if(abilities[i] != null){
                Debug.Log("Set ability " + abilities[i].AbilityName + " to " + entryIndex + " " + abilityIndex);
            }
            abilityInputManager.SetAbilityToEntry(entryIndex, abilities[i]);
            abilityInputManager.SetPath(entryIndex, entryIndex.ToString());
        }
    }
    public override void Drop(Slot slot, ObjectContainer fromContainer)
    {
        base.Drop(slot, fromContainer);
        UpdateAbility();
    }
    public void SetItems(List<Ability> abilities){
        CreateSlots(abilities);
    }
    public override bool CanDrop(Draggable dragged, Slot slot)
    {
        return true;
    }
}
