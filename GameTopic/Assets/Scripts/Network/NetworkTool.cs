using System.Net;
using System.Net.NetworkInformation;
using System.Linq;
using Newtonsoft.Json;

public static class NetworkTool{
    public static NetworkHost GetLocalNetworkHost(){
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
        var ipAndSubnetMask = nics
            .Where(x => x.OperationalStatus == OperationalStatus.Up)
            .Select(x => x.GetIPProperties())
            .SelectMany(x => x.UnicastAddresses)
            .Where(x => x.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            .Select(x => (x.Address.ToString(), x.IPv4Mask.ToString()))
            .Where(x => x.Item1 != "127.0.0.1")
            .ToArray();
        return new NetworkHost{
            IPAndSubnetMask = ipAndSubnetMask
        };
    }

    public static bool AtSameSubnet(NetworkHost host1, NetworkHost host2, out (string, string) resultIPs){
        foreach (var (ip1, mask1) in host1.IPAndSubnetMask)
        {
            foreach (var (ip2, mask2) in host2.IPAndSubnetMask)
            {
                if (mask1 == mask2 && AtSameSubnet(ip1, ip2, mask1))
                {
                    resultIPs = (ip1, ip2);
                    return true;
                }
            }
        }
        resultIPs = ("", "");
        return false;
    }
    private static bool AtSameSubnet(string ip1, string ip2, string mask){
        var ip1Bytes = IPAddress.Parse(ip1).GetAddressBytes();
        var ip2Bytes = IPAddress.Parse(ip2).GetAddressBytes();
        var maskBytes = IPAddress.Parse(mask).GetAddressBytes();
        
        var ip1Subnet = ip1Bytes.Zip(maskBytes, (ip1Byte, maskByte) => (byte)(ip1Byte & maskByte)).ToArray();
        var ip2Subnet = ip2Bytes.Zip(maskBytes, (ip2Byte, maskByte) => (byte)(ip2Byte & maskByte)).ToArray();
        return ip1Subnet.SequenceEqual(ip2Subnet);
    }
}

public class NetworkHost{
    public (string, string)[] IPAndSubnetMask;
    
    public string ToJson(){
        return JsonConvert.SerializeObject(this);
    }
    public static NetworkHost FromJson(string json){
        return JsonConvert.DeserializeObject<NetworkHost>(json);
    }
}