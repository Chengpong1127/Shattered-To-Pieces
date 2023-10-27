using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class CooperationMapRunner : MapTestGameRunner
{
    public GameObject Leg;
    public Transform[] SetLegSpawnPoint;
    private void Start()
    {
        GameEvents.GameComponentEvents.OnGameComponentSelected += HandleGameComponentConnected;
        foreach(Transform t in SetLegSpawnPoint)
        {
            var obj = Instantiate(Leg, t);
            spawnedObjects.Add(obj);
            obj?.GetComponent<NetworkObject>().Spawn();
        }
        StartCoroutine(RepeatSetRandomComponents());
    }

}
