using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class ChooseMapEditor : EditorWindow
{
    [MenuItem("Tools/ChooseMapEditor")]
    static void Init()
    {
        ChooseMapEditor chooseMapEditor = GetWindow<ChooseMapEditor>();
        chooseMapEditor.Show();
    }

    public void CreateGUI(){
        VisualElement root = rootVisualElement;
        var mapInfos = ResourceManager.Instance.LoadAllMapInfo();

        // list all map button to choose

        foreach(var mapInfo in mapInfos){
            var button = new Button();
            button.text = mapInfo.MapName;
            button.clicked += async () => {
                Debug.Log("Choose map: " + mapInfo.MapName);
                await LocalGameManager.Instance.LobbyManager.ChangeLobbyMap(mapInfo);
            };
            root.Add(button);
        }
    }


}