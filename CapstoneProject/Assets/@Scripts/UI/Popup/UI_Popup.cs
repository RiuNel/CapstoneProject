using UnityEngine;

/// <summary>
/// Scene�� ���ִ� �˾��� ����� �����ϴ� Ŭ����
/// </summary>
public class UI_Popup : UI_Base
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        Managers.UI.SetCanvas(gameObject, false);
        return true;
    }

    public virtual void ClosePopupUI()
    {
        Managers.UI.ClosePopupUI(this);
    }
}
