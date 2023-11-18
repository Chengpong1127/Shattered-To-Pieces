using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using DG.Tweening;

public class Target : MonoBehaviour
{
    public int TargetID { get; set; }
    public Connector OwnerConnector { get; set; } = null;
    public bool IsConnected  => LinkedConnector != null;
    public Collider2D BodyCollider;

    public event Action<Connector> OnLinked;
    public event Action<Connector> OnUnlinked;

    private Connector LinkedConnector = null;
    [SerializeField]
    private SpriteRenderer Renderer;
    [SerializeField]
    private SpriteRenderer CircleRenderer;
    public Vector3 ConnectionPosition => transform.localPosition;

    private void Awake() {
        Debug.Assert(BodyCollider != null);
        Debug.Assert(Renderer != null);
        Debug.Assert(CircleRenderer != null);
        
    }
    void Start()
    {
        SetTargetDisplay(false);
    }

    public void SetOwner(Connector oc)
    {
        OwnerConnector = oc;
    }
    public void SetLink(Connector lic)
    {
        LinkedConnector = lic ?? throw new ArgumentNullException("lic");
        OnLinked?.Invoke(lic);
    }
    public void Unlink()
    {
        LinkedConnector = null;
        OnUnlinked?.Invoke(OwnerConnector);
    }

    public void SetTargetDisplay(bool display)
    {
        Renderer.enabled = display;
        BodyCollider.enabled = display;
        CircleRenderer.enabled = display;

        if (display){
            Renderer.DOFade(1, 0);
            Renderer.DOFade(0.5f, 1f).SetLoops(-1, LoopType.Yoyo);
        }else{
            Renderer.DOKill();
        }
    }

}
