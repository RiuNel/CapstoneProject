using StarterAssets;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if(base.Init()==false)
            return false;

        //GameScene에서 Scene타입 정의
        SceneType = Define.EScene.GameScene;

        ThirdPersonController player = Managers.Object.Spawn<ThirdPersonController>(new Vector3(0, 0.7f, 0), 0);

        Transform targetTransform = Util.FindChild(player.gameObject, "PlayerCameraRoot", false).transform;
        CinemachineCamera cinemachineCamera = GameObject.Find("PlayerFollowCamera").GetComponent<CinemachineCamera>();
        cinemachineCamera.Target.TrackingTarget = targetTransform;

        return true;
    }

    public override void Clear()
    {
        //TODO - Scene이 바뀔 경우 오브젝트들을 밀어주는 작업추가
    }

}
