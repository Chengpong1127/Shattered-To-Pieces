using UnityEngine;
using TMPro;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class CreateLobbyPanelController : MonoBehaviour
{
    public event Action<string, MapInfo> OnCreateLobby;
    [SerializeField]
    private TMP_InputField _lobbyNameInput;
    [SerializeField]
    private GameWidget _gameWidget;
    [SerializeField]
    private RainbowText _rainbowText;
    [SerializeField]
    private Dropdown _mapDropdown;
    private MapInfo[] availableMaps;
    void Awake()
    {
        Debug.Assert(_lobbyNameInput != null);
        Debug.Assert(_gameWidget != null);
        Debug.Assert(_rainbowText != null);
        Debug.Assert(_mapDropdown != null);
    }


    public void Show(){
        _gameWidget.Show();
        _lobbyNameInput.text = "";
        _rainbowText.StarRainbow();
        availableMaps = ResourceManager.Instance.LoadAllMapInfo().Where(map => map.Available).ToArray();
        _mapDropdown.ClearOptions();
        _mapDropdown.AddOptions(availableMaps.Select(map => map.MapName).ToList());
    }
    public void Close(){
        _gameWidget.Close();
    }
    public void OnPressCreateLobby(){
        OnCreateLobby?.Invoke(_lobbyNameInput.text, availableMaps[_mapDropdown.value]);
    }

}