using UnityEngine;

public class AssemblyRoomLocalPlayerManager: BaseLocalPlayerManager{
    private void Start() {
        if (FindAnyObjectByType(typeof(LocalGameManager)) == null)
        {
            StartPlayerSetup(NetworkType.Host);
            Debug.Log("There is no GameLocalPlayerManager, self start connection");
        }
    }
}