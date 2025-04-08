using UnityEngine;
using UnityEngine.EventSystems;
using static Define;

/// <summary>
/// �⺻���� Scene���� ��ӹ��� ���� Ŭ����
/// </summary>
public abstract class BaseScene : InitBase
{
    //�ʱ� SceneŸ���� Unknown������ ����� �޾Ƽ� ������ �ʿ�
    public EScene SceneType { get; protected set; } = EScene.UnKnown;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Object obj = FindFirstObjectByType<EventSystem>();

        //���������� EventSystem�� ����
        if (obj == null)
        {
            GameObject go = new GameObject() { name = "@EventSystem" };
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }

        return true;
    }

    /// <summary>
    /// Scene�� �о��ִ� �߻� �Լ�
    /// </summary>
    public abstract void Clear();
}
