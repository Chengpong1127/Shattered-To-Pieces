using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileController : GameWidgetController
{
    [SerializeField]
    private ProfileSetNameController _profileSetNameController;
    [SerializeField]
    private Text _nameText;
    private PlayerProfile _playerProfile;
    void Awake()
    {
        Debug.Assert(_profileSetNameController != null);
        Debug.Assert(_nameText != null);
        _profileSetNameController.OnSetName += OnSetNameHandler;
    }
    private void SetProfileUI(PlayerProfile profile){
        _nameText.text = profile.Name;
    }
    public void SetName_ButtonAction(){
        _profileSetNameController.Show();
    }

    private void OnSetNameHandler(string name){
        _playerProfile.Name = name;
        SetProfileUI(_playerProfile);
        ResourceManager.Instance.SaveLocalPlayerProfile(_playerProfile);
    }
    public override void Show()
    {
        base.Show();
        _playerProfile = ResourceManager.Instance.LoadLocalPlayerProfile();
        SetProfileUI(_playerProfile);
    }


    void OnDestroy()
    {
        _profileSetNameController.OnSetName -= OnSetNameHandler;
    }
    
}