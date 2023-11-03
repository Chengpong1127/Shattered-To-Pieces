using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class NetworkToolTest
{
    [Test]
    public void AtSameSubnetTest(){
        var host1 = new NetworkHost{
            IPAndSubnetMask = new (string, string)[]{
                ("1.2.7.4", "255.255.0.0")
            }
        };

        var host2 = new NetworkHost{
            IPAndSubnetMask = new (string, string)[]{
                ("1.2.8.4", "255.255.0.0")
            }
        };

        Assert.True(NetworkTool.AtSameSubnet(host1, host2, out var _));
    }
}