using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class MapTestGameRunner : SimpleGameRunner
{
    public Transform[] ComponentSpawnPoint;
    public GameObject[] Components;
    protected void Gaming_Enter()
    {
        SetRandomComponents();
    }
    private void SetRandomComponents()
    {
        foreach(Transform t in ComponentSpawnPoint)
        {
            int randomIndex = Random.Range(0, Components.Length);
            GameObject randomComponentPrefab = Components[randomIndex];
            var obj=Instantiate(randomComponentPrefab, t.position, Quaternion.identity);
            obj.GetComponent<NetworkObject>()?.Spawn();
        }
    }
}
