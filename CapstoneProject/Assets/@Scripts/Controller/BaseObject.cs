using UnityEngine;
using static Define;

//Scene의 배치될 모든 오브젝트들의 조상 클래스
public class BaseObject : InitBase
{
    //모든 오브젝트들이 가질 기본적인 컴포넌트
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public Rigidbody Rigidbody { get; protected set; }

    //다른 오브젝트들도 고유 DataID가 있을 경우를 고려
    public int DataTemplateID { get; set; }

    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        Rigidbody = GetComponent<Rigidbody>();

        return true;
    }
}
