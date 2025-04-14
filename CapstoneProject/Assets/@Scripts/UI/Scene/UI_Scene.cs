using UnityEngine;

/// <summary>
/// Scene�� ���������� �����Ǿ����� UI���� ����,
/// �� ������ ��ũ��Ʈ�� ����� �� Ŭ������ ��� �޾ƾ���
/// </summary>
public class UI_Scene : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetCanvas(gameObject, false);

        return true;
    }
}
