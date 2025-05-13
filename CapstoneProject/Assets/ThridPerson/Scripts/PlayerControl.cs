using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using StarterAssets;
public class PlayerControl : MonoBehaviour
{
    // Dotween
    //https://velog.io/@hyeon23/UnityDOTween
    [Space]
    [Header("Components")]
    [SerializeField] private Animator anim; 
    [SerializeField] private ThirdPersonController thirdPersonController;
   // [SerializeField] private GameControl gameControl;
 
    [Space]
    [Header("Combat")]
    public Transform target;
    [SerializeField] private Transform attackPos;
    [Tooltip("Offset Stoping Distance")][SerializeField] private float quickAttackDeltaDistance;
    [Tooltip("Offset Stoping Distance")][SerializeField] private float heavyAttackDeltaDistance;
    [SerializeField] private float knockbackForce = 10f; 
    [SerializeField] private float airknockbackForce = 10f; 
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float reachTime = 0.3f;
    [SerializeField] private LayerMask enemyLayer;
    bool isAttacking = false;

    [Space]


    [Header("Debug")]
    [SerializeField] private bool debug = true;

    void Awake()
    {
        enemyLayer = LayerMask.GetMask("Enemy");
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput(); // 입력 처리 함수
    }

    private void FixedUpdate()
    {
        if(target == null)
        {
            return;
        }

        if((Vector3.Distance(transform.position, target.position) >= TargetDetectionControl.instance.detectionRange)) // 탐지 범위 이상이면 target 없음
        {
            NoTarget();
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Attack(0);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Attack(1);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            Attack(0);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Attack(1);
        }

    }

    #region Attack, PerformAttack, Reset Attack, Change Target
  

    public void Attack(int attackState)
    {
        if (isAttacking)
        {
            return;
        }

        thirdPersonController.canMove = false;
        TargetDetectionControl.instance.canChangeTarget = false;
        RandomAttackAnim(attackState); 
       
    }

    private void RandomAttackAnim(int attackState)
    {
        

        switch (attackState) 
        {
            case 0: //Quick Attack

                QuickAttack();
                break;

            case 1:
                HeavyAttack();
                break;

        }


       
    }

    void QuickAttack()
    {
        int attackIndex = Random.Range(1, 4);
        if (debug)
        {
            //Debug.Log(attackIndex + " attack index");
        }

        switch (attackIndex)
        {
            case 1: //punch

                if (target != null)
                {
                    MoveTowardsTarget(target.position, quickAttackDeltaDistance, "punch"); // target.position으로 deltadistance만큼 속도로 이동해서 punch 애니메이션 작동
                    isAttacking = true;
                }
                else
                {
                    thirdPersonController.canMove = true;
                    TargetDetectionControl.instance.canChangeTarget = true;
                }

                break;

            case 2: //kick

                if (target != null)
                {
                    MoveTowardsTarget(target.position, quickAttackDeltaDistance, "kick");
                    isAttacking = true;
                }
                else
                {
                    thirdPersonController.canMove = true;
                    TargetDetectionControl.instance.canChangeTarget = true;
                }
                   

                break;

            case 3: //mmakick

                if (target != null)
                {
                    MoveTowardsTarget(target.position, quickAttackDeltaDistance, "mmakick");

                    isAttacking = true;
                }
                else
                {
                    thirdPersonController.canMove = true;
                    TargetDetectionControl.instance.canChangeTarget = true;
                }
               

                break;
        }
    }

    void HeavyAttack()
    {
        int attackIndex = Random.Range(1, 3);
        //int attackIndex = 2;
        if (debug)
        {
            Debug.Log(attackIndex + " attack index");
        }

        switch (attackIndex)
        {
            case 1: //heavyAttack1

                if (target != null)
                {
                    //MoveTowardsTarget(target.position, kickDeltaDistance, "heavyAttack1");
                    FaceThis(target.position); // 목표 방향으로 회전
                    anim.SetBool("heavyAttack1", true);
                    isAttacking = true;
                  
                }
                else
                {
                    TargetDetectionControl.instance.canChangeTarget = true;
                    thirdPersonController.canMove = true;
                }


                break;

            case 2: //heavyAttack2

                if (target != null)
                {
                    //MoveTowardsTarget(target.position, kickDeltaDistance, "heavyAttack2");
                    FaceThis(target.position);
                    anim.SetBool("heavyAttack2", true);
                    isAttacking = true;
                }
                else
                {
                    thirdPersonController.canMove = true;
                    TargetDetectionControl.instance.canChangeTarget = true;
                }

                break;
        }
    }

