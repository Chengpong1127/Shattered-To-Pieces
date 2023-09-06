using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : BaseCoreComponent, IBodyControlable
{
    public BaseCoreComponent body { get; private set; }
    protected override void Awake()
    {
        body = this;


        base.Awake();
    }
}
