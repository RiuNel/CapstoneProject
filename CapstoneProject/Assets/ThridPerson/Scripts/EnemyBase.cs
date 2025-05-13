using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    [SerializeField] private GameObject hitVfx; // visual effect
    [SerializeField] private GameObject activeTargetObject; // Ÿ�� �Ǹ� �Ӹ����� ����� ���� �ﰢ��

    // Start is called before the first frame update
    void Start()
    {
        ActiveTarget(false);
    }

  
    public void SpawnHitVfx(Vector3 Pos_) // hit ���� ������ vfx ��ȯ
    {
        Instantiate(hitVfx, Pos_, Quaternion.identity);
    }

    public void ActiveTarget(bool bool_) // Ÿ�� �Ǹ� ���ڷ� ���� bool������ ����
    {
        activeTargetObject.SetActive(bool_);
    }


}
