using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BaseCoreComponentTest{
    [UnityTest]
    public IEnumerator GetOverlapCircleCoreComponentsAllTest(){
        var gameObject = new GameObject();
        gameObject.transform.position = Vector3.zero;
        var coreComponent = gameObject.AddComponent<TestBaseCoreComponent>();
        var componentFactory = new GameComponentFactory();

        var detectedComponents = coreComponent.TestGetOverlapCircleCoreComponentsAll(1, gameObject.transform.position);
        Assert.AreEqual(0, detectedComponents.Length);

        var detectComponent = componentFactory.CreateGameComponentObject("ControlRoom");
        detectComponent.BodyTransform.position = Vector3.zero;

        detectedComponents = coreComponent.TestGetOverlapCircleCoreComponentsAll(1, gameObject.transform.position);
        Assert.AreEqual(1, detectedComponents.Length);
        Assert.AreEqual(detectComponent.CoreComponent, detectedComponents[0]);

        detectComponent.BodyTransform.Translate(Vector3.right * 5);
        yield return null;

        detectedComponents = coreComponent.TestGetOverlapCircleCoreComponentsAll(3, gameObject.transform.position);
        Assert.AreEqual(0, detectedComponents.Length);

        detectedComponents = coreComponent.TestGetOverlapCircleCoreComponentsAll(5, gameObject.transform.position);
        Assert.AreEqual(1, detectedComponents.Length);

        var detectComponent2 = componentFactory.CreateGameComponentObject("ControlRoom");
        detectComponent2.BodyTransform.position = Vector3.zero;

        detectedComponents = coreComponent.TestGetOverlapCircleCoreComponentsAll(10, gameObject.transform.position);
        Assert.AreEqual(2, detectedComponents.Length);

    }   


    class TestBaseCoreComponent : BaseCoreComponent{
        public ICoreComponent[] TestGetOverlapCircleCoreComponentsAll(float radius, Vector2 fromPosition){
            return GetOverlapCircleCoreComponentsAll(radius, fromPosition);
        }
        protected override void Start() {
        }
    }
}