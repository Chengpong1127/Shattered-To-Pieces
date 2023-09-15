


using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

public class AssemblyRoomLocalPlayerManager: BaseLocalPlayerManager{
    protected override void PlayerSpawnSetup(){
        Player.LocalAbilityActionMap.Enable();
    }
}