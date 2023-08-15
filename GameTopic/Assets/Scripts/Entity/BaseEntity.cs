using UnityEngine;


public abstract class BaseEntity : MonoBehaviour, IUnit
{
    public int UnitID { get; set; }
}