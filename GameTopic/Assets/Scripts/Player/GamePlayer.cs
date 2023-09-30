using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class GamePlayer: AssemblyablePlayer{
    public GameObject AssemblyUI;
    public GameObject SkillUI;
    [ClientRpc]
    public override void SetAssemblyMode_ClientRpc(bool enabled)
    {
        base.SetAssemblyMode_ClientRpc(enabled);
        AssemblyUI.SetActive(enabled);
        SkillUI.SetActive(enabled);
    }
}