using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempSaver : MonoBehaviour
{
    public void Create0(){
        var component = GameComponentFactory.Instance.CreateComponent(0);
        component.transform.position = new Vector3(0, 10, 0);
    }
    public void Create1(){
        var component = GameComponentFactory.Instance.CreateComponent(1);
        component.transform.position = new Vector3(0, 10, 0);
    }
}
