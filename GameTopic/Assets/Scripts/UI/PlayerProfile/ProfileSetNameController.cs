using UnityEngine;
using TMPro;
using System;

public class ProfileSetNameController : GameWidgetController
{
    [SerializeField]
    private TMP_InputField _inputField;
    public event Action<string> OnSetName;
    void Awake()
    {
        Debug.Assert(_inputField != null);
    }
    public void SetName_ButtonAction(){
        OnSetName?.Invoke(_inputField.text);
        Close();
    }
}