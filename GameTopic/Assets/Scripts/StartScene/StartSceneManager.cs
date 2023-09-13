using UnityEngine;
using Unity.Netcode;

public class StartSceneManager: MonoBehaviour{




    void Start()
    {
        
    }



    public void EnterAssemblyRoom(){
        LocalGameManager.Instance.EnterAssemblyRoom();
    }
    public void EnterGameRoom(int roomID){

    }
}