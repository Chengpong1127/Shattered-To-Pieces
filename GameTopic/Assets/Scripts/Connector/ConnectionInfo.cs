public class ConnectionInfo: IInfo
{
    public int linkedTargetID;
    public float connectorRotation;
    public bool IsConnected => linkedTargetID != -1;
    public static ConnectionInfo NoConnection(){
        return new ConnectionInfo{
            linkedTargetID = -1,
            connectorRotation = 0f
        };
    }
    public override bool Equals(object obj)
    {
        return obj is ConnectionInfo info &&
               linkedTargetID == info.linkedTargetID &&
               connectorRotation == info.connectorRotation;
    }
    public override int GetHashCode()
    {
        int hashCode = 1861411795;
        hashCode = hashCode * -1521134295 + linkedTargetID.GetHashCode();
        hashCode = hashCode * -1521134295 + connectorRotation.GetHashCode();
        return hashCode;
    }

}