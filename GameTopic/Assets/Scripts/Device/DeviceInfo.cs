using Newtonsoft.Json;
using Unity.Netcode;
public class DeviceInfo: IInfo, INetworkSerializable
{
    public static DeviceInfo CreateFromJson(string json){
        return JsonConvert.DeserializeObject<DeviceInfo>(json);
    }
    public TreeInfo<GameComponentInfo> TreeInfo;
    public AbilityManagerInfo AbilityManagerInfo;
    public string ToJson(){
        return JsonConvert.SerializeObject(this);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter){
            string json = JsonConvert.SerializeObject(this);
            serializer.SerializeValue(ref json);
        }
        if (serializer.IsReader){
            string json = "";
            serializer.SerializeValue(ref json);
            var result = JsonConvert.DeserializeObject<DeviceInfo>(json);
            TreeInfo = result.TreeInfo;
            AbilityManagerInfo = result.AbilityManagerInfo;
        }
    }
}