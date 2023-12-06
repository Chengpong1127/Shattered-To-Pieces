using Newtonsoft.Json;
using Unity.Netcode;

public class PlayerProfile: INetworkSerializable{
    public string Name = "";
    public string Avatar = "";

    public static PlayerProfile DefaultPlayerProfile(){
        return new PlayerProfile(){
            Name = "Player",
            Avatar = "",
        };
    }
    public string ToJson(){
        return JsonConvert.SerializeObject(this);
    }
    public static PlayerProfile FromJson(string json){
        return JsonConvert.DeserializeObject<PlayerProfile>(json);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Name);
        serializer.SerializeValue(ref Avatar);
    }
}