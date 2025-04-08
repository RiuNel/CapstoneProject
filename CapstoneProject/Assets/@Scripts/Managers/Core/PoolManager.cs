using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

/// <summary>
/// ������Ʈ�� Ȱ��ȭ,��Ȱ��ȭ ���·� �ٲٸ鼭 ��ü�� ��� = Pooling (�޸𸮿��� ���� x)
/// </summary>
internal class Pool
{
    private GameObject _prefab;
    private IObjectPool<GameObject> _pool;

    private Transform _root;

    public Transform Root //Pooling�� ������Ʈ���� ��ġ�� Root
    {
        get 
        {
            if(_pool == null)
            {
                GameObject go = new GameObject { name = $@"{_prefab.name}" };
                _root = go.transform;
            }
            return _root;
        }
    }

    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        //Pool�� ����
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public void Push(GameObject go)
    {
        if (go.activeSelf)
            _pool.Release(go);
    }

    public GameObject Pop()
    {
        return _pool.Get();
    }

    #region Helper Func
    /// <summary>
    /// ������Ʈ�� ó�� ���鶧
    /// </summary>
    private GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab);
        go.transform.SetParent(Root);
        go.name = _prefab.name;
        return go;
    }

    /// <summary>
    /// ������Ʈ�� ų��
    /// </summary>
    private void OnGet(GameObject go)
    {
        go.SetActive(true);
    }

    /// <summary>
    /// ������Ʈ�� ����
    /// </summary>
    private void OnRelease(GameObject go)
    {
        go.SetActive(false);
    }

    /// <summary>
    /// �Ϻ��ϰ� �����ؾ� �Ҷ�
    /// </summary>
    private void OnDestroy(GameObject go)
    {
        GameObject.Destroy(go);
    }
    #endregion
}

public class PoolManager
{
    private Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    /// <summary>
    /// Pool ����ҿ��� ��������
    /// </summary>
    public GameObject Pop(GameObject prefab)
    {
        //�������� �̸��� �״�� Ű������ ���
        if (_pools.ContainsKey(prefab.name) == false)
            CreatePool(prefab);

        return _pools[prefab.name].Pop();
    }

    /// <summary>
    /// Pool ����ҿ� �ݳ� 
    /// </summary>
    public bool Push(GameObject go)
    {
        if (_pools.ContainsKey(go.name) == false)
            return false;

        _pools[go.name].Push(go);
        return true;
    }

    /// <summary>
    /// Pool ����Ҹ� �ʱ�ȭ
    /// </summary>
    public void Clear()
    {
        _pools.Clear();
    }

    private void CreatePool(GameObject original)
    {
        Pool pool = new Pool(original);
        _pools.Add(original.name, pool);
    }
}
