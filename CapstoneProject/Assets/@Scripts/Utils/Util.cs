using UnityEngine;
using System;

public static class Util
{
    /// <summary>
    /// GameObject�� T��� ������Ʈ�� ã�� ������ T ������Ʈ�� �߰���Ű�� �Լ�
    /// </summary>
    /// <returns>ã�Ƽ� ������ �� ������Ʈ�� �߰��ϰ� �� ������Ʈ�� ��ȯ</returns>
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }
    
    /// <summary>
    /// GameOject�� �ڽ� ������Ʈ�� ��ȯ��Ű�� �Լ�
    /// </summary>
    /// <param name="name"></param>
    /// <param name="recursive">��������� Child�� Child�� ã����</param>
    /// <returns></returns>
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }


}
