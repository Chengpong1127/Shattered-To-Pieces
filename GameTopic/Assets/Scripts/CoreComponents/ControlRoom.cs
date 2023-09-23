using AbilitySystem.Authoring;
using System.Collections.Generic;
using UnityEngine;
using AbilitySystem;
using Unity.Netcode;
using AttributeSystem.Authoring;
using AttributeSystem.Components;
using GameplayTagNamespace.Authoring;

public class ControlRoom : BaseCoreComponent {
    [SerializeField] public Material m_Default;
    [SerializeField] public Material m_Invisible;
    [SerializeField] public Collider2D AssemblyCollider;




    [ClientRpc]
    public void Invisible_ClientRpc(bool isInvisible)
    {
        var player = Utils.GetLocalPlayerDevice();

        //Debug.Assert(player);
        //Debug.Assert(player.SelfDevice != null);
        //Debug.Assert(player.SelfDevice.RootGameComponent != null);
        //Debug.Assert(player.SelfDevice.RootGameComponent.CoreComponent != null);


        //if (player.SelfDevice.RootGameComponent.CoreComponent.GetComponent<ControlRoom>() == this) { return; }

        Invisible(isInvisible);
    }

    public void Invisible(bool isInvisible)
    {
        var baseCoreComponents = this.GetAllChildren();

        foreach (var baseComponent in baseCoreComponents)
        {
            SpriteRenderer s = baseComponent?.GetComponent<SpriteRenderer>();
            if (s != null)
                s.material = isInvisible ? m_Invisible : m_Default;
            else
            {
                for (var i = 1; i < baseComponent.transform.parent.childCount; i++)
                {
                    var anotherChild = baseComponent.transform.parent.GetChild(i);
                    for (var j = 0; j < anotherChild.childCount; j++)
                    {
                        var anotherChild_Renderer = anotherChild.GetChild(j);
                        SpriteRenderer cs = anotherChild_Renderer.gameObject?.GetComponent<SpriteRenderer>();
                        if (cs != null)
                            cs.material = isInvisible ? m_Invisible : m_Default;
                    }
                }
            }
        }
    }
}