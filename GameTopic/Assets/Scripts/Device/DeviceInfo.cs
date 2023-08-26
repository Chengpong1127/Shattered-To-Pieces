using Newtonsoft.Json;
public class DeviceInfo: IInfo
{
    public static DeviceInfo CreateFromJson(string json){
        return JsonConvert.DeserializeObject<DeviceInfo>(json);
    }
    public TreeInfo<GameComponentInfo> treeInfo;
    public AbilityManagerInfo abilityManagerInfo;
    public string ToJson(){
        return JsonConvert.SerializeObject(this);
    }
}