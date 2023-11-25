using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class LoadingUIController : MonoBehaviour
{
    [SerializeField]
    private Text LoadingText;
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