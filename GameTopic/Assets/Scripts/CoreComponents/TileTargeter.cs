using UnityEngine;


public class TileTargeter: BaseCoreComponent, ITileTargeter{
    public Transform TargetTransform;
    public Vector2Int[] GetTargetTileCoordinates(){
        var tilemap = GameRunner.ServerGameRunnerInstance.MapManager.CurrentTileMap;
        var targetPoints = new Vector2Int[1];
        targetPoints[0] = (Vector2Int)tilemap.WorldToCell(TargetTransform.position);
        return targetPoints;
    }
}