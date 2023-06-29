

public interface IDevice{
    public void LoadDevice(DeviceInfo info);
    public DeviceInfo DumpDevice();
    public IGameComponent RootGameComponent { set; get; }
}
