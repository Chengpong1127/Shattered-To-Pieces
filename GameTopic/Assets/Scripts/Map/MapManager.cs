using UnityEngine.Tilemaps;
using UnityEngine;
using Unity.Netcode;
public class MapManager: NetworkBehaviour{
    [SerializeField]
    private Tilemap _currentTileMap;
    public Tilemap CurrentTileMap => _currentTileMap;
    public void SetTile(Vector2Int position, string tileName, bool force){
        if (!IsServer) Debug.LogError("Only server can set tile");
        if (force) SetTile_ClientRpc(position, tileName);
        else {
            if (_currentTileMap.GetTile((Vector3Int)position) == null){
                SetTile_ClientRpc(position, tileName);
            }
        }
    }
    public void RemoveTile(Vector2Int position, bool force){
        if (!IsServer) Debug.LogError("Only server can remove tile");
        RemoveTile_ClientRpc(position);
    }
    [ClientRpc]
    private void SetTile_ClientRpc(Vector2Int position, string tileName){
        var tile = ResourceManager.Instance.LoadTile(tileName);
        _currentTileMap.SetTile((Vector3Int)position, tile);
    }
    [ClientRpc]
    private void RemoveTile_ClientRpc(Vector2Int position){
        _currentTileMap.SetTile((Vector3Int)position, null);
    }

}