using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileController : GameWidgetController
{
    [SerializeField]
    private ProfileSetNameController _profileSetNameController;
    [SerializeField]
    private Text _nameText;
    [SerializeField]
    private GameObject _singleMapRecordPrefab;
    [SerializeField]
    private Transform _singleMapRecordParent;
    private PlayerProfile _playerProfile;
    private GameRecord _gameRecord;
    private SingleMapRecordController[] _singleMapRecordControllers = new SingleMapRecordController[0];
    void Awake()
    {
        Debug.Assert(_profileSetNameController != null);
        Debug.Assert(_nameText != null);
        Debug.Assert(_singleMapRecordPrefab != null);
        _profileSetNameController.OnSetName += OnSetNameHandler;
    }

    private void OnSetNameHandler(string name){
        _playerProfile.Name = name;
        UpdateUI();
        ResourceManager.Instance.SaveLocalPlayerProfile(_playerProfile);
    }
    public override void Show()
    {
        base.Show();
        _playerProfile = ResourceManager.Instance.LoadLocalPlayerProfile();
        _gameRecord = ResourceManager.Instance.LoadLocalGameRecord();
        UpdateUI();
    }
    
    private void UpdateUI(){
        _nameText.text = _playerProfile.Name;
        _singleMapRecordControllers.ToList().ForEach(x => Destroy(x.gameObject));
        _singleMapRecordControllers = _gameRecord.PlayerWinCountMap.Select(x => {
            var obj = Instantiate(_singleMapRecordPrefab, _singleMapRecordParent);
            var controller = obj.GetComponent<SingleMapRecordController>();
            controller.SetRecord(x.Key, x.Value);
            return controller;
        }).ToArray();
    }


    void OnDestroy()
    {
        _profileSetNameController.OnSetName -= OnSetNameHandler;
    }
    
}