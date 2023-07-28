using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : MonoBehaviour, ICoreComponent
{
    public Dictionary<string, Ability> AllAbilities { get; private set; } = new Dictionary<string, Ability>();

    private void Start() {
        AllAbilities.Add("Swing", new Ability("Swing", Swing, this));
    }

    private void Swing(){
        Debug.Log("Swing");
    }
}
