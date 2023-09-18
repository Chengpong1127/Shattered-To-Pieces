using Newtonsoft.Json;


public class ConnectionInfo: IInfo
{
    public int linkedTargetID;
    public bool IsConnected => linkedTargetID != -1;
    public static ConnectionInfo NoConnection(){
        return new ConnectionInfo{
            linkedTargetID = -1,
        };
    }
    public override bool Equals(object obj)
    {
        return obj is ConnectionInfo info &&
            linkedTargetID == info.linkedTargetID;
    }
    public override int GetHashCode()
    {
        return linkedTargetID.GetHashCode();
    }

    public string ToJson(){
        return JsonConvert.SerializeObject(this);
    }

    public static ConnectionInfo CreateFromJson(string json){
        return JsonConvert.DeserializeObject<ConnectionInfo>(json);
    }

}