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
    [SerializeField] public Button renameBTN;

    /// <summary>
    /// The file name which be store or load.
    /// </summary>
    public string fileName { get; set; }

    /// <summary>
    /// A unity action for invoke rename function to AssemblyRoom.
    /// </summary>
    public UnityAction<string,string> renameAction { get; set; }

    private void Awake() {
        fileName = displayInputField.text;
    }

    /// <summary>
    /// Open rename input text box, driven by rename button in unity editor.
    /// </summary>
    /// <param name="b"></param>
    public void SetInputActive(bool b) {
        if (displayInputField == null) { return; }
        if (b) { displayInputField.Select(); }
    }

    /// <summary>
    /// Invoke function for finish rename.
    /// Driven by inputfield in unity editor.
    /// </summary>
    public void OnFinshRename() {
        renameAction.Invoke(fileName, displayInputField.text);
        fileName = displayInputField.text;  
    }


    /// <summary>
    /// Open or Close the rename button.
    /// </summary>
    /// <param name="b"></param>
    public void SetRenameBTNActive(bool b) {
        if(renameBTN == null) { return; }

        renameBTN.gameObject.SetActive(b);
    }

    /// <summary>
    /// Return element's file name.
    /// </summary>
    /// <returns>Current FileElement's file name.</returns>
    public string GetFileName() {
        return displayInputField?.text;
    }

    /// <summary>
    /// Set default file name to UI.
    /// </summary>
    /// <param name="newfileName">file name</param>
    public void SetFileName(string newfileName) {
        fileName = newfileName;
        displayInputField.text = newfileName;
    }
}
