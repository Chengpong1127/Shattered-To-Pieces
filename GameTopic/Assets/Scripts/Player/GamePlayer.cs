using UnityEngine;
using Unity.Netcode;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;

public class GamePlayer: AssemblyablePlayer{
    public SkillUIController SkillUI;
    private CinemachineVirtualCamera VirtualCamera;
    [SerializeField]
    private AnimationCurve _cameraZoomInCurve;
    [SerializeField]
    private AnimationCurve _cameraZoomOutCurve;
    [SerializeField]
    private float _cameraZoomDuration = 0.5f;
    protected override async void Start(){
        base.Start();
        SkillUI.gameObject.SetActive(false);
        await UniTask.WaitUntil(() => LocalPlayerManager.RoomInstance.StateMachine.State == LocalPlayerManager.LocalPlayerStates.Gaming);
        if (IsOwner)
        {
            VirtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            SkillUI.LoadUI();
            SkillUI.gameObject.SetActive(true);
            await SkillUI.Initialize();

            await UniTask.WaitForSeconds(0.5f);
            SkillUI.HideSkillUI();

        }
    }
    [ClientRpc]
    public override void SetAssemblyMode_ClientRpc(bool enabled)
    {
        base.SetAssemblyMode_ClientRpc(enabled);
        if(IsOwner){
            switch(enabled){
                case true:
                    SkillUI.ShowSkillUI();
                    break;
                case false:
                    SkillUI.HideSkillUI();
                    break;
            }
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