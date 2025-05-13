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
        HandleInput(); // �Է� ó�� �Լ�
    }

    private void FixedUpdate()
    {
        if(target == null)
        {
            return;
        }

        if((Vector3.Distance(transform.position, target.position) >= TargetDetectionControl.instance.detectionRange)) // Ž�� ���� �̻��̸� target ����
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
                    MoveTowardsTarget(target.position, quickAttackDeltaDistance, "punch"); // target.position���� deltadistance��ŭ �ӵ��� �̵��ؼ� punch �ִϸ��̼� �۵�
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
                    FaceThis(target.position); // ��ǥ �������� ȸ��
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
        Collider[] hitEnemies = Physics.OverlapSphere(attackPos.position, attackRange, enemyLayer); // attackPos�� ������1¥�� �� �����ؼ� overlap ����, enemyLayer�� ����
        //Debug.Log(hitEnemies.Length);
        foreach (Collider enemy in hitEnemies)
        {
            Rigidbody enemyRb = enemy.GetComponent<Rigidbody>();
            EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();
            if (enemyRb != null)
            {
                // Calculate knockback direction
                Vector3 knockbackDirection = enemy.transform.position - transform.position; // �˺� ���� ���
                knockbackDirection.y = airknockbackForce; // ������ ���� ��(y)�� �����ϰ� ������ ���� �˹� ���� �ִ� ��, ���� ��¦ ���� ����

                // Apply force to the enemy
                enemyRb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);
                enemyBase.SpawnHitVfx(enemyBase.transform.position);
            }
        }
    }

    private EnemyBase oldTarget; 
    private EnemyBase currentTarget; 
    public void ChangeTarget(Transform target_) // Ÿ�� ���� �Լ�
    {
        
        if(target != null) // ���� Ÿ���� ���� ��
        {
            //oldTarget = target_.GetComponent<EnemyBase>(); //clear old target
            oldTarget.ActiveTarget(false); // ���� Ÿ���� �Ӹ� �� ǥ�� ����
        }
       
        target = target_; // ���ڷ� ���� Ÿ������ ����

        oldTarget = target_.GetComponent<EnemyBase>(); 
        currentTarget = target_.GetComponent<EnemyBase>(); 
        currentTarget.ActiveTarget(true); // ���� Ÿ���� �Ӹ� �� ǥ�� ����

    }

    private void NoTarget() // When player gets out of range of current Target
    {
        currentTarget.ActiveTarget(false); // Ÿ�� �Ӹ� �� ǥ�� ����
        // Ÿ�� �� �б�
        currentTarget = null;
        oldTarget = null;
        target = null;
    }

    #endregion


    #region MoveTowards, Target Offset and FaceThis
    public void MoveTowardsTarget(Vector3 target_, float deltaDistance, string animationName_) // Ÿ�� ������ �̵�, ������ ����
    {
        //Debug.Log("MoveTowrad");
        PerformAttackAnimation(animationName_); // �ִϸ��̼� ����� ���� Name���� �� �ִϸ��̼� bool���� true�� ����
        FaceThis(target_); // ��ǥ�� ȸ��
        Vector3 finalPos = TargetOffset(target_, deltaDistance); // �� ��ġ���� target ��ġ�� deltaDistance��ŭ �̵��� ��ġ 
        //Debug.Log("FinalPost = " + finalPos);
        finalPos.y = transform.position.y; // y = 0?
        transform.DOMove(finalPos, reachTime); // curPos���� finalPos�� reachTime �� ���� �̵�

    }
    
    public void GetClose() // Animation Event ---- for Moving Close to Target // �������Ҷ� �ִϸ��̼ǿ��� �����
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

    public Vector3 TargetOffset(Vector3 target, float deltaDistance) // target���� �� position���� deltaDistance��ŭ �ٰ��� ��ġ ����
    {
        Vector3 position;
        position = target;
        return Vector3.MoveTowards(position, transform.position, deltaDistance); // position���� �� position���� deltaDistance��ŭ �ٰ��� ��ġ ����
    }
    
    public void FaceThis(Vector3 target) // ��ǥ�� ȸ��
    {
        Vector3 target_ = new Vector3(target.x, target.y, target.z);
        Quaternion lookAtRotation = Quaternion.LookRotation(target_ - transform.position); // ���� ��ġ���� ��ǥ�� �ϴ°������� ȸ�� �� ���ϱ�
        lookAtRotation.x = 0;
        lookAtRotation.z = 0;
        transform.DOLocalRotateQuaternion(lookAtRotation, 0.2f); // 0.2�ʵ��� lookAtRotation���� ȸ��
    }
    #endregion

    void OnDrawGizmosSelected() // �÷��̾� ���� �����ϴ� ���� �� draw
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos.position, attackRange); // Visualize the attack range
    }
}
