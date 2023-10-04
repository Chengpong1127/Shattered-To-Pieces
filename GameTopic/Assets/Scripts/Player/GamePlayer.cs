using UnityEngine;
using Unity.Netcode;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;

public class GamePlayer: AssemblyablePlayer{
    public GameObject AssemblyUI;
    public GameObject SkillUI;
    private CinemachineVirtualCamera VirtualCamera;
    [SerializeField]
    private AnimationCurve _cameraZoomInCurve;
    [SerializeField]
    private AnimationCurve _cameraZoomOutCurve;
    [SerializeField]
    private float _cameraZoomDuration = 0.5f;
    protected override void Start(){
        base.Start();
        AssemblyUI.SetActive(false);
        SkillUI.SetActive(false);
        if (IsOwner)
        {
            VirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        }
    }
    [ClientRpc]
    public override void SetAssemblyMode_ClientRpc(bool enabled)
    {
        base.SetAssemblyMode_ClientRpc(enabled);
        if(IsOwner){
            AssemblyUI.SetActive(enabled);
            SkillUI.SetActive(enabled);
            if (VirtualCamera != null)
                ZoomCamera(enabled ? _cameraZoomInCurve : _cameraZoomOutCurve);
        }
        
    }
    private async void ZoomCamera(AnimationCurve curve){
        VirtualCamera.m_Lens.OrthographicSize = curve.Evaluate(0);
        var timer = 0f;
        while(timer < _cameraZoomDuration){
            timer += Time.deltaTime;
            VirtualCamera.m_Lens.OrthographicSize = curve.Evaluate(timer / _cameraZoomDuration);
            await UniTask.Yield();
        }
        VirtualCamera.m_Lens.OrthographicSize = curve.Evaluate(1);
    }


}