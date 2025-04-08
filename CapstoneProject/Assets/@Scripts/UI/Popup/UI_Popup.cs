using UnityEngine;

/// <summary>
/// Scene상에 떠있는 팝업들 목록을 관리하는 클래스
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
