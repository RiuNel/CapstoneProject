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

    //TODO (���� �ʿ�) -> ������Ʈ�� �߾� ��ġ�� �ƴ� �� ��ġ�� �ʱ� �����Ǿ�����
    public Vector3 CenterPosition { get { return transform.position + Vector3.up; } }

    public override bool Init()
    {
        if(base.Init() == false)
            return false;

        Rigidbody = GetComponent<Rigidbody>();

        return true;
    }

    #region Helper Func
    /// <summary>
    /// ������Ʈ �ڱ� �ڽŰ� obj���� �Ÿ��� ��ȯ��Ű�� �Լ�
    /// </summary>
    /// <param name="obj"></param>
    public Vector3 GetDistanceTargetFromObj(GameObject obj)
    {
        Vector3 position = gameObject.transform.position;

        return position;
    }
    #endregion

    #region Virtual Func
    public virtual void OnDamaged() { }
    public virtual void OnDead() { }
    #endregion
}
