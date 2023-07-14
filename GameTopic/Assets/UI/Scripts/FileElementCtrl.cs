using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FileElementCtrl : MonoBehaviour
{
    [SerializeField] TMP_InputField displayInputField;
    [SerializeField] Button renameBTN;
 
    public void SetInputActive(bool b) {
        if (displayInputField == null) { return; }

        // if(EventSystem.current.currentSelectedGameObject == displayInputField.gameObject) {
        //     EventSystem.current.SetSelectedGameObject(null);
        // }
        // 
        // displayInputField.interactable = b;
        if (b) { displayInputField.Select(); }
    }

    public void OnFinshRename() {
        Debug.Log("finsh rename : " + displayInputField.text);
    }

    public void SetRenameBTNActive(bool b) {
        if(renameBTN == null) { return; }

        renameBTN.gameObject.SetActive(b);
    }

    public string GetFileName() {
        return displayInputField?.text;
    }
}
