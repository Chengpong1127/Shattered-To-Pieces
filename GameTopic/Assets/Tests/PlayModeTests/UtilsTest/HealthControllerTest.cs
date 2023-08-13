using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class HealthControllerTest
{
    [Test]
    public void HealthControllerTestSimplePasses()
    {
        var healthController = new HealthController<float>(100);
        Assert.True(healthController != null);

        var healthController2 = new HealthController<int>(100);
        Assert.True(healthController2 != null);
    }

    [Test]
    public void HealthControllerTestInit()
    {
        var healthController = new HealthController<float>(100);
        Assert.AreEqual(healthController.MaxHealth, 100);
        Assert.AreEqual(healthController.CurrentHealth, 100);
        Assert.AreEqual(healthController.MinHealth, 0);

        healthController = new HealthController<float>(100.0f);
        Assert.AreEqual(healthController.MaxHealth, 100.0f);
        Assert.AreEqual(healthController.CurrentHealth, 100.0f);

        healthController = new HealthController<float>(100, 50, -2);
        Assert.AreEqual(healthController.MaxHealth, 100);
        Assert.AreEqual(healthController.CurrentHealth, 50);
        Assert.AreEqual(healthController.MinHealth, -2);
    }

    [Test]
    public void DamageTest(){
        var healthController = new HealthController<float>(100);
        healthController.TakeDamage(10);
        Assert.AreEqual(healthController.CurrentHealth, 90);

        healthController.TakeDamage(100);
        Assert.AreEqual(healthController.CurrentHealth, 0);

        Assert.Throws<System.ArgumentException>(() => healthController.TakeDamage(-10));

        healthController.TakeDamage(10);
        Assert.AreEqual(healthController.CurrentHealth, 0);

    }
    [Test]
    public void HealTest(){
        var healthController = new HealthController<float>(100);
        healthController.TakeDamage(10);
        healthController.TakeHeal(10);
        Assert.AreEqual(healthController.CurrentHealth, 100);

        healthController.TakeHeal(100);
        Assert.AreEqual(healthController.CurrentHealth, 100);

        Assert.Throws<System.ArgumentException>(() => healthController.TakeHeal(-10));

        healthController.TakeHeal(10);
        Assert.AreEqual(healthController.CurrentHealth, 100);
    }

    [Test]
    public void ResetTest(){
        var healthController = new HealthController<float>(100);
        healthController.TakeDamage(10);
        healthController.ResetToMaxHealth();
        Assert.AreEqual(healthController.CurrentHealth, 100);
    }

    [Test]
    public void SetCurrentHealthTest(){
        var healthController = new HealthController<float>(100);
        healthController.SetCurrentHealth(50);
        Assert.AreEqual(healthController.CurrentHealth, 50);

        Assert.Throws<System.ArgumentException>(() => healthController.SetCurrentHealth(-10));
        Assert.Throws<System.ArgumentException>(() => healthController.SetCurrentHealth(110));
    }

    [Test]
    public void DiedTest(){
        var died = false;
        var healthController = new HealthController<float>(100);
        healthController.OnDied += () => died = true;
        healthController.TakeDamage(float.PositiveInfinity);
        Assert.AreEqual(healthController.CurrentHealth, 0);
        Assert.True(healthController.IsDead);
        Assert.True(died);
    }

    [Test]
    public void HealedFullTest(){
        var healedFull = false;
        var healthController = new HealthController<float>(100);
        healthController.OnHealedFull += () => healedFull = true;
        healthController.TakeHeal(float.PositiveInfinity);
        Assert.AreEqual(healthController.CurrentHealth, 100);
        Assert.True(healedFull);
    }
}