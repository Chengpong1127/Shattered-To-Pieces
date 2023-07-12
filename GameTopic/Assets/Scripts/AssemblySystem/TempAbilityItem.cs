using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragAndDrop;
using TMPro;

public class TempAbilityItem : Draggable
{
    public TMP_Text text;
    public override void UpdateObject()
    {
        Ability ability = obj as Ability;
        if(ability != null){
            text.text = ability.name;
        }
        else{
            text.text = "";
        }
    }
}
