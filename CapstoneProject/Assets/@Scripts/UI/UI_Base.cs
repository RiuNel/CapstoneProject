using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// ��� UI���� Ŭ�������� ��ӹ��� ���� Ŭ����
/// </summary>
public class UI_Base : InitBase
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    private void Awake()
    {
        Init();
    }

    //UI�� �ϳ��� �ν�����â���� �巡�� ��� �ϴ� ����� ����
    //�̸����� �ϳ��� ã�ư��� binding���� ��������ִ°� ��Ģ������
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[names.Length];
        //_objects ��ųʸ��� TŸ�Կ� ���� ������Ʈ �迭�� ���
        _objects.Add(typeof(T), objects);

        //enum �̸����� ��ȸ�ϸ� ������Ʈ���� ã�� �迭�� ����
        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
                objects[i] = Util.FindChild(gameObject, names[i], true);
            else
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);

            if (objects[i] == null)
                Debug.Log($"Failed to bind({names[i]})");
        }
    }

    protected void BindObjects(Type type) { Bind<GameObject>(type); }
    protected void BindImages(Type type) { Bind<Image>(type); }
    protected void BindTexts(Type type) { Bind<TMP_Text>(type); }
    protected void BindButtons(Type type) { Bind<Button>(type); }
    protected void BindToggles(Type type) { Bind<Toggle>(type); }


    /// <summary>
    /// �ε����� ���� T Ÿ���� ������Ʈ�� �������� �Լ�
    /// </summary>
    /// <typeparam name="T">T�� UnityEngine.Object�� ����ϴ� Ÿ���̿��� ��</typeparam>
    /// <param name="idx">�迭�� �ε���</param>
    /// <returns>idx�� �ش��ϴ� ������Ʈ�� �迭���� �����ͼ� T Ÿ������ casting�� ����</returns>
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;
        //_objects ��ųʸ��� T Ÿ�Կ� �����Ǵ� ������Ʈ �迭�� ã�´�
        if (_objects.TryGetValue(typeof(T), out objects) == false)
            //�ش� Ÿ���� ������Ʈ �迭�� ���� ���� ������ null
            return null;

        return objects[idx] as T;
    }

    protected GameObject GetObject(int idx) { return Get<GameObject>(idx); }
    protected TMP_Text GetText(int idx) { return Get<TMP_Text>(idx); }
    protected Button GetButton(int idx) { return Get<Button>(idx); }
    protected Image GetImage(int idx) { return Get<Image>(idx); }
    protected Toggle GetToggle(int idx) { return Get<Toggle>(idx); }

    /// <summary>
    /// � ���ӿ�����Ʈ(go)���ٰ� type�̶�� �̺�Ʈ�� �����ϰ� �Ͱ� �� �̺�Ʈ�� �߻��ϸ� action �̺�Ʈ�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="go">���� ������Ʈ</param>
    /// <param name="action">�̺�Ʈ�� �߻��� �����ų �Լ�</param>
    /// <param name="type">�����ϰ� ���� �̺�Ʈ Ÿ��</param>
    public static void BindEvent(GameObject go, Action<PointerEventData> action = null, Define.EUIEvent type = Define.EUIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.EUIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action;
                break;
            case Define.EUIEvent.PointerDown:
                evt.OnPointerDownHandler -= action;
                evt.OnPointerDownHandler += action;
                break;
            case Define.EUIEvent.PointerUp:
                evt.OnPointerUpHandler -= action;
                evt.OnPointerUpHandler += action;
                break;
            case Define.EUIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action;
                break;
        }
    }
}
