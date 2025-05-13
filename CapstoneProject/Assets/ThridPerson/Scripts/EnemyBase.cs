using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{

    [SerializeField] private GameObject hitVfx; // visual effect
    [SerializeField] private GameObject activeTargetObject; // 타겟 되면 머리위에 생기는 작은 삼각형

    // Start is called before the first frame update
    void Start()
    {
        ActiveTarget(false);
    }

  
    public void SpawnHitVfx(Vector3 Pos_) // hit 판정 난곳에 vfx 소환
    {
        Instantiate(hitVfx, Pos_, Quaternion.identity);
    }

    public void ActiveTarget(bool bool_) // 타깃 되면 인자로 들어온 bool값으로 조정
    {
        activeTargetObject.SetActive(bool_);
    }


}
