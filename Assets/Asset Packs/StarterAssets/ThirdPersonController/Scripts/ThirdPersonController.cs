using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using Cinemachine;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.05f;

        [SerializeField] Transform UpperBody;

        private float CameraSensitivity = 1f;

        [SerializeField] private AudioClip LandingAudioClip;
        [SerializeField] private AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        //[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        //public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool FlyMode = false;
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
        private CinemachineVirtualCamera _cinemachineLockCamera;
        // player
        private float SpeedChangeRate = 10.0f;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private Vector3 inputDirection;
        

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _footstepTimoutDelta;

        // animation IDs
        private int _animIDStrafeX;
        private int _animIDStrafeZ;
        private int _animIDGrounded;
        private int _animIDJump;
        //private int _animIDFreeFall;
        private int _animIDMotionSpeed;



#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private ShooterController _shooterController;
        private BuildModeController _buildModeController;
        private PlayerStats _stats;
        private Boost _boost;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            _stats = GetComponent<PlayerStats>();
            _boost = GetComponent<Boost>();
            _shooterController = GetComponent<ShooterController>();
            _buildModeController = GetComponent<BuildModeController>();
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _animator = GetComponent<Animator>();
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();

            _cinemachineLockCamera = _shooterController.LockCamera;
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            //_fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            if (_stats.PlayerDead || GameStateManager.Instance.IsPlayingCutscene)
            {
                _input.jump = false;
                _input.drop = false;
                if (_stats.PlayerDead)
                    FlyMode = false;
            }
            JumpAndGravity();
            GroundedCheck();
            RotateToFaceForward();
            Move();
            if (_footstepTimoutDelta > 0)
            {
                _footstepTimoutDelta -= Time.deltaTime;
            }
        }

        private void LateUpdate()
        {
            if (_buildModeController.GetBuildMode())
            {
                return;
            }

            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDStrafeX = Animator.StringToHash("StrafingX");
            _animIDStrafeZ = Animator.StringToHash("StrafingZ");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            //_animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            _animator.SetBool(_animIDGrounded, Grounded);
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier * CameraSensitivity;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier * CameraSensitivity;
            }
            if (LockCameraPosition)
            {
                Transform target = _shooterController.AimTarget;
                CinemachineCameraTarget.transform.LookAt(target);
                _cinemachineTargetPitch = CinemachineCameraTarget.transform.rotation.eulerAngles.x;
                _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y - 1.907f;
                if (_cinemachineTargetPitch > 180)
                {
                    _cinemachineTargetPitch -= 360;
                }
                /*
                 * if camera rotation eulerAngle.x is ~90, camera will spin around super fast
                 * this will happen if target is directly above or below player
                 * below is the code to simply unlock and avoid that problem
                 * but this doesn't feel good
                 */
                if (Mathf.Abs(_cinemachineTargetPitch) > 80)
                {
                    _shooterController.TargetLockToggle(false);
                    Debug.Log("Target Lock angle limit reached. Auto unlock.");
                }
                //_cinemachineLockCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>().Damping.x = 10;

            }
            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);

        }


        private void RotateToFaceForward()
        {
            if (_stats.PlayerDead || GameStateManager.Instance.IsPlayingCutscene)
            {
                _targetRotation = 0f;
                return;
            }
            // normalise input direction
            inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y);
            if (_input.move != Vector2.zero || _boost.GetBoostState() != Boost.BoostState.Stop)
            {
                _targetRotation = Mathf.Atan2(inputDirection.normalized.x, inputDirection.normalized.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
            }

            // no turning for dash attack
            if (_buildModeController.GetBuildMode() ||
                _boost.GetBoostState() == Boost.BoostState.DashAccel ||
                _boost.GetBoostState() == Boost.BoostState.DashDecel)
            {
                return;
            }
            float rotationSmoothTime;
            if (_animator.GetBool("Attacking"))
            {
                rotationSmoothTime = RotationSmoothTime * 5f;
            }
            else
            {
                rotationSmoothTime = RotationSmoothTime;
            }
            float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, Camera.main.transform.eulerAngles.y, ref _rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, yAngle, 0f);

        }

        private void Move()
        {

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed;
            if (_boost.GetBoostState() != Boost.BoostState.Stop)
            {
                targetSpeed = _boost.GetBoostTargetSpeed();
                SpeedChangeRate = _boost.GetBoostTargetAccel();
            }
            // if there is no legal input, set the target speed to 0
            else if (_buildModeController.GetBuildMode() || 
                _input.move == Vector2.zero || _stats.PlayerDead || 
                GameStateManager.Instance.IsPlayingCutscene)
            {
                targetSpeed = 0.0f;
                SpeedChangeRate = _stats.WalkAcceleration;
            }
            else if (_animator.GetBool("Attacking"))
            {
                targetSpeed = _stats.MoveSpeed / 2;
                SpeedChangeRate = _stats.WalkAcceleration;
            }
            else
            {
                targetSpeed = _stats.MoveSpeed;
                SpeedChangeRate = _stats.WalkAcceleration;
            }
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            Vector3 targetDirection;
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            if (_boost.GetBoostState() == Boost.BoostState.DashAccel || _boost.GetBoostState() == Boost.BoostState.DashDecel)
            {
                targetDirection = transform.forward;
            }
            else
            {
                targetDirection = (Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward).normalized;
            }

            Vector3 movement = Vector3.Lerp(new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z), targetDirection * targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            // move the player
            _controller.Move(movement * Time.deltaTime + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            Vector3 LocalSpeed = transform.InverseTransformDirection(movement);
            // update animator if using character
            float lerpX = Mathf.Lerp(_animator.GetFloat(_animIDStrafeX), LocalSpeed.x, 10f * Time.deltaTime);
            float lerpZ = Mathf.Lerp(_animator.GetFloat(_animIDStrafeZ), LocalSpeed.z, 10f * Time.deltaTime);
            _animator.SetFloat(_animIDStrafeX, lerpX);
            _animator.SetFloat(_animIDStrafeZ, lerpZ);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
        
        private void JumpAndGravity()
        {
            if (_buildModeController.GetBuildMode())
            {
                _input.jump = false;
                _input.drop = false;
            }
            if (Grounded)
            {
                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    if(_boost.GetBoostState() != Boost.BoostState.Stop)
                    {
                        //set lower so player can fly while boosting on ground
                        _verticalVelocity = -1f;
                    }
                    else
                    {
                        _verticalVelocity = -5f;
                    }
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f && _boost.GetBoostState() == Boost.BoostState.Stop)
                {
                    if(_boost.GetBoostState() == Boost.BoostState.Stop)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            // not grounded
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;
            }

            //key pressed and not overheat. In the air or boosting on ground
            if (_input.jump && (!Grounded || _boost.GetBoostState() != Boost.BoostState.Stop))
            {
                FlyMode = true;
            }
            else if(!_input.jump && Grounded && _boost.GetBoostState() == Boost.BoostState.Stop)
            {
                FlyMode = false;
            }
            if (_boost.OverHeat)
            {
                FlyMode = false;
            }

            //flying and gravity
            if (FlyMode)
            {
                if (_input.jump)
                {
                    _verticalVelocity = Mathf.Lerp(_verticalVelocity, _stats.VerticalFlightSpeed, _stats.VerticalAcceleration * Time.deltaTime);
                }
                else if (_input.drop)
                {
                    _animator.SetBool(_animIDJump, false);
                    _verticalVelocity = Mathf.Lerp(_verticalVelocity, -_stats.VerticalFlightSpeed, _stats.VerticalAcceleration * Time.deltaTime);
                }
                else
                {
                    _verticalVelocity = Mathf.Lerp(_verticalVelocity, 0, _stats.VerticalAcceleration * Time.deltaTime);
                }
            }
            else
            {
                if (_verticalVelocity < _terminalVelocity)
                {
                    _verticalVelocity += Gravity * Time.deltaTime;
                }
                if (_input.drop)
                {
                    _animator.SetBool(_animIDJump, false);
                    if(_verticalVelocity > -_stats.VerticalFlightSpeed)
                    {
                        _verticalVelocity = Mathf.Lerp(_verticalVelocity, -_stats.VerticalFlightSpeed, _stats.VerticalAcceleration * Time.deltaTime);
                    }
                }
            }


        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
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

        //TODO: fix footstep in blendtree
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.2f && _footstepTimoutDelta <= 0)
            {
                _footstepTimoutDelta = .1f;
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand()
        {
            FlyMode = false;
            AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
        }

        private void OnJumpStart()
        {
            // the square root of H * -2 * G = how much velocity needed to reach desired height
            if (_animator.GetBool(_animIDJump))
            {
                _animator.SetBool(_animIDJump, false);
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
            }
        }

        public void SetFlyMode(bool fly)
        {
            if (fly)
            {
                FlyMode = true;
            }
            else
            {
                FlyMode = false;
            }
        }

        public void SetSensitivity(float newSensitivity)
        {
            CameraSensitivity = newSensitivity;
        }

        public void ResetCamera()
        {
            _cinemachineTargetYaw = 0;
            _cinemachineTargetPitch = 0;
        }
    }
}