using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public static class Utils{
    public static GameObject GetGameObjectUnderMouse(){
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);

        RaycastHit2D hit;
        hit = Physics2D.Raycast(worldPoint, Vector2.zero);
        return hit.collider != null ? hit.collider.gameObject : null;
    }
    private static BasePlayer localPlayerDevice;
    public static BasePlayer GetLocalPlayerDevice(){
        if(localPlayerDevice == null){
            localPlayerDevice = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<BasePlayer>();
        }
        return localPlayerDevice;
    }
    public static BasePlayer ServerGetPlayerDevice(ulong playerID){
        Debug.Assert(NetworkManager.Singleton.IsServer, "ServerGetPlayerDevice can only be called on server");
        if(NetworkManager.Singleton.ConnectedClients.TryGetValue(playerID, out var networkClient)){
            return networkClient.PlayerObject.GetComponent<BasePlayer>();
        }else{
            throw new System.ArgumentException("Player with id " + playerID + " not found");
        }
    }
    public static GameObject GetLocalGameObjectByNetworkID(ulong networkID){
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkID, out var networkObject)){
            return networkObject.gameObject;
        }else{
            throw new System.ArgumentException("NetworkObject with id " + networkID + " not found");
        }
    }
    public static NetworkTransport GetGlobalTransport(){
        return NetworkManager.Singleton.NetworkConfig.NetworkTransport;
    }
}