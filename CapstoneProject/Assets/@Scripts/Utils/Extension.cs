using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public static class Extension
{
    /// <summary>
    /// GameObject�� T��� ������Ʈ�� ã�� ������ T ������Ʈ�� �߰���Ű�� �Լ�
    /// </summary>
    /// <returns>ã�Ƽ� ������ �� ������Ʈ�� �߰��ϰ� �� ������Ʈ�� ��ȯ</returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    /// <summary>
    /// UI�� �̺�Ʈ�� �޾� �Լ��� ���������
    /// </summary>
    /// <param name="go"></param>
    /// <param name="action">�����ų �Լ�</param>
    /// <param name="type">���� Ÿ���� �̺�Ʈ����</param>
    public static void BindEvent(this GameObject go, Action<PointerEventData> action = null, Define.EUIEvent type = Define.EUIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    /// <summary>
    /// ����Ʈ�� �޾� ���ý�Ű�� �Լ�
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">���ý�ų list</param>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            (list[k], list[n]) = (list[n], list[k]); //swap
        }
    }
}
