using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.Netcode;
public class FlashLight : MonoBehaviour
{
    [SerializeField]
    public GameObject canvas;
    public RawImage Image;
    public float FadeSpeed = 60f;
    public Action Clear;
    public bool monitor_Clear;
    private BaseCoreComponent[] baseCoreComponents;
    private BaseCoreComponent controlRoom;
    private void Awake()
    {
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        monitor_Clear = this.GetComponentInParent<LightScript>().clear;
        if (Camera.main.GetComponentInChildren<Canvas>() == null)
        {
            var s=Instantiate(canvas);
            s.GetComponent<NetworkObject>()?.Spawn();
            s.transform.parent = Camera.main.transform;
        }
        Image =Camera.main.transform.GetChild(0).GetChild(0).GetComponent<RawImage>();
    }
    private IEnumerator Set()
    {
        this.gameObject.AddComponent<BoxCollider2D>().enabled=false;
        var root = this.GetComponentInParent<BaseCoreComponent>();
        controlRoom = root.GetRoot() as BaseCoreComponent;
        var baseCoreComponents = controlRoom.GetComponentInChildren<BaseCoreComponent>();
        yield return null;
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        StartCoroutine(Set());

            if(other.gameObject.GetComponentInParent<BaseCoreComponent>()!=null)
            if (other.gameObject.GetComponentInParent<BaseCoreComponent>().GetRoot() as BaseCoreComponent == controlRoom) return;

        monitor_Clear = false;
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
