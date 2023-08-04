using System.Collections.Generic;
using UnityEngine;

public class ControlRoom : MonoBehaviour, ICoreComponent
{
    public Dictionary<string, Ability> AllAbilities => new Dictionary<string, Ability>();
    public IGameComponent OwnerGameComponent { get; set; }
    
}