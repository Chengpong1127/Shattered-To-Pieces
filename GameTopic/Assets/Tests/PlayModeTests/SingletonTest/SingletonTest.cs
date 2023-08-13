using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SingletonTest
{
    [Test]
    public void SingletonTestSimplePasses()
    {
        var testSingleton = TestSingleton.Instance;
        testSingleton.value = 1;
        Assert.AreEqual(testSingleton.value, TestSingleton.Instance.value);
    }

    [Test]
    public void SingletonTestModifyValue()
    {
        var testSingleton = TestSingleton.Instance;
        testSingleton.value = 2;
        Assert.AreEqual(testSingleton.value, TestSingleton.Instance.value);
    }

    [Test]
    public void SingletonMonoBehaviorTestSimplePasses()
    {
        var testSingletonMonoBehavior = TestSingletonMonoBehavior.Instance;
        testSingletonMonoBehavior.value = 1;
        Assert.AreEqual(testSingletonMonoBehavior.value, TestSingletonMonoBehavior.Instance.value);
    }

    [Test]
    public void SingletonMonoBehaviorTestModifyValue()
    {
        var testSingletonMonoBehavior = TestSingletonMonoBehavior.Instance;
        testSingletonMonoBehavior.value = 2;
        Assert.AreEqual(testSingletonMonoBehavior.value, TestSingletonMonoBehavior.Instance.value);
    }


    class TestSingleton : Singleton<TestSingleton>
    {
        public int value = 0;
    }

    class TestSingletonMonoBehavior : SingletonMonoBehavior<TestSingletonMonoBehavior>
    {
        public int value = 0;
    }
}