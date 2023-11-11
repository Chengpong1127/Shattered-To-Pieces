using UnityEngine;
using TMPro;
using System;

public class ReadyButtonController : MonoBehaviour
{
    public event Action<ReadyButtonState> OnReadyButtonPressed;
    [SerializeField]
    private TMP_Text _readyButtonText;
    public ReadyButtonState ButtonState {get; private set;}

    void Awake()
    {
        Debug.Assert(_readyButtonText != null);
    
    }
    public void SetReady(){
        _readyButtonText.text = "Ready";
        ButtonState = ReadyButtonState.Ready;
    }
    public void SetUnready(){
        _readyButtonText.text = "Unready";
        ButtonState = ReadyButtonState.Unready;
    }
    public void OnPressReadyButton(){
        OnReadyButtonPressed?.Invoke(ButtonState);
    }
    public enum ReadyButtonState{
        Ready,
        Unready
    }
}