    public void ResetAttack() // Animation Event ---- for Reset Attack
    {
        anim.SetBool("punch", false);
        anim.SetBool("kick", false);
        anim.SetBool("mmakick", false);
        anim.SetBool("heavyAttack1", false);
        anim.SetBool("heavyAttack2", false);
        thirdPersonController.canMove = true;
        TargetDetectionControl.instance.canChangeTarget = true;
        isAttacking = false;
    }

    public void PerformAttack() // Animation Event ---- for Attacking Targets
    {
        // Assuming we have a melee attack with a short range

        Debug.Log(attackPos.position);
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos.position, attackRange, enemyLayer); // attackPos에 반지름1짜리 구 생성해서 overlap 판정, enemyLayer만 검출
        //Debug.Log(hitEnemies.Length);
        foreach (Collider enemy in hitEnemies)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyRb != null)
            {
                // Calculate knockback direction
                Vector3 knockbackDirection = enemy.transform.position - transform.position; // 넉벡 방향 계산
                knockbackDirection.y = airknockbackForce; // 원래의 높이 차(y)를 무시하고 일정한 수직 넉백 힘을 넣는 것, 적을 살짝 띄우는 연출

                // Apply force to the enemy
                enemyRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
                enemyBase.SpawnHitVfx(enemyBase.transform.position);
            }
        }
    }

    private EnemyBase oldTarget; 
    private EnemyBase currentTarget; 
    public void ChangeTarget(Transform target_) // 타겟 변경 함수
    {
        
        if(target != null) // 기존 타겟이 있을 때
        {
            //oldTarget = target_.GetComponent<EnemyBase>(); //clear old target
            oldTarget.ActiveTarget(false); // 기존 타겟의 머리 위 표시 제거
        }
       
        target = target_; // 인자로 들어온 타겟으로 변경

        oldTarget = target_.GetComponent<EnemyBase>(); 
        currentTarget = target_.GetComponent<EnemyBase>(); 
        currentTarget.ActiveTarget(true); // 현재 타겟의 머리 위 표시 생성

    }

    private void NoTarget() // When player gets out of range of current Target
    {
        currentTarget.ActiveTarget(false); // 타겟 머리 위 표시 제거
        // 타겟 다 밀기
        currentTarget = null;
        oldTarget = null;
        target = null;
    }

    #endregion


    #region MoveTowards, Target Offset and FaceThis
    public void MoveTowardsTarget(Vector3 target_, float deltaDistance, string animationName_) // 타겟 앞으로 이동, 가벼운 공격
    {
        //Debug.Log("MoveTowrad");
        PerformAttackAnimation(animationName_); // 애니메이션 출력을 위해 Name으로 된 애니메이션 bool값에 true로 전달
        FaceThis(target_); // 목표로 회전
        Vector3 finalPos = TargetOffset(target_, deltaDistance); // 내 위치에서 target 위치로 deltaDistance만큼 이동한 위치 
        //Debug.Log("FinalPost = " + finalPos);
        finalPos.y = transform.position.y; // y = 0?
        transform.DOMove(finalPos, reachTime); // curPos에서 finalPos로 reachTime 초 동안 이동

    }
    
    public void GetClose() // Animation Event ---- for Moving Close to Target // 강공격할때 애니메이션에서 실행됨
    {
        Debug.Log("GetClose!!!");
        Vector3 getCloseTarget;
        if (target == null)
        {
            getCloseTarget = oldTarget.transform.position;
        }
        else
        {
            getCloseTarget = target.position;
        }
        FaceThis(getCloseTarget);
        Vector3 finalPos = TargetOffset(getCloseTarget, 1.4f);
        finalPos.y = transform.position.y;
        transform.DOMove(finalPos, 0.2f);
    }
    
    void PerformAttackAnimation(string animationName_)
    {
        anim.SetBool(animationName_, true);
    }

    public Vector3 TargetOffset(Vector3 target, float deltaDistance) // target에서 내 position으로 deltaDistance만큼 다가온 위치 리턴
    {
        Vector3 position;
        position = target;
        return Vector3.MoveTowards(position, transform.position, deltaDistance); // position에서 내 position으로 deltaDistance만큼 다가온 위치 리턴
    }
    
    public void FaceThis(Vector3 target) // 목표로 회전
    {
        Vector3 target_ = new Vector3(target.x, target.y, target.z);
        Quaternion lookAtRotation = Quaternion.LookRotation(target_ - transform.position); // 현재 위치에서 목표로 하는곳까지의 회전 양 구하기
        lookAtRotation.x = 0;
        lookAtRotation.z = 0;
        transform.DOLocalRotateQuaternion(lookAtRotation, 0.2f); // 0.2초동안 lookAtRotation으로 회전
    }
    #endregion

    void OnDrawGizmosSelected() // 플레이어 공격 판정하는 빨간 구 draw
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange); // Visualize the attack range
    }
}
