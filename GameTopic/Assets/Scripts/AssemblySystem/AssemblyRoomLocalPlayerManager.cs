using UnityEngine;

public class AssemblyRoomLocalPlayerManager: BaseLocalPlayerManager{
    protected override void PlayerSpawnSetup(){
        Player.LocalAbilityActionMap.Enable();
    }
    public override void ExitGame()
    {
        (GameRunner as AssemblyRoomRunner).SaveCurrentDevice();
        base.ExitGame();
    }
    private void Start() {
        if (FindAnyObjectByType(typeof(LocalGameManager)) == null)
        {
            StartPlayerSetup(NetworkType.Host);
            Debug.Log("There is no GameLocalPlayerManager, self start connection");
        }
    }
}