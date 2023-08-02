using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AffectedObjectData", menuName = "AffectedObject/AffectedObjectData")]
public class AffectedObjectData : ScriptableObject
{
    public float health { get; set; } = 50.0f;
    public float defense { get; set; } = 0.0f;
    public bool IsHealthBuffAffected { get; set; } = true;
    public bool IsHealthDeBuffAffected { get; set; } = true;
    public bool IsDefenceBuffAffected { get; set; } = true;
    public bool IsDefenceDeBuffAffected { get; set; } = true;
    public bool IsBreak { get; set; } = false;
}
