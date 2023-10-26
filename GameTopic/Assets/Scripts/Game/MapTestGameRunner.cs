using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class MapTestGameRunner : SimpleGameRunner
{
    public Transform[] ComponentSpawnPoint;
    public GameObject[] Components;
    private float repeatRate = 15.0f; 
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private int[] shuffledIndices;
    private void Start()
    {
        GameEvents.GameComponentEvents.OnGameComponentConnected += HandleGameComponentConnected;
        StartCoroutine(RepeatSetRandomComponents());
    }
    private int[] GetShuffledIndices(int length)
    {
        int[] indices = new int[length];
        for (int i = 0; i < length; i++)
        {
            indices[i] = i;
        }

        for (int i = 0; i < length; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, length);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }
        return indices;
    }
    private IEnumerator RepeatSetRandomComponents()
    {
        while (true)
        {
            if (ComponentSpawnPoint.Length > 0)
            {
                shuffledIndices = GetShuffledIndices(ComponentSpawnPoint.Length);
            }
            DestroySpawnedObjects();
            SetRandomComponents();
            yield return new WaitForSeconds(repeatRate);
        }
    }

    private void SetRandomComponents()
    {
        for (int i = 0; i < 3; i++)
        {
            int randomSpawnPointIndex = shuffledIndices[i];
            Transform spawnPoint = ComponentSpawnPoint[randomSpawnPointIndex];
            int randomIndex = Random.Range(0, Components.Length);
            GameObject randomComponentPrefab = Components[randomIndex];
            var obj = Instantiate(randomComponentPrefab, spawnPoint.position, Quaternion.identity);
            spawnedObjects.Add(obj);
            obj.GetComponent<NetworkObject>()?.Spawn();
        }
    }

    private void DestroySpawnedObjects()
    {
        foreach (var obj in spawnedObjects)
        {
            Destroy(obj);
            obj.GetComponent<NetworkObject>()?.Despawn();
        }
        spawnedObjects.Clear();
    }
    private void HandleGameComponentConnected(GameComponent child, GameComponent parent)
    {
        spawnedObjects.Remove(child.gameObject);
    }
}
