using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;


public class EventExtensionTest{

    [Test]
    public void ObjectTestCall(){
        var value = false;
        void testAction() {value = true;}
        this.StartListening("TestEvent", testAction);
        this.TriggerEvent("TestEvent");

        Assert.True(value == true);
    }
}