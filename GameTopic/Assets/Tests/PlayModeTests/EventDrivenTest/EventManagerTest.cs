using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;

public class EventManagerTest{
    [Test]
    public void SimpleTriggerTest(){
        var value = false;
        EventManager.Instance.StartListening("testEvent", ()=>value = true);
        EventManager.Instance.TriggerEvent("testEvent");
        Assert.True(value == true);
    }
    [Test]
    public void StopListeningTest(){
        var value = false;
        void handler() { value = true; }
        EventManager.Instance.StartListening("testEvent", handler);
        EventManager.Instance.TriggerEvent("testEvent");
        Assert.True(value == true);

        value = false;
        EventManager.Instance.StopListening("testEvent", handler);
        EventManager.Instance.TriggerEvent("testEvent");
        Assert.True(value == false);

    }
}