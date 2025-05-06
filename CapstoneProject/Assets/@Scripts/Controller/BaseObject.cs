using UnityEngine;
using static Define;

//Scene�� ��ġ�� ��� ������Ʈ���� ���� Ŭ����
public class BaseObject : InitBase
{
    //��� ������Ʈ���� ���� �⺻���� ������Ʈ
    public EObjectType ObjectType { get; protected set; } = EObjectType.None;
    public Rigidbody Rigidbody { get; protected set; }

    //�ٸ� ������Ʈ�鵵 ���� DataID�� ���� ��츦 ���
    public int DataTemplateID { get; set; }

    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        Rigidbody = GetComponent<Rigidbody>();

        return true;
    }
}
