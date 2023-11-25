using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pipeline : MonoBehaviour
{
    public AssemblyRoomRunner runner;
    void Start()
    {
        runner.OnBuyingGameComponent += AnimationTrigger;
    }
    public void AnimationTrigger(IGameComponent gameComponent)
    {
        // var renderers = gameComponent.BodyTransform.gameObject.GetComponentsInChildren<Renderer>();
        var renderers = (gameComponent as BaseCoreComponent).BodyRenderers.ToList();
        foreach (var renderer in renderers)
        {
            // if (renderer?.GetComponent<Target>() == null)
            renderer.enabled = false;
        }
        this.GetComponent<Animator>().SetTrigger("Trigger");
        StartCoroutine(SetSprite(gameComponent));
    }
    public IEnumerator SetSprite(IGameComponent gameComponent)
    {
        yield return new WaitForSeconds(0.3f);
        // var renderers = gameComponent.BodyTransform.gameObject.GetComponentsInChildren<Renderer>();
        var renderers = (gameComponent as BaseCoreComponent).BodyRenderers.ToList();
        foreach (var renderer in renderers)
        {
            // if (renderer?.GetComponent<Target>() == null)
            renderer.enabled = true;
        }
        yield return null;
    }
}
