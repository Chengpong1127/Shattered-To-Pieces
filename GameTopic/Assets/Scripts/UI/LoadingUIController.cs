using UnityEngine;
using TMPro;

public class LoadingUIController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text LoadingText;
    [SerializeField]
    private GameWidget GameWidget;

    private void Awake() {
        Debug.Assert(LoadingText != null);
        Debug.Assert(GameWidget != null);
    }



    public void ShowLoading(string text = "Loading..."){
        LoadingText.text = text;
        GameWidget.Show();
    }

    public void FinishLoading(){
        GameWidget.Close();
    }

}