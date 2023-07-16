using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FileElementCtrl : MonoBehaviour
{
    [SerializeField] TMP_InputField displayInputField;
    [SerializeField] Button renameBTN;

    public string fileName { get; set; }
    public UnityAction<string,string> renameAction { get; set; }

    private void Awake() {
        fileName = displayInputField.text;
    }

    public void SetInputActive(bool b) {
        if (displayInputField == null) { return; }
        if (b) { displayInputField.Select(); }
    }

    public void OnFinshRename() {
        renameAction.Invoke(fileName, displayInputField.text);
        // Debug.Log("finsh rename : " + displayInputField.text);
        fileName = displayInputField.text;
    }

    public void SetRenameBTNActive(bool b) {
        if(renameBTN == null) { return; }

        renameBTN.gameObject.SetActive(b);
    }

    public string GetFileName() {
        return displayInputField?.text;
    }
}
