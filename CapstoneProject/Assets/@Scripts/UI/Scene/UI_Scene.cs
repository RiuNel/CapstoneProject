using UnityEngine;

/// <summary>
/// Scene에 영구적으로 고정되어있을 UI들을 관리
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
