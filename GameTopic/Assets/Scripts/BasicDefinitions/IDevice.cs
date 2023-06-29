

public interface IDevice{
    public void LoadDevice(DeviceInfo info);
    public DeviceInfo DumpDevice();
    public void AddComponent(IGameComponent newComponent);
    public void SetConnection(ConnectorInfo info);
    public void RemoveComponent(IGameComponent component);
    public void RemoveComponent(int componentID);

}
