using System;

public interface INetworkConnector{
    public void StartConnection();
    public event Action OnAllPlayerConnected;
}