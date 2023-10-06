using UnityEngine;
using Unity.Netcode;

public class StartSceneManager: MonoBehaviour{




    void Start()
    {
        
    }



    public void EnterAssemblyRoom(){
        LocalGameManager.Instance.EnterRoom("AssemblyRoom", NetworkType.Host);
    }
    public void EnterGameRoomAsServer(string roomName){
        LocalGameManager.Instance.EnterRoom(roomName, NetworkType.Server);
    }
    public void EnterGameRoomAsHost(string roomName){
        LocalGameManager.Instance.EnterRoom(roomName, NetworkType.Host);
    }
    public void EnterGameRoomAsClient(string roomName){
        LocalGameManager.Instance.EnterRoom(roomName, NetworkType.Client);
    }
}