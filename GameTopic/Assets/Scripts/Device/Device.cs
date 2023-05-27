using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Device
{
    public Dictionary<int, IGameComponent> ComponentMap;
    public List<int> ComponentIDList;
    public Dictionary<int, List<int>> ConnecterMap;


    public Device()
    {
        ComponentMap = new Dictionary<int, IGameComponent>();
        ComponentIDList = new List<int>();
    }


}
