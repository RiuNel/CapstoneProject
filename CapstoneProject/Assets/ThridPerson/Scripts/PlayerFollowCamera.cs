using Unity.Cinemachine;
using UnityEngine;

public class PlayerFollowCamera : InitBase
{
    private BaseObject _target;//ī�޶� ���� ���̽�������Ʈ
    public BaseObject Target
    {
        get { return _target; }
        set { _target = value; }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        //�ν����ͻ� ī�޶� Projection Size ����

        return true;
    }

}
