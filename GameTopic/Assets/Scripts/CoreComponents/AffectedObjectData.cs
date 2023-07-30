using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AffectedObjectData", menuName = "AffectedObject/AffectedObjectData")]
public class AffectedObjectData : ScriptableObject
{
    public float health { get; set; } = 50.0f;
    public float defence { get; set; } = 0.0f;
    public bool IsHealthAffected { get; set; } = true;
    public bool IsDefenceAffected { get; set; } = true;
    public bool IsBreak { get; set; } = false;
}
