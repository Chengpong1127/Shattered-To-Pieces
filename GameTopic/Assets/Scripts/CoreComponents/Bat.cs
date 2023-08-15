using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : BaseCoreComponent, IRotatable
{
    [SerializeField]
    private Transform Handle;
    public Transform RotateBody => BodyTransform;
    public Transform RotateCenter => Handle;
}
