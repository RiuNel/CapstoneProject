using UnityEngine;

/// <summary>
/// Scene�� ���������� �����Ǿ����� UI���� ����
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
