using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public static class Utils{
    public static GameObject GetGameObjectUnderMouse(){
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);

        RaycastHit2D hit;
        hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        return hit.collider != null ? hit.collider.gameObject : null;
    }
    private static PlayerDevice localPlayerDevice;
    public static PlayerDevice GetLocalPlayerDevice(){
        if(localPlayerDevice == null){
            localPlayerDevice = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerDevice>();
        }
        return localPlayerDevice;
    }
    public static GameObject GetLocalGameObjectByNetworkID(ulong networkID){
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkID, out var networkObject)){
            return networkObject.gameObject;
        }else{
            throw new System.ArgumentException("NetworkObject with id " + networkID + " not found");
        }
    }
}