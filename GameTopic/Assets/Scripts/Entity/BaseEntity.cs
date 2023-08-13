using UnityEngine;


public abstract class BaseEntity : MonoBehaviour, IUnit
{
    public int UnitID { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
}