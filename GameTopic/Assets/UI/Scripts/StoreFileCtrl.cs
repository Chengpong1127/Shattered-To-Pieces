using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreFileCtrl : MonoBehaviour
{
    [SerializeField] List<FileElementCtrl> fileElements;

    bool isStroe;
    string interactFileName;

    public void OnClickFileBTN(int btnId) {
        if(btnId < 0 || btnId >= fileElements.Count) return;

        interactFileName = fileElements[btnId].GetFileName();
        Debug.Log("Get : " + interactFileName);
    }
}
