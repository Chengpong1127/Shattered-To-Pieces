using Newtonsoft.Json;
using Unity.Netcode;


public class ConnectionInfo: IInfo, INetworkSerializable
{
    public int linkedTargetID;
    public bool IsConnected => linkedTargetID != -1;
    public static ConnectionInfo NoConnection(){
        return new ConnectionInfo{
            linkedTargetID = -1,
        };
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref linkedTargetID);
    }
}