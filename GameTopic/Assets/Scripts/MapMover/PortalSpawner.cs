using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class PortalSpawner : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public GameObject Portal;
    public Vector2 minPosition = new Vector2(0f, 0f);
    public Vector2 maxPosition = new Vector2(120f, 60f);
    public IEnumerator SpawnPortal()
    {
        while (true)
        {
            var SpawnSuccess = false;
            GameObject Portal1=null;
            GameObject Portal2=null;
            while (!SpawnSuccess)
            {
                float randomX = Random.Range(minPosition.x, maxPosition.x);
                float randomY = Random.Range(minPosition.y, maxPosition.y);
                Vector2 randomSpawnPoint = new Vector2(randomX, randomY);
                float minDistance = 20f; 
                Vector2 secondRandomSpawnPoint;
                do
                {
                    float randomX2 = Random.Range(minPosition.x, maxPosition.x);
                    float randomY2 = Random.Range(minPosition.y, maxPosition.y);

                    secondRandomSpawnPoint = new Vector2(randomX2, randomY2);

                } while (Vector2.Distance(randomSpawnPoint, secondRandomSpawnPoint) < minDistance);
                Collider2D[] collidersforFirst = Physics2D.OverlapCircleAll(randomSpawnPoint, 3f);
                Collider2D[] collidersforSecond= Physics2D.OverlapCircleAll(secondRandomSpawnPoint, 3f);
                if (collidersforFirst.Length == 0&& collidersforSecond.Length==0)
                {
                    Portal1=Instantiate(Portal, randomSpawnPoint, Quaternion.identity);
                    Portal2 = Instantiate(Portal, secondRandomSpawnPoint, Quaternion.identity);
                    Portal1.GetComponent<Portal>().destination = Portal2.transform;
                    Portal2.GetComponent<Portal>().destination = Portal1.transform;
                    Portal1.GetComponent<NetworkObject>()?.Spawn();
                    Portal2.GetComponent<NetworkObject>()?.Spawn();
                    SpawnSuccess=true;
                }
            }
            yield return new WaitForSeconds(5);
            Destroy(Portal1);
            Destroy(Portal2);
            Portal1.GetComponent<NetworkObject>()?.Despawn();
            Portal2.GetComponent<NetworkObject>()?.Despawn();
        }
    }
}
