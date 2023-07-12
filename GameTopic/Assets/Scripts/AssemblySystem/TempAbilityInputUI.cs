using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DragAndDrop;

public class TempAbilityInputUI : ObjectContainerList<Ability>
{
    public AbilityInputManager abilityInputManager;
    private void Start() {
        List<Ability> abilities = new List<Ability>();
        for(int i = 0; i < 10; i++){
            abilities.AddRange(abilityInputManager.AbilityInputEntries[i].Abilities);
        }
        CreateSlots(abilities);
    }
    public override bool CanDrop(Draggable dragged, Slot slot)
    {
        return true;
    }

}
