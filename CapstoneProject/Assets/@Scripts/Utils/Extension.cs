using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public static class Extension
{
    /// <summary>
    /// GameObject에 T라는 컴포넌트를 찾고 없으면 T 컴포넌트를 추가시키는 함수
    /// </summary>
    /// <returns>찾아서 없으면 그 컴포넌트를 추가하고 그 컴포넌트를 반환</returns>
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    /// <summary>
    /// UI의 이벤트를 받아 함수를 실행시켜줌
    /// </summary>
    /// <param name="go"></param>
    /// <param name="action">실행시킬 함수</param>
    /// <param name="type">무슨 타입의 이벤트인지</param>
    public static void BindEvent(this GameObject go, Action<PointerEventData> action = null, Define.EUIEvent type = Define.EUIEvent.Click)
    {
        UI_Base.BindEvent(go, action, type);
    }

    /// <summary>
    /// 리스트를 받아 셔플시키는 함수
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list">셔플시킬 list</param>
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
