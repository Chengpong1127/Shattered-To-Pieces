using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class StartSceneManager: MonoBehaviour{
    public TMP_InputField IPField;
    public TMP_InputField PortField;

    public NetworkIPPort NetworkIPPort => new NetworkIPPort(IPField.text, ushort.Parse(PortField.text));
    

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
        LocalGameManager.Instance.EnterRoom(roomName, NetworkType.Host, NetworkIPPort);
    }
    public void EnterGameRoomAsClient(string roomName){
        LocalGameManager.Instance.EnterRoom(roomName, NetworkType.Client, NetworkIPPort);
    }
}