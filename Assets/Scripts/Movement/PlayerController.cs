using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController), typeof(Animator), typeof(PlayerControllerInput))]
public class PlayerController : MonoBehaviour
{
	[FormerlySerializedAs("MoveSpeed")]
	[Header("Player")]
    [Tooltip("Move speed of the character in m/s")]
    public float moveSpeed = 4.0f;
    [FormerlySerializedAs("SprintSpeed")] [Tooltip("Sprint speed of the character in m/s")]
    public float sprintSpeed = 6.0f;
	[FormerlySerializedAs("MaxSprintTime")] [Tooltip("The maximum amount of time that the character can sprint without resting in seconds")]
    public float maxSprintTime = 5f;
    [FormerlySerializedAs("RotationSpeed")] [Tooltip("Rotation speed of the character")]
    public float rotationSpeed = 1.0f;
    [FormerlySerializedAs("SpeedChangeRate")] [Tooltip("Acceleration and deceleration")]
    public float speedChangeRate = 10.0f;
    [FormerlySerializedAs("AirborneMoveStrength")] [Tooltip("How strongly does the input affect player airborne movement?"), Range(0, 1)]
    public float airborneMoveStrength = 0.6f;

    [FormerlySerializedAs("JumpHeight")]
    [Space(10)]
    [Tooltip("The height the player can jump")]
    public float jumpHeight = 1.2f;
    [FormerlySerializedAs("Gravity")] [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float gravity = -15.0f;

    [FormerlySerializedAs("JumpTimeout")]
    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    public float jumpTimeout = 0.1f;
    [FormerlySerializedAs("FallTimeout")] [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float fallTimeout = 0.15f;

    [FormerlySerializedAs("Grounded")]
    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool grounded = true;
    [FormerlySerializedAs("GroundedOffset")] [Tooltip("Useful for rough ground")]
    public float groundedOffset = -0.14f;
    [FormerlySerializedAs("GroundedRadius")] [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float groundedRadius = 0.5f;
    [FormerlySerializedAs("GroundLayers")] [Tooltip("What layers the character uses as ground")]
    public LayerMask groundLayers;

    [FormerlySerializedAs("CinemachineCameraTarget")]
    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    public GameObject cinemachineCameraTarget;
    [FormerlySerializedAs("TopClamp")] [Tooltip("How far in degrees can you move the camera up")]
    public float topClamp = 90.0f;
    [FormerlySerializedAs("BottomClamp")] [Tooltip("How far in degrees can you move the camera down")]
    public float bottomClamp = -90.0f;

    // cinemachine
    private float _cinemachineTargetPitch;
    
    // player
    private float _speed;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;
    private float _availableSprint;

    private float _airborneSpeed;
    private Vector3 _airborneMove;

    // Feather info
    private bool _isFeathered;
    private int _maxJumps;
    private int _availableJumps;
    private Coroutine _featherRoutine;
    private float _originalAirborneMoveStrength;
    
    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    
    private const float LookThreshold = 0.01f;

    private PlayerControllerInput input;
    private CharacterController controller;
    private Animator animator;
    private static readonly int AnimatorMoveSpeed = Animator.StringToHash("MoveSpeed");
    private static readonly int AnimatorDeathTrigger = Animator.StringToHash("Death");

    private bool _touchingObelisk;
    
    private void Awake()
    {
        input = GetComponent<PlayerControllerInput>();
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        _availableSprint = maxSprintTime;
        GameManager.GetHUD().SetStaminaSliderPercent(1f);

        _availableJumps = 1;
        _maxJumps = 1;
        _originalAirborneMoveStrength = airborneMoveStrength;
        _touchingObelisk = false;

        EventManager.OnPlayerDeath += OnDeath;
    }

    private void OnDeath()
    {
	    animator.SetTrigger(AnimatorDeathTrigger);
	    input.enabled = false;
	    enabled = false;
	    
	    EventManager.OnPlayerDeath -= OnDeath;
    }

    public void IncreaseMoveSpeed(float multiplier)
    {
	    moveSpeed *= multiplier;
	    sprintSpeed *= multiplier;
    }
    
    public bool CanInteract()
    {
	    return _touchingObelisk;
    }
    
    private void Update()
	{
		if (PauseMenu.IsPaused) return;
		JumpAndGravity();
		GroundedCheck();
		CheckInteractions();
		Move();
	}

	private void LateUpdate()
	{
		if (PauseMenu.IsPaused) return;
		CameraRotation();
	}

	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
		grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
	}

	private void CameraRotation()
	{
		// if there is an input
		if (input.look.sqrMagnitude >= LookThreshold)
		{
			//Don't multiply mouse input by Time.deltaTime
			float deltaTimeMultiplier = input.IsCurrentDeviceMouse() ? 1.0f : Time.deltaTime;
			
			_cinemachineTargetPitch += input.look.y * rotationSpeed * deltaTimeMultiplier;
			_rotationVelocity = input.look.x * rotationSpeed * deltaTimeMultiplier;

			// clamp our pitch rotation
			_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, bottomClamp, topClamp);

			// Update Cinemachine camera target pitch
			cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

			// rotate the player left and right
			transform.Rotate(Vector3.up * _rotationVelocity);
		}
	}

	private void Move()
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = grounded ? (input.sprint ? sprintSpeed : moveSpeed) : _airborneSpeed;

		Vector2 targetMove;
			
		if (grounded) targetMove = input.move;
		else
		{
			// world move direction needs to be put in local direction
			Vector3 localAirborneMove = transform.InverseTransformDirection(_airborneMove);

			targetMove = new Vector2(localAirborneMove.x, localAirborneMove.z);
			
			// The actual target move is somewhere between the airborne move and the input move, depending on the AirborneMoveStrength
			// An AirborneMoveStrength of 0 means that the input is ignored, while an AirborneMoveStrength of 1 means only the input is read
			targetMove = Vector2.Lerp(targetMove, input.move, airborneMoveStrength);
		}

		// Update available sprint time
		if (input.sprint) _availableSprint -= Time.deltaTime;
		else
		{
			_availableSprint += Time.deltaTime;

			if (_availableSprint > maxSprintTime) _availableSprint = maxSprintTime;
		}

		if (_availableSprint <= 0)
		{
			input.StopSprint();
			_availableSprint = 0;
		}
		
		GameManager.GetHUD().SetStaminaSliderPercent(_availableSprint / maxSprintTime);

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		if (targetMove == Vector2.zero) targetSpeed = 0.0f;

		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;

		const float speedOffset = 0.1f;
		float inputMagnitude = input.analogMovement ? targetMove.magnitude : 1f;
		
		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		// normalise input direction
		Vector3 inputDirection = new Vector3(targetMove.x, 0.0f, targetMove.y).normalized;

		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (targetMove != Vector2.zero)
		{
			// move
			inputDirection = transform.right * targetMove.x + transform.forward * targetMove.y;
		}
		
		// animator.SetBool(AnimatorIsMoving, targetMove != Vector2.zero);
		animator.SetFloat(AnimatorMoveSpeed, targetSpeed);


		// move the player
		controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
	}

