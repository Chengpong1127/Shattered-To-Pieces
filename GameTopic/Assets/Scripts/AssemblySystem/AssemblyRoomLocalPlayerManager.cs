using UnityEngine;

public class AssemblyRoomLocalPlayerManager: LocalPlayerManager{
    private void Exiting_Enter(){
        (GameRunner as AssemblyRoomRunner).SaveCurrentDevice();
    }
}