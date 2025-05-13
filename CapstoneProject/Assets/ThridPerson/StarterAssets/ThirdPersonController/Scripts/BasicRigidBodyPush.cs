using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.UIElements;

public class BasicRigidBodyPush : MonoBehaviour
{
    //https://velog.io/@nagi0101/Unity-%EC%99%84%EB%B2%BD%ED%95%9C-CharacterController-%EA%B8%B0%EB%B0%98-Player%EB%A5%BC-%EC%9C%84%ED%95%B4
    // character controller
    // 특징 : 세세한 부분 프로그래밍 가능. 그러나 rigidbody 물리를 활용하지 못해 직접 중력, 점프, 이동 기능 구현해야함. ThirdPerson에 있음
    public LayerMask pushLayers;
	public bool canPush;
	[Range(0.5f, 5f)] public float strength = 1.1f;

	private void OnControllerColliderHit(ControllerColliderHit hit) // 케릭터 컨트롤러가 다른 충돌체(Collider)랑 충돌했을 때 호출
    {
		if (canPush) PushRigidBodies(hit);
    }

	private void PushRigidBodies(ControllerColliderHit hit)
	{
       // -hit.collider → 상대 Collider
       // -hit.gameObject → 상대 object
       // - hit.moveDirection → Player의 이동 방향
       // - hit.controller → Player의 CharacterController
        // https://docs.unity3d.com/ScriptReference/CharacterController.OnControllerColliderHit.html

        // make sure we hit a non kinematic rigidbody
        Rigidbody body = hit.collider.attachedRigidbody; // 상대 rigidbody 들고 오기
		if (body == null || body.isKinematic) return; // 상대 충돌 판정 없는 경우 무시

		// make sure we only push desired layer(s)
		var bodyLayerMask = 1 << body.gameObject.layer; // 레이어에서 1비트 왼쪽 쉬프트해서 마스크 값  저장
		if ((bodyLayerMask & pushLayers.value) == 0) return; // A & B = A와 B가 모두 1일 경우가 하나라도 없다면 조기 종료
                                                             // 현재 충돌한 오브젝트의 레이어가 pushLayers에 포함되지 않으면 무시하고 함수 종료
                                                             // ex) bodyLayer = 1 << 8 -> 100000000이 되고 만약 pushLayer가 7,8번을 허용하는 레이어로 갖고 있다면
                                                             // pushLayer = 110000000이라서 둘이 &연산하면 1이 리턴 됨


        // ControllerColliderHit.moveDirection은 CharacterController가 충돌한 시점에 이동하려던 방향을 나타냅니다.
        // We dont want to push objects below us
        if (hit.moveDirection.y < -0.3f) return; // 자신의 이동방향이 y의 -방향이면 조기 종료

        // Calculate push direction from move direction, horizontal motion only
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z); // 내 이동 x,z축 이동방향만 고려해서 미는 방향 결정

		// Apply the push and take strength into account
		body.AddForce(pushDir * strength, ForceMode.Impulse); // ForceMode.Impluse는 순간적인 힘.		
	}
}