public class NetworkIPPort{
    public string IP;
    public ushort Port;
    public NetworkIPPort(string ip, ushort port){
        IP = ip;
        Port = port;
    }
    public override string ToString(){
        return $"{IP}:{Port}";
    }
}