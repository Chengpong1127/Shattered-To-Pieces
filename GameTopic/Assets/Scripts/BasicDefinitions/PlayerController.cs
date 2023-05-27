
public interface IPlayerController{
    void CreateDeviceInstance(IDeviceInfo info);
    
}
public class PlayerController : IPlayerController
{
    public void CreateDeviceInstance(IDeviceInfo info)
    {
        string encodedInfo = info.Encode();
        info.Decode(encodedInfo);
    }
}
