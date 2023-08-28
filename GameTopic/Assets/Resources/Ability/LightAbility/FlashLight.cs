using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class FlashLight : MonoBehaviour
{
    public RawImage Image;
    public float FadeSpeed = 60f;
    public Action Clear;
    public bool monitor_Clear;
    private BaseCoreComponent[] baseCoreComponents;
    private void Awake()
    {
        this.gameObject.GetComponent<Collider2D>().enabled = false;
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        monitor_Clear = this.transform.parent.GetChild(0).GetComponent<LightScript>().clear;
        Image =Camera.main.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
    }
    private IEnumerator Set()
    {
        var root = this.transform.root;
        while(root.parent != null)
        {
            root = root.parent;
            yield return null;
        }
        var ControlRoom = root.GetChild(0).GetComponent<BaseCoreComponent>();
        baseCoreComponents = ControlRoom.GetAllChildren();
        yield return null;

    }
    private void OnTriggerStay2D(Collider2D other)
    {
        StartCoroutine(Set());
        foreach (BaseCoreComponent bs in baseCoreComponents)
        {

            if (other.gameObject == bs.gameObject) return;
        }
        monitor_Clear = false;
       // Debug.Log(collision.transform.parent.name);
        StartCoroutine(FadeToWhite());
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        monitor_Clear = true;
        StartCoroutine(FadeToClear());
    }
    private IEnumerator FadeToWhite()
    {
        while (!monitor_Clear&&Image.color.a<=0.7)
        {
            Image.color = Color.Lerp(Image.color, Color.white, FadeSpeed * Time.deltaTime);
            yield return null;
        }

        yield return null;
    }
    private IEnumerator FadeToClear()
    {
        while(monitor_Clear)
        {
            Image.color = Color.Lerp(Image.color, Color.clear, FadeSpeed * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}
