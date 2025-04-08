using UnityEngine;

/// <summary>
/// ��� ������Ʈ���� �⺻���� �ʱ�ȭ�� ����ϴ� ���� Ŭ����
/// </summary>
public class InitBase : MonoBehaviour
{
    protected bool _init = false; //�ʱ�ȭ ����
    public virtual bool Init()
    {
        if (_init) //�ѹ��̶� �ʱ�ȭ�� ������
            return false; //����

        _init = true; //�װ� �ƴҽ� �ʱ�ȭ ����
        return true;
    }

    //�ڽ� Ŭ�������� �⺻������ Awake�� ��������� �ʾҴٸ�
    private void Awake() 
    {
        //�⺻������ Init�� ȣ��
        Init();
    }
}
