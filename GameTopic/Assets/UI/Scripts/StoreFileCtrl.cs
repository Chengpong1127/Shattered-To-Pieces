using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoreFileCtrl : MonoBehaviour
{
    [SerializeField] List<FileElementCtrl> fileElements;

    bool isStroe;
    string interactFileName;

    public UnityAction<string> StoreAction { get; set; }
    public UnityAction<string> LoadAction { get; set; }

    private void Awake() {
        
    }

    public void OnClickFileBTN(int btnId) {
        if(btnId < 0 || btnId >= fileElements.Count) return;

        interactFileName = fileElements[btnId].fileName;
        Debug.Log("Get : " + interactFileName);

        if (isStroe) {
            StoreAction.Invoke(interactFileName);
        } else {
            LoadAction.Invoke(interactFileName);
        }
    }

    public void SwitchActiveAndMode(bool b) {
        if(isStroe == b) {
            this.gameObject.SetActive(!this.gameObject.activeSelf);
            return;
        }

        isStroe = b;
        this.gameObject.SetActive(true);
    }

    public void SetRenameAction(UnityAction<string,string> renameAction) {
        fileElements.ForEach(ele => {
            ele.renameAction += renameAction;
        });
    }
}
