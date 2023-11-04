using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class DebugEnterRoomWithOnePerson : EditorWindow
{
    [MenuItem("Tools/DebugEnterRoomWithOnePerson")]
    static void Init()
    {
        DebugEnterRoomWithOnePerson DebugEnterRoomWithOnePerson = GetWindow<DebugEnterRoomWithOnePerson>();
        DebugEnterRoomWithOnePerson.Show();
    }

    public void CreateGUI(){
        VisualElement root = rootVisualElement;
        var mapInfos = ResourceManager.Instance.LoadAllMapInfo();

        // list all map button to choose

        foreach(var mapInfo in mapInfos){
            var button = new Button();
            button.text = mapInfo.MapName;
            button.clicked += () => {
                Debug.Log("Enter map: " + mapInfo.MapName);
                mapInfo.MapPlayerCount = 1;
                LocalGameManager.Instance.EnterRoom(mapInfo, NetworkType.Host, null);
            };
            root.Add(button);
        }
    }
}