using StarterAssets;
using UnityEngine;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if(base.Init()==false)
            return false;

        //GameScene���� SceneŸ�� ����
        SceneType = Define.EScene.GameScene; 

        return true;
    }
    public override void Clear()
    {
        //TODO - Scene�� �ٲ� ��� ������Ʈ���� �о��ִ� �۾��߰�
    }

    public void Start()
    {
        Managers.Object.Spawn<ThirdPersonController>(new Vector3(0, 0.7f, 0), 0);
    }
}
