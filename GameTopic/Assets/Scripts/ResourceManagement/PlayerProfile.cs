using Newtonsoft.Json;

public class PlayerProfile{
    public string Name;
    public string Avatar;
    public string ToJson(){
        return JsonConvert.SerializeObject(this);
    }
    public static PlayerProfile FromJson(string json){
        return JsonConvert.DeserializeObject<PlayerProfile>(json);
    }
}