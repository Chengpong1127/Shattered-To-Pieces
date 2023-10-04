


using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;
using System.Reflection;

public class AssemblyRoomLocalPlayerManager: BaseLocalPlayerManager{
    protected override void PlayerSpawnSetup(){
        Player.LocalAbilityActionMap.Enable();
    }
    public override void ExitGame()
    {
        (GameRunner as AssemblyRoomRunner).SaveCurrentDevice();
        base.ExitGame();
    }
}