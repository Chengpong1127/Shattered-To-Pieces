using UnityEngine;
using UnityEditor;
using System.Linq;
using System;


public class CustomMenuItems
{
    [MenuItem("TestFunction/Enter Map Test Host")]
    private static void EnterMapTestHost(){
        LocalGameManager.Instance.EnterRoom("MapTest", NetworkType.Host);
    }

    [MenuItem("TestFunction/Enter Map Test Client")]
    private static void EnterMapTestClient(){
        LocalGameManager.Instance.EnterRoom("MapTest", NetworkType.Client);
    }



}

