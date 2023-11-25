using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class ProfileSetNameController : GameWidgetController
{
    [SerializeField]
    private InputField _inputField;
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