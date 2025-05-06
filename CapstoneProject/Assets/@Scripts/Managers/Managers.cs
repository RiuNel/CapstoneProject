using UnityEngine;

//수동으로 Empty Object를 만들고 스크립트를 넣는 방식이 아닌 코드로서 자동으로 생성

/// <summary>
/// 전역으로 모든 매니저들을 관리할 클래스
/// </summary>
public class Managers : MonoBehaviour
{
    public static bool Initialized { get; set; } = false;//한번만 초기화 하기 위한 flag

    private static Managers _instance;//싱글톤 인스턴스
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
            Initialized = true;//초기화는 딱 한번만 진행

            //Managers라는 오브젝트가 없으면
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                //다시 만들어주고 Managers 컴포넌트를 추가
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            //초기화
            _instance = go.GetComponent<Managers>();
        }
    }
}
