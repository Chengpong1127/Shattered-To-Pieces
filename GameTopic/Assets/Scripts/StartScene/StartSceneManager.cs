using UnityEngine;
using Unity.Netcode;

public class StartSceneManager: MonoBehaviour{




    void Start()
    {
        
    }



    public void EnterAssemblyRoom(){
        LocalGameManager.Instance.EnterRoom("AssemblyRoom");
    }
    public void EnterGameRoom(string roomName){
        LocalGameManager.Instance.EnterRoom(roomName);
    }
}