using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.Netcode;

public class GameResult: INetworkSerializable{
    public Dictionary<ulong, int> PlayerRankMap = new();
    public ulong SelfPlayerId = 0;
    public bool IsGameAborted = false;
    public bool IsRankingGame = false;
    public int GetSelfRank(){
        return PlayerRankMap[SelfPlayerId];
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SelfPlayerId);
        serializer.SerializeValue(ref IsGameAborted);
        serializer.SerializeValue(ref IsRankingGame);
        if (serializer.IsWriter){
            string json = JsonConvert.SerializeObject(this);
            serializer.SerializeValue(ref json);
        }
        if (serializer.IsReader){
            string json = "";
            serializer.SerializeValue(ref json);
            var result = JsonConvert.DeserializeObject<GameResult>(json);
            PlayerRankMap = result.PlayerRankMap;
        }
    }
}