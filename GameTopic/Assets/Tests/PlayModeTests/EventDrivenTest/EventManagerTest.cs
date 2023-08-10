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
        EventManager.Instance.AddListener("testEvent", new Action(()=>value = true));
        EventManager.Instance.TriggerEvent("testEvent");
        Assert.True(value == true);
    }
    [Test]
    public void StopListeningTest(){
        var value = false;
        void handler() { value = true; }
        Action handlerAction = handler;
        EventManager.Instance.AddListener("testEvent", handlerAction);
        EventManager.Instance.TriggerEvent("testEvent");
        Assert.True(value == true);

        value = false;
        EventManager.Instance.RemoveListener("testEvent", handlerAction);
        EventManager.Instance.TriggerEvent("testEvent");
        Assert.True(value == false);

    }
    [Test]
    public void MultipleTriggerTest(){
        var value = 0;
        void handler() { value++; }
        Action handlerAction = handler;
        EventManager.Instance.AddListener("testEvent", handlerAction);
        EventManager.Instance.TriggerEvent("testEvent");
        EventManager.Instance.TriggerEvent("testEvent");
        EventManager.Instance.TriggerEvent("testEvent");
        Assert.True(value == 3);
    }

    [Test]
    public void MultipleEventTest(){
        var value = 0;
        void handler() { value++; }
        Action handlerAction = handler;
        EventManager.Instance.AddListener("testEvent", handlerAction);
        EventManager.Instance.AddListener("testEvent2", handlerAction);
        EventManager.Instance.TriggerEvent("testEvent");
        EventManager.Instance.TriggerEvent("testEvent2");
        Assert.True(value == 2);
    }
    [Test]
    public void OneParamTest(){
        var value = 0;
        void handler(int i) { value = i; }
        Action<int> handlerAction = handler;
        EventManager.Instance.AddListener("testEventWithOneParam", handlerAction);
        EventManager.Instance.TriggerEvent("testEventWithOneParam", 5);
        Assert.True(value == 5);
    }
}