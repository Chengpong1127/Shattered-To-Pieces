using UnityEngine;


public class AssemblyRoomRunner: BaseGameRunner{
    protected override void GameInitialize(){
    }

    protected override void PreGameStart(){
        var effectManager = new GameEffectManager();
        effectManager.Enable();
    }


    protected override void GameStart(){
    }
}