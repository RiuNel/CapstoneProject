using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx 
{
    public BaseScene CurrensScene { get { return GameObject.FindAnyObjectByType<BaseScene>(); } }

    //Scene�� �ε��ϴ� �Լ�
    public void LoadScene(Define.EScene type)
    {
        SceneManager.LoadScene(GetSceneName(type));
    }

    //SceneType�� string���� ��ȯ�Ͽ� ����
    private string GetSceneName(Define.EScene type)
    {
        string name = System.Enum.GetName(typeof(Define.EScene), type);
        return name;
    }

    public void Clear()
    {
        //CurrensScene.Clear();
    }
}
