using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Propeller : BaseCoreComponent, IForceAddable {

    public void AddForce(Vector2 force, ForceMode2D mode)
    {
        AddForce_ClientRpc(force, mode);
    }
    [ClientRpc]
    private void AddForce_ClientRpc(Vector2 force, ForceMode2D mode)
    {
        if (IsOwner)
        {
            BodyRigidbody.AddForce(force, mode);
        }
    }
}
