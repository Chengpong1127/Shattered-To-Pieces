using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StoreFileCtrl : MonoBehaviour
{
    [SerializeField] public List<FileElementCtrl> fileElements;

    bool isStroe;
    string interactFileName;

    public UnityAction<string> StoreAction { get; set; }
    public UnityAction<string> LoadAction { get; set; }

    private void Awake() {
        gameObject.SetActive(false);
    }
    public void OnClickFileBTN(int btnId) {
        if(btnId < 0 || btnId >= fileElements.Count) return;

        interactFileName = fileElements[btnId].fileName;

        if (isStroe) {
            if (interactFileName == null || interactFileName == "") { Debug.Log("No file name enter."); interactFileName = "__" + btnId.ToString(); }
            fileElements[btnId].SetFileName(interactFileName);
            fileElements[btnId].renameBTN.onClick.Invoke();
            StoreAction?.Invoke(interactFileName);  
        } else {
            if (interactFileName == null || interactFileName == "") { Debug.Log("No file name enter."); return; }
            LoadAction?.Invoke(interactFileName);
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

    public void RemoveRenameAction(UnityAction<string, string> renameAction) {
        fileElements.ForEach(ele => {
            ele.renameAction -= renameAction;
        });
    }
}
