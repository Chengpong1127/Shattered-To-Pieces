using System;

public interface INetworkConnector{
    public void StartConnection(int playerCount);
    public void StopConnection();
    public event Action OnAllDeviceConnected;
}