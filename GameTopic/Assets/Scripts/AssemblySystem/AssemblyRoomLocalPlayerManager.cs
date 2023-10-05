using UnityEngine;

public class AssemblyRoomLocalPlayerManager: BaseLocalPlayerManager{
    private void Exiting_Enter(){
        (GameRunner as AssemblyRoomRunner).SaveCurrentDevice();
    }
    private void Start() {
        if (FindAnyObjectByType(typeof(LocalGameManager)) == null)
        {
            StartPlayerSetup(NetworkType.Host);
            Debug.Log("There is no GameLocalPlayerManager, self start connection");
        }
    }
}