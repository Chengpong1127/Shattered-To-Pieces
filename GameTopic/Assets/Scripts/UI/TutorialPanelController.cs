using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TutorialPanelController : GameWidgetController
{
    [SerializeField]
    private Sprite[] images;
    [SerializeField]
    private string[] descriptions;
    [SerializeField]
    private Button NextPageButton;
    private int Page = 0;
    public Text text;
    public Image now_Img;

    void Awake()
    {
        Debug.Assert(images != null);
    }
    public override void Show()
    {
        Page = 0;
        NextPageButton.interactable = true;
        text.color = new Color(text.color.r, text.color.g, text.color.b,1f);
        now_Img.color = new Color(now_Img.color.r, now_Img.color.g, now_Img.color.b, 1f);
        SetPage();
        base.Show();
    }
    private void SetPage()
    {
        now_Img.sprite = images[Page];
        text.text = descriptions[Page];
        now_Img.SetNativeSize();
    }
    public void OnChangePage()
    {
        if (Page + 1 == images.Length)
        {
            base.Close();
            return;
        }
        Page++;
        NextPageButton.interactable = false;
        StartCoroutine(ChangingPage());

    }
    private IEnumerator ChangingPage()
    {
        while (now_Img.color.a > 0)
        {
            float alphaChangeRate = 1.5f; 
            float newAlpha = now_Img.color.a - alphaChangeRate * Time.deltaTime;
            now_Img.color = new Color(now_Img.color.r, now_Img.color.g, now_Img.color.b, Mathf.Max(newAlpha, 0f));
            yield return null;
        }
        SetPage();
        while (now_Img.color.a < 1)
        {
            float alphaChangeRate = 1.5f;
            float newAlpha = now_Img.color.a + alphaChangeRate * Time.deltaTime;
            now_Img.color = new Color(now_Img.color.r, now_Img.color.g, now_Img.color.b, Mathf.Max(newAlpha, 0f));
            yield return null;
        }
        NextPageButton.interactable = true;
        yield return null;
    }
}