	private void JumpAndGravity()
	{
		if (grounded)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = fallTimeout;
			
			// Reset number of jumps available
			if (_isFeathered) _availableJumps = _maxJumps;

			// stop our velocity dropping infinitely when grounded
			if (_verticalVelocity < 0.0f)
			{
				_verticalVelocity = -2f;
			}

			// Jump 
			
			if (input.jump && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
				
				// Set the airborne movement and speed to current movement and speed
				_airborneSpeed = input.sprint ? sprintSpeed : moveSpeed;
				_airborneMove = transform.TransformDirection(input.move.x, 0, input.move.y);

				_availableJumps--;
				input.jump = false;
			} 

			// jump timeout
			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			_jumpTimeoutDelta = jumpTimeout;

			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}

			// if we are not grounded, do not jump - this prevents holding the jump button to infinitely jump
			if (input.jump && _availableJumps > 0)
			{
				_availableJumps--;
				
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				_verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
				
				// Set the airborne movement and speed to current movement and speed
				_airborneSpeed = input.sprint ? sprintSpeed : moveSpeed;
				_airborneMove = transform.TransformDirection(input.move.x, 0, input.move.y);

				input.jump = false;
				
				print("Jumping in air! " + _availableJumps + " are still left!");
			}
		}

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		if (_verticalVelocity < _terminalVelocity)
		{
			_verticalVelocity += gravity * Time.deltaTime;
		}
	}

	private void CheckInteractions()
	{
		if (input.interact)
		{
			if (_touchingObelisk)
			{
				print("Interact with Obelisk!");
				GameManager.OpenUpgradeMenu();
			}
			input.interact = false;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Obelisk"))
		{
			_touchingObelisk = true;
			GameManager.SetNotificationVisibility(true);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Obelisk"))
		{
			_touchingObelisk = false;
			GameManager.SetNotificationVisibility(false);
		}
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

	public void ApplyFeather(float duration, int maxJumps)
	{
		if (_featherRoutine != null) StopCoroutine(_featherRoutine);

		_maxJumps = maxJumps;
		_availableJumps = maxJumps;
		_featherRoutine = StartCoroutine(FeatherRoutine(duration));
	}
	
	private IEnumerator FeatherRoutine(float duration)
	{
		airborneMoveStrength = 1;
		_isFeathered = true;

		yield return new WaitForSeconds(duration);

		_isFeathered = false;
		airborneMoveStrength = _originalAirborneMoveStrength;
	}

	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
	}
}