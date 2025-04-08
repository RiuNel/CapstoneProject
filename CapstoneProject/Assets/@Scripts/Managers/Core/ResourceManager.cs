using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System;

/// <summary>
/// Addressable�� ����� ���ҽ���� ��ϵ� ���ҽ����� ������ Ŭ����
/// </summary>
public class ResourceManager 
{
    //���������� �޸𸮿� ������� ���ҽ��� ���ҽ��� ������ handle
    private Dictionary<string, UnityEngine.Object> _resources = new Dictionary<string, UnityEngine.Object>();
    private Dictionary<string, AsyncOperationHandle> _handles = new Dictionary<string, AsyncOperationHandle>();

    #region Load Reosurce -> Addressable�� ��ϵ� ���ҽ����� �ϳ��� ������ ���
    public T Load<T>(string key) where T : Object
    {
        if (_resources.TryGetValue(key, out Object resource))
            return resource as T;

        //Sprite Ÿ�� ���ҽ��ε� �� �̸� �ڿ� .sprite�� ���ٸ�
        if (typeof(T) == typeof(Sprite) && key.Contains(".sprite") == false)
        {
            //�̸��ڿ� .sprite�� �߰��Ͽ� ����
            if (_resources.TryGetValue($"{key}.sprite", out resource))
                return resource as T;
        }

        return null;
    }

    //�ε��� ���ҽ��� �������̶�� Instantiate�� ���
    public GameObject Instantiate(string key, Transform parent = null, bool pooling = false)
    {
        GameObject prefab = Load<GameObject>(key);
        if (prefab == null)
        {
            //1. �ε� ���а� ������ Addressable Group�� �߰��� �Ǿ����� 1�������� �˻�
            Debug.LogError($"Failed to load prefab : {key}");
            return null;
        }

        if (pooling)
            return Managers.Pool.Pop(prefab);

        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;

        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        if (Managers.Pool.Push(go))
            return;

        Object.Destroy(go);
    }
    #endregion

    #region Addressable
    //�ʹݿ� �ʿ��� ���ҽ����� ���� �޸𸮿� ��� ����

    private void LoadAsync<T>(string key, Action<T> callback = null) where T : UnityEngine.Object
    {
        // Cache
        if (_resources.TryGetValue(key, out Object resource))
        {
            callback?.Invoke(resource as T);
            return;
        }

        string loadKey = key;
        //��������Ʈ�� Ư���� �̸����� �ٲ��ִ� ó���� ��
        if (key.Contains(".sprite"))
            loadKey = $"{key}[{key.Replace(".sprite", "")}]";

        //�װ� �ƴ϶�� �Ϲ������� �ε��Ҷ��� LoadAssetAsync�Լ� ���
        var asyncOperation = Addressables.LoadAssetAsync<T>(loadKey);
        //LoadAssetAsync�� handle�� ��ȯ�ϸ� 
        //Completed�ʿ� �ε��� �Ϸ�Ǹ� ����Ǿ����� �ϴ� �Լ��� �־���
        asyncOperation.Completed += (op) =>
        {
            //���ҽ��� handle�� �߰�
            _resources.Add(key, op.Result);
            _handles.Add(key, asyncOperation);
            callback?.Invoke(op.Result);
        };
    }

    /// <summary>
    /// ó�� ���۽� ���µ��� ���� �ε��ϴ� �Լ�
    /// </summary>
    /// <typeparam name="T">�ε��� ���ҽ��� Ÿ��</typeparam>
    /// <param name="label">Addressable �׷쿡�� �ε� �ϰ� ���� label</param>
    /// <param name="callback">string,int,int 3���� ���ڸ� �޾��ִ� �ݹ� �Լ��� ����</param>
    public void LoadAllAsync<T>(string label, Action<string, int, int> callback) where T : UnityEngine.Object
    {
        //label�� �ش��ϴ� ���ҽ����� ����� ���� ��ƿ´�
        var opHandle = Addressables.LoadResourceLocationsAsync(label, typeof(T));
        //�� ��ϵ��� �ϳ��� ��ȸ��
        opHandle.Completed += (op) =>
        {
            int loadCount = 0;//���� �ε��� ���� ����
            int totalCount = op.Result.Count;//�ε��ؾ��� ������ �� ����

            foreach (var result in op.Result)
            {
                //��������Ʈ�� ����� �ҷ����� ���ϴ� �һ�� �� ���׶����� ���� �˻�
                if (result.PrimaryKey.Contains(".sprite"))
                {
                    LoadAsync<Sprite>(result.PrimaryKey, (obj) =>
                    {
                        loadCount++;
                        //�������� � ������ �ε��ߴ����� Primarykey�� �޾��ش�
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });
                }
                else
                {
                    LoadAsync<T>(result.PrimaryKey, (obj) =>
                    {
                        loadCount++;
                        callback?.Invoke(result.PrimaryKey, loadCount, totalCount);
                    });
                }
            }
        };
    }

    public void Clear()
    {
        _resources.Clear();

        foreach (var handle in _handles)
            Addressables.Release(handle);

        _handles.Clear();
    }
    #endregion
}
