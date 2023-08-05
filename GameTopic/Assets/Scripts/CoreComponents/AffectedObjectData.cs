using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AffectedObjectData", menuName = "AffectedObject/AffectedObjectData")]
public class AffectedObjectData : ScriptableObject {

    [field: SerializeField] public float health { get; set; } = 50.0f;
    [field: SerializeField] public float defense { get; set; } = 0.0f;
    [field:SerializeField] public bool IsHealthBuffAffected { get; set; } = true;
    [field:SerializeField] public bool IsHealthDeBuffAffected { get; set; } = true;
    [field:SerializeField] public bool IsDefenceBuffAffected { get; set; } = true;
    [field:SerializeField] public bool IsDefenceDeBuffAffected { get; set; } = true;
    [field: SerializeField] public bool IsBreak { get; set; } = false;
}
