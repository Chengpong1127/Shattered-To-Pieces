using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class MapTestGameRunner : SimpleGameRunner
{
    public Transform[] ComponentSpawnPoint;
    public GameObject[] Components;
    private void Gaming_Enter()
    {
        SetPlayerSpawnPoints();
        SetRandomComponents();
    }
    private void SetPlayerSpawnPoints()
    {
        int i = 0;
        foreach (var player in PlayerMap)
        {
            player.Value.SetPlayerPoint(SpawnPoints[i++]);
        }
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
