using UnityEngine;
using static Define;

public class UI_TitleScene : UI_Scene
{

    enum GameObjects
    {
        //������Ʈ �̸��� �����ؾ���
        StartImage
    } 

    enum Texts
    {
        //������Ʈ �̸��� �����ؾ���
        DisplayText
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        //Object��� Text�� UI�� Binding
        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));

        //StartImage�� Ŭ�������� BindEvent�� �ѱ� �Լ�
        GetObject((int)GameObjects.StartImage).BindEvent((evt) =>
        {
            Debug.Log("Change Scene");
            Managers.Scene.LoadScene(EScene.DevScene);
        });

        //�ε��� �Ǳ����� ��Ȱ��ȭ ó��
        GetObject((int)GameObjects.StartImage).gameObject.SetActive(false);
        GetText((int)Texts.DisplayText).text = $"";

        StartLoadAssets();

        return true;

    }

    void StartLoadAssets()
    {
        //���ϴ� Ÿ���� ���ҽ��� �ε� ����������
        //������ ó���� ���� �ε带 �ϱ⶧���� ��� ���µ��� �ֻ��� Ŭ������ Object�� �־���
        //�񵿱�� �ε��� �Ϸ�Ǹ� �� ���� �־��ִ� �ݹ� �Լ��� ���� �� �Լ��� ����ǹǷ� ���� �Ϸ� �뺸�� �޴°�
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount)
            {
                //�ε��� ������ ������ �ʱ�ȭ
                Managers.Data.Init();

                //�ε��� ������ �Ѿ �� �ְ� �Ǹ� ���������� �̹����� �ؽ�Ʈ�� ����ȭó��
                GetObject((int)GameObjects.StartImage).gameObject.SetActive(true);
                GetText((int)Texts.DisplayText).text = $"Touch To Start";

                //�ε��� ������ �Ʒ�ó�� �����͸� ��밡����
                //Managers.Data.TestDic;
            }
        });
    }
}
