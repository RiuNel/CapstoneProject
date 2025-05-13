using Unity.Cinemachine;
using UnityEngine;

public class PlayerFollowCamera : InitBase
{
    private BaseObject _target;//카메라가 따라갈 베이스오브젝트
    public BaseObject Target
    {
        get { return _target; }
        set { _target = value; }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;
        //인스펙터상에 카메라 Projection Size 조절

        return true;
    }

}
