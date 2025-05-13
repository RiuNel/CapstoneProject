using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetDetectionControl : MonoBehaviour
{

    public static TargetDetectionControl instance;

    [Header("Components")]
    public PlayerControl playerControl;

    [Header("Scene")]
    public List<Transform> allTargetsInScene = new List<Transform>();
    
    [Space]
    [Header("Target Detection")]
    public LayerMask whatIsEnemy;
    public bool canChangeTarget = true;

    [Tooltip("Detection Range: \n Player range for detecting potential targets.")]
    [Range(0f, 15f)] public float detectionRange = 10f;

    [Tooltip("Dot Product Threshold \nHigher Values: More strict alignment required \nLower Values: Allows for broader targeting")]
    [Range(0f, 1f)] public float dotProductThreshold = 0.15f;

    [Space]
    [Header("Debug")]
    public bool debug;
    public Transform checkPos;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        PopulateTargetInScene();
        StartCoroutine(RunEveryXms());
    }

    private void PopulateTargetInScene()
    {
        // Find all active GameObjects in the scene
        EnemyBase[] allGameObjects = Object.FindObjectsOfType<EnemyBase>();

        // Convert the array to a list
        List<EnemyBase> gameObjectList = new List<EnemyBase>(allGameObjects);

        // Output the number of GameObjects found
        if (debug)
            Debug.Log("Number of targets found: " + gameObjectList.Count);

        // Optionally, iterate over the list and do something with each GameObject
        foreach (EnemyBase obj in gameObjectList)
        {
            allTargetsInScene.Add(obj.transform);
        }
    }

    private IEnumerator RunEveryXms()
    {
        while (true)
        {
            yield return new WaitForSeconds(.1f); // Wait for 'x' milliseconds
            GetEnemyInInputDirection();
        }
    }

    #region Get Enemy In Input Direction

    public void GetEnemyInInputDirection() // 플레이어 입력한 방향에 근접한 적 찾기
    {
        if (canChangeTarget)
        {
            Vector3 inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized; // 플레이어 입력 방향 저장

            if (inputDirection != Vector3.zero)
            {
                inputDirection = Camera.main.transform.TransformDirection(inputDirection); //입력 방향을 local -> 월드 카메라 기준으로 방향 변환
                inputDirection.y = 0;
                inputDirection.Normalize();


                Transform closestEnemy = GetClosestEnemyInDirection(inputDirection); // 가장 근접한 적 하나 리턴

                if (closestEnemy != null && (Vector3.Distance(transform.position, closestEnemy.position)) <= detectionRange) // 적이 있고, 탐지 범위 안이라면
                {
                    playerControl.ChangeTarget(closestEnemy); // 가장 가까운 적으로 타겟 변경
                    // Do something with the closest enemy in the input direction
                    Debug.Log("Closest enemy in direction: " + closestEnemy.name);
                }
            }

        }
    }
    
    Transform GetClosestEnemyInDirection(Vector3 inputDirection) // 가장 가까운 적 찾기
    {
        Transform closestEnemy = null;
        float maxDotProduct = dotProductThreshold; // 초기 내적 값 세팅 임계값으로 설정 

        foreach (Transform enemy in allTargetsInScene) // 모든 오브젝트 중
        {
            Vector3 enemyDirection = (enemy.position - transform.position).normalized; // 적 방향
            float dotProduct = Vector3.Dot(inputDirection, enemyDirection);
            // Dot Product(내적)을 사용해서 입력 방향과 적 방향이 얼마나 일치하는지 판단.

            // 내적이 1이면 완전히 같은 방향, 0이면 직각, -1이면 반대 방향.

            

            if (dotProduct > maxDotProduct) // 일정 임계값(dotProductThreshold) 이상인 적들 중 가장 값이 큰(가장 정면에 있는) 적을 선택
            {
                maxDotProduct = dotProduct;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    #endregion

    #region Unused Code/ Might Delete Later


    #endregion
}
