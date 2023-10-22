using UnityEngine;

public class AssemblyCurtainControl : MonoBehaviour {
    public RectTransform Curtain;
    private Transform Target;



    void OnEnable()
    {
        SetTargetTransform();
    }

    void Update()
    {
        Curtain.position = Camera.main.WorldToScreenPoint(Target.position);
    }


    private void SetTargetTransform(){
        var player = Utils.GetLocalPlayer();
        Target = player.GetTracedTransform();
    }
}