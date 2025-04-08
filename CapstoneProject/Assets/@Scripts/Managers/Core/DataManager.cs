using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value> 
{
    //Dictionary�� ����� ���Ͻ�Ű�� �Լ�
    Dictionary<Key, Value> MakeDict();
}

/// <summary>
/// DataSheet�� ������ Ŭ����
/// </summary>
public class DataManager
{
    #region �� �����͵��� Dictionary

    #endregion

    public void Init()
    {

    }

    /// <summary>
    /// Addressable�� ����� ���ҽ��� �� �ö� �ִ� ���¿��� �����͵��� ��ȯ���Ѽ� �޸𸮿� �ö�
    /// </summary>
    public Loader LoadJson<Loader,Key,Value>(string path) where Loader : ILoader<Key,Value>
    {
        //��ο� �ִ� TextAsset�� �����ͼ�
        TextAsset textAsset = Managers.Resource.Load<TextAsset>(path);
        //�� textAsset�� Json���� �Ľ�
        return JsonConvert.DeserializeObject<Loader>(textAsset.text);
    }
}
