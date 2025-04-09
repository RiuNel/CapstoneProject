using System;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// � UI�� Ŭ���Ǵ� �巡�� ������ ����� �Լ��� �����ϴ� Ŭ���� (UI���ٰ� � �̺�Ʈ�� ���̰������)
/// </summary>
public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    //���� ���������� EventSystem ������Ʈ���� ����� ����
    public event Action<PointerEventData> OnClickHandler = null;//���콺 ��ư Ŭ�������� �߻��ϴ� �̺�Ʈ
    public event Action<PointerEventData> OnPointerDownHandler = null;
    public event Action<PointerEventData> OnPointerUpHandler = null;
    public event Action<PointerEventData> OnDragHandler = null;


    /// <summary>
    /// ���콺�� Ŭ�������� �߻��� �̺�Ʈ
    /// </summary>
    /// <param name="eventData">�̺�Ʈ �߻��� �����ų �Լ�</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickHandler?.Invoke(eventData);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDownHandler?.Invoke(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUpHandler?.Invoke(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        OnDragHandler?.Invoke(eventData);
    }
}
