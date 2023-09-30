using UnityEngine;
using Unity.Netcode;
using System.Linq;

public class GamePlayer: AssemblyablePlayer{
    public GameObject AssemblyUI;
    public GameObject SkillUI;
    protected override void Start(){
        base.Start();
        AssemblyUI.SetActive(false);
        SkillUI.SetActive(false);
    }
    [ClientRpc]
    public override void SetAssemblyMode_ClientRpc(bool enabled)
    {
        base.SetAssemblyMode_ClientRpc(enabled);
        if(IsOwner){
            AssemblyUI.SetActive(enabled);
            SkillUI.SetActive(enabled);
        }
        
    }
}