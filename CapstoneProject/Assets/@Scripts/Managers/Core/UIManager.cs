using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static bool Initialized { get; set; } = false;//한번만 초기화 하기 위한 flag

    private static Managers _instance;//싱글톤 인스턴스
    public static Managers Instance { get { Init(); return _instance; } }
    

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
                //수동으로 Empty Object를 만들고 스크립트를 넣는 방식이 아닌 코드로서 자동으로 생성
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);

            //초기화
            _instance = go.GetComponent<Managers>();
            //_instance._sound.Init();
        }
    }
}
