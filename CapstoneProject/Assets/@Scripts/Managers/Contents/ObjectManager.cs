using System.Collections.Generic;
using UnityEngine;
using static Define;

//������Ʈ���� ������ ������ ������ Ŭ����
public class ObjectManager
{
    //������ ������Ʈ���� ���� Root ������Ʈ�� ����
    public Transform GetRootTransform(string name)
    {
        //name�̶�� �̸��� root(GameObject)�� ã�´�
        GameObject root = GameObject.Find(name);
        if (root == null)//���ٸ�
            //������Ʈ�� ���� ����
            root = new GameObject { name = name };

        //�� ������Ʈ�� transform ����
        return root.transform;
    }

    public T Spawn<T>(Vector3 position, int templateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;//������Ʈ�� ������Ʈ �̸��� ������

        //������Ʈ�� Instantiate�ɶ� Awake���� Init���� �ʱ�ȭ�� ����
        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        //������Ʈ�� �Ʒ����� Ÿ�Կ� ���� ����
        BaseObject obj = go.GetComponent<BaseObject>();

        //Creature Ÿ���� ������Ʈ���
        if(obj.ObjectType == EObjectType.Creature)
        {
            //TODO
            //Creature ��ü�� ������ Creature������Ʈ�� �����ͼ�

            //Switch���� ���� CreatureType�� �ٽ� �˻�
            //Player, MonsterŸ�������� ���� Root������ ���� ��
            
            //Creature�� �⺻���� ����
        }

        //������Ʈ�� �����ϰ� �� TŸ�Կ� obj�� ����
        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseObject
    {
        EObjectType objectType = obj.ObjectType;

        if(obj.ObjectType == EObjectType.Creature)
        {
            //TODO
            //Spawn�Լ��� �����ϰ� Ÿ�� �˻��� �����ϴ� �۾�
        }

        Managers.Resource.Destroy(obj.gameObject);
    }
}
