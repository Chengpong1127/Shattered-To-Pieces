using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Target : NetworkBehaviour
{
    public int TargetID { get; set; }
    public Connector OwnerConnector { get; set; } = null;
    public bool IsConnected { get => AimerConnector != null; }
    public Collider2D BodyCollider { get; private set; } = null;

    private Connector AimerConnector = null;
    private Renderer Renderer = null;
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
    public void LinkedBy(Connector lic)
    {
        AimerConnector = lic ?? throw new System.ArgumentNullException("lic");
    }
    public void Unlink()
    {
        AimerConnector = null;
    }

    public void SetTargetDisplay(bool display)
    {
        Renderer.enabled = display;
        BodyCollider.enabled = display;
    }

}
