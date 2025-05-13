 using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : BaseObject
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f; // 이동속도 보간할때 보간 속도 조절용

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f; 

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f; // 점프 대기 시간

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f; // 점프 후 낙하까지의 시간

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed; // 이동 속도
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        // 두 시간 모두 중력가속도 계산식에 영향을 받음
        private float _jumpTimeoutDelta; // 점프 쿨다운
        private float _fallTimeoutDelta; // 점프했다가 떨어지는 시간

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }
        [Space]
        [Header("Custom")]
        public bool canMove;
        [Range(0f, 1f)]
        public float directMoveBlend = 0f;

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            _hasAnimator = TryGetComponent(out _animator); // 애니메이션
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>(); // input 관리
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity(); // 점프 체크와 처리
            GroundedCheck(); // 땅에 있는지 확인
            Move(); // 이동
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump"); // 애니메이션 파라미터 중 점프 값 들고와 해쉬로 바꿔 저장. 나중에 전달항때 이 값에 전달하면 애니메이션에 jump 파라미터가 변경되는 것
            _animIDFreeFall = Animator.StringToHash("FreeFall"); // FreeFall : 중력이 작용하는 상태에서 물체가 자유롭게 움직이는 것을 의미
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck() // 땅에 있는지 확인
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z); // grounded 상태 체크하는 구의 위치 값 설정. grounded offset은 크게 할수록 구가 발 밑을 검사하고, 작게 할수록 발 위를 검사하게 됨
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore); // 물리 충돌 감지 함수. 구를 기준으로 인자 값들만큼 설정해서 출동 체크. 마지막 인자로 트리거, 콜라이더로 체크할지도 체크 가능

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded); // 애니메이션에도 땅에 닿고 있는지 전달
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition) // look의 길이 = 마우스 이동한 값, threshold보다 크다는 것은 이동 값이 계산될 정도로 유의미 하다는 것. 
                                                                               // && camera position이 lock되지 않았을 때
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime; // 마우스라면 1.0, 아니면(ex 조이스틱) deltaTime
                // 마우스는 deltaTime 쓰지 않는 이유
                // 마우스는 delta 값 자체가 프레임 비례로 나오기 때문에 그냥 사용
                // 조이스틱은 메 프레임마다 일정한 값만 이동 -> 프레임차이나면 이동량 차이남

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier; // 카메라 yaw 회전은 마우스 x좌표 이동에 비례
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier; // 카메라 pitch 회전은 마우스 y좌표 이동에 비례
               

            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue); 
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
            // 시네마신은 타겟을 따라 회전함. 스스로 회전 X. 그래서 타겟의 회전을 바꾸면 시네마신 회전도 바뀜
            // 타깃을 플레이어에 회전 따라가는 빈 오브젝트로 해서 처리하게 만듦
            //Debug.Log(CinemachineCameraTarget.transform.rotation);
        }

        private void Move() // 움직임
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed; // sprint는 달리기, sprint 따라 targerspeed 조정, 변수의 의미는 내가 목표로 하는 속도(내가 원하는 속도)

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;  // 안 움직일때 speed 0 세팅

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude; // 현재 x,z축(수평) 움직임 속도 

            float speedOffset = 0.1f; // 내가 원하는 속도에 오차범위
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f; // 이제 안쓰는것 같음. 무조껀 1

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || // 현재 수평 이속이 내가 원하는 속도보다 낮거나 높을 경우
                currentHorizontalSpeed > targetSpeed + speedOffset) // ''
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate); // 내가 원하는 속도로 보정, 수평의 이동속도를 목표로 하는 속도로 lerp

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f; // 속도를 소수점 셋째 자리까지 반올림. 이유는 float가 소수점이 커질수록 계산만 느려지기 때문
            }
            else
            {
                _speed = targetSpeed; // 내가 원하는 속도에 있다면 target speed 넘기기
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate); // animationBlend에도 targetspeed값 세팅. 애니메이션에도 캐릭도 속도에 맞춰 자연스러운 전환 유도
            if (_animationBlend < 0.01f) _animationBlend = 0f; // 너무 작은수면 0

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized; // 이동 입력 방향 + 정규화, _input.move.z가 아니고 y인 이유는 move가 vector2라서 y에 z값 있음

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero) // 이동이 있을 때만 회전
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y; // Atan2는 방향을 라디안 각도로 변경. Rad2Deg = Radian to degree. transform.eulerAngles는 각 x,y,z가 pitch, yaw, roll이 됨
                // 즉 카메라의 y의 회전값을 더해준다. 그래서 카메라 기준에서 입력 방향을 계산하게 된 것
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime); // 현재 y회전에서 부드럽게 목표로 회전하는 값 계산. _rotationVelocity 가속도?
                // RotationSmoothTime 회전 부드러움 조절. 적을수록 빠름, 

                // rotate to face input direction relative to camera position
                if (canMove) // 움직일 수 있다면
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f); // 실제 회전값 적용
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
            Vector3 directMovement = transform.forward;
            targetDirection = Vector3.Lerp(targetDirection, directMovement, directMoveBlend);
            // move the player
            if (canMove) // 이동할 수 있다면
            {
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime); // 실제 이동. 수직 가속도도 추가
            }
           
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend); // 애니메이션에 이동 속도 값 넘기기
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude); // 애니메이션에 입력 강도 넘기기
            }

            //leanAnimator.User_DeliverIsAccelerating(_input.move != Vector2.zero);
            //leanAnimator.User_DeliverIsGrounded(Grounded);
            //leanAnimator.User_DeliverAccelerationSpeed(_speed);
        }

        private void JumpAndGravity() // 로직 : 점프 직후 속도 세팅 되서 올라감 + 올라가는 도중 중력 작용해서 속도 점점 감소 -> 결국 어느 시점에서 0과 -가 되어 밑으로 내려옴
                                      // 특징 : 점프 후 땅에 닿아야 점프 쿨다운 적용. 공중에 있을 땐 점프 쿨다운 바뀌지 않음
        {
            if (Grounded) // 땅에 있을 때
            {
                // reset the fall timeout timer
                // 점프 후 낙하까지 시간(위로 뜨는 시간) 초기화
                _fallTimeoutDelta = FallTimeout; //원래 코드
                //_fallTimeoutDelta = Mathf.Sqrt(JumpHeight * -2f * Gravity) / -Gravity;

                // update animator if using character
                if (_hasAnimator) // 애니메이션 있을 때
                {
                    _animator.SetBool(_animIDJump, false); // 점프 false 설정 전달
                    _animator.SetBool(_animIDFreeFall, false); // 중력 작용중일때 움직임 가능 false 전달
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f) // 땅에 있을때
                {
                    _verticalVelocity = -2f; // 수직 가속도 마이너스 줘서 계속 땅에 있기
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && canMove) // 점프 누르고 and 점프 쿨다운이 다 되었고 and 이동가능할때
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity); // 수직 속도 계산

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true); // 점프 애니메이션 출력
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f) // 땅에 있을 때 점프 쿨다운 계산
                {
                    _jumpTimeoutDelta -= Time.deltaTime; 
                }
            }
            else // 땅에 안닿아있을 때
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout; // 점프 쿨다운 초기화

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f) // 점프 후 낙하까지의 시간 계산
                {
                    _fallTimeoutDelta -= Time.deltaTime; 
                }
                else // 점프 최대 높이일때
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true); // 움직일 수 있다. 애니메이션에 전달
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false; // 점프 중일 때 점프 안되게 input 관리
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity) 
            {
                _verticalVelocity += Gravity * Time.deltaTime; // 최대 속도 되기 전까지 더해주기
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax) // 각도 조절, 360도 넘는것조절하고 입력으로 들어온 범위로 clamp
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected() // 발 밑에 grounded 체크하는 구 보이게 해주는 함수
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent) // 발 소리 내기
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume); // clip sound를 world space point에서 나게 하기
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }
    }
}