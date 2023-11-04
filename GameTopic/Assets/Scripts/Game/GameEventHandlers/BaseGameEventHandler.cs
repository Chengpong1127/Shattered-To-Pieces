using Unity.Netcode;
using UnityEngine;
using Cysharp.Threading.Tasks;

public abstract class BaseGameEventHandler: NetworkBehaviour{
    protected GameRunner CurrentGameRunner => GameRunner.ServerGameRunnerInstance;
    protected LocalPlayerManager LocalPlayerManager => LocalPlayerManager.RoomInstance;
}
