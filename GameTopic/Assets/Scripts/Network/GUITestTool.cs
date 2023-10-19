using UnityEngine;
using UnityEditor;
using System.Linq;
using System;


public class CustomMenuItems
{
    [MenuItem("LobbyFunction/CreateLobby")]
    private static void CreateLobby(){
        LocalGameManager.Instance.CreateLobby("TestLobby");
    }

    [MenuItem("LobbyFunction/JoinLobby")]
    private static async void JoinLobby(){
        var lobbies = await LocalGameManager.Instance.GetAllAvailableLobby();
        if (lobbies.Count() == 0){
            Debug.Log("No lobby available");
            return;
        }
        LocalGameManager.Instance.JoinLobby(lobbies.First());
    }

    [MenuItem("LobbyFunction/PrintLobbies")]
    private static async void PrintLobbies(){
        var lobbies = await LocalGameManager.Instance.GetAllAvailableLobby();
        if (lobbies.Count() == 0){
            Debug.Log("No lobby available");
            return;
        }
        foreach(var lobby in lobbies){
            Debug.Log("Lobby: " + lobby.Name);
        }
    }

    [MenuItem("LobbyFunction/PlayerReady")]
    private static void PlayerReady(){
        LocalGameManager.Instance.PlayerReady();
    }

    [MenuItem("LobbyFunction/PlayerUnready")]
    private static void PlayerUnready(){
        LocalGameManager.Instance.PlayerUnready();
    }

}

