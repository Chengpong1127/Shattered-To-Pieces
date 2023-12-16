using UnityEngine;
using Unity.Netcode;
using System.Linq;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class GamePlayer: AssemblyablePlayer{
    public SkillUIController SkillUI;
    private CinemachineVirtualCamera VirtualCamera;
    [SerializeField]
    private float _zoomInValue = 3;
    [SerializeField]
    private float _zoomOutValue = 10;
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
                DOZoomCamera(enabled ? _zoomInValue : _zoomOutValue);
        }
        
    }
    private void DOZoomCamera(float endValue){
        DOTween.To(
            () => VirtualCamera.m_Lens.OrthographicSize,
            x => VirtualCamera.m_Lens.OrthographicSize = x,
            endValue,
            _cameraZoomDuration
        ).SetEase(Ease.InOutSine);
    }


}