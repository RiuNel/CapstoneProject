using UnityEngine;

/// <summary>
/// ����� �Ǵ� Enum �������� ��Ƴ��� Ŭ����
/// </summary>
public static class Define
{
    //���� Ÿ��
    public enum EScene
    {
        UnKnown,
        TitleScene,
        GameScene,
    }

    //UI �̺�Ʈ Ÿ��
    public enum EUIEvent
    {
        Click,//Ŭ��
        PointerDown,//���콺 �����¼���
        PointerUp,//���콺 ���� ����
        Drag,//�����鼭 �巡��
    }

    //Sound Ÿ��
    public enum ESound 
    {
        Bgm,
        Effect,
        Max,
    }

    //Object���� Ÿ��
    public enum EObjectType
    {
        None,
        Creature,
    }
}
