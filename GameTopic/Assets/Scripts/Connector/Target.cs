using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Target : MonoBehaviour
{
    public int TargetID { get; set; }
    public Connector OwnerConnector { get; set; } = null;
    public bool IsConnected  => LinkedConnector != null;
    public Collider2D BodyCollider { get; private set; } = null;

    public event Action<Connector> OnLinked;
    public event Action<Connector> OnUnlinked;

    private Connector LinkedConnector = null;
    private Renderer Renderer = null;
    public SpriteRenderer CircleRenderer = null;
    public Vector3 ConnectionPosition => transform.localPosition;

    private void Awake() {
        Renderer = GetComponent<Renderer>();
        if (Renderer == null) {
            Debug.LogWarning("Target: Renderer is null");
        }
        BodyCollider = GetComponent<Collider2D>();
        if (BodyCollider == null) {
            Debug.LogWarning("Target: BodyCollider is null");
        }
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
    }

}
