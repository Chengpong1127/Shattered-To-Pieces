using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System;


public class EventExtensionTest{

    [Test]
    public void ObjectTestCall(){
        EventManager.Instance.ClearAllListeners();
        var value = false;
        void testAction() {value = true;}
        this.StartListening("TestEvent", testAction);
        this.TriggerEvent("TestEvent");

        Assert.True(value == true);
    }
    [Test]
    public void ObjectTestCallWithParam(){
        EventManager.Instance.ClearAllListeners();
        var value = 0;
        void testAction(int i) {value = i;}
        Action<int> action = testAction;
        this.StartListening("TestEventWithOneParam", action);
        this.TriggerEvent("TestEventWithOneParam", 5);

        Assert.True(value == 5);
    }
    [Test]
    public void ObjectTestCallWithTwoParam(){
        EventManager.Instance.ClearAllListeners();
        var value = 0;
        void testAction(int i, int j) {value = i + j;}
        Action<int, int> action = testAction;
        this.StartListening("TestEventWithTwoParam", action);
        this.TriggerEvent("TestEventWithTwoParam", 5, 10);

        Assert.True(value == 15);
    }
    [Test]
    public void ObjectTestCallWithThreeParam(){
        EventManager.Instance.ClearAllListeners();
        var value = 0;
        void testAction(int i, int j, int k) {value = i + j + k;}
        Action<int, int, int> action = testAction;
        this.StartListening("TestEventWithThreeParam", action);
        this.TriggerEvent("TestEventWithThreeParam", 5, 10, 15);

        Assert.True(value == 30);
    }

    [Test]
    public void RemoveListenerTest(){
        EventManager.Instance.ClearAllListeners();
        var value = 0;
        void testAction(int i, int j, int k) {value = i + j + k;}
        Action<int, int, int> action = testAction;
        this.StartListening("TestEventWithThreeParam", action);
        this.TriggerEvent("TestEventWithThreeParam", 5, 10, 15);

        Assert.True(value == 30);

        this.StopListening("TestEventWithThreeParam", action);
        this.TriggerEvent("TestEventWithThreeParam", 1, 10, 15);

        Assert.True(value == 30);
    }
    [Test]
    public void RemoveOneListenerFromTwo(){
        EventManager.Instance.ClearAllListeners();
        var value = 0;
        void testAction(int i, int j, int k) {value = i + j + k;}
        Action<int, int, int> action = testAction;
        this.StartListening("TestEventWithThreeParam", action);
        this.StartListening("TestEventWithThreeParam", action);
        this.TriggerEvent("TestEventWithThreeParam", 5, 10, 15);

        Assert.True(value == 30);

        this.StopListening("TestEventWithThreeParam", action);
        this.TriggerEvent("TestEventWithThreeParam", 10, 10, 15);

        Assert.True(value == 35);

        this.StopListening("TestEventWithThreeParam", action);
        this.TriggerEvent("TestEventWithThreeParam", 1, 10, 15);

        Assert.True(value == 35);
    }
}