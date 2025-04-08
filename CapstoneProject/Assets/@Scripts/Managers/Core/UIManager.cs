using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static bool Initialized { get; set; } = false;//�ѹ��� �ʱ�ȭ �ϱ� ���� flag

    private static Managers _instance;//�̱��� �ν��Ͻ�
    public static Managers Instance { get { Init(); return _instance; } }
    

    public static void Init()
    {
        if (_instance == null && Initialized == false)
        {
            Initialized = true;//�ʱ�ȭ�� �� �ѹ��� ����

            //Managers��� ������Ʈ�� ������
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                //�ٽ� ������ְ� Managers ������Ʈ�� �߰�
                //�������� Empty Object�� ����� ��ũ��Ʈ�� �ִ� ����� �ƴ� �ڵ�μ� �ڵ����� ����
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            //�ʱ�ȭ
            _instance = go.GetComponent<Managers>();
            //_instance._sound.Init();
        }
    }
}
