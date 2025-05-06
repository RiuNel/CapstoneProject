using UnityEngine;

//�������� Empty Object�� ����� ��ũ��Ʈ�� �ִ� ����� �ƴ� �ڵ�μ� �ڵ����� ����

/// <summary>
/// �������� ��� �Ŵ������� ������ Ŭ����
/// </summary>
public class Managers : MonoBehaviour
{
    public static bool Initialized { get; set; } = false;//�ѹ��� �ʱ�ȭ �ϱ� ���� flag

    private static Managers _instance;//�̱��� �ν��Ͻ�
    public static Managers Instance { get { Init(); return _instance; } }

    #region Content Manager
    private GameManager _game = new GameManager();
    private ObjectManager _object = new ObjectManager();

    public static GameManager Game { get { return Instance?._game; } }
    public static ObjectManager Object { get { return Instance?._object; } }
    #endregion

    #region Core Manager
    private ResourceManager _resource = new ResourceManager();
    private DataManager _data = new DataManager();
    private PoolManager _pool = new PoolManager();
    private SceneManagerEx _scene = new SceneManagerEx();
    private SoundManager _sound = new SoundManager();
    private UIManager _ui = new UIManager();

    public static ResourceManager Resource { get { return Instance?._resource; } }
    public static DataManager Data { get { return Instance?._data; } }
    public static PoolManager Pool { get { return Instance?._pool; } }
    public static SceneManagerEx Scene { get { return Instance?._scene; } }
    public static SoundManager Sound { get { return Instance?._sound; } }
    public static UIManager UI { get { return Instance?._ui; } }
    #endregion

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
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            //�ʱ�ȭ
            _instance = go.GetComponent<Managers>();
        }
    }
}
