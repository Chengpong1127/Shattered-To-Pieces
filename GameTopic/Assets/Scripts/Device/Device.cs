using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device: MonoBehaviour, IDevice
{
    public IGameComponentFactory GameComponentFactory;
    public IGameComponent RootGameComponent { set; get; }
    private Tree tree;
    private void Start() {
        tree = new Tree(RootGameComponent);
    }


    public void LoadDevice(DeviceInfo info){
        
    }

    public IInfo Dump()
    {
        return tree.Dump();
    }
}
