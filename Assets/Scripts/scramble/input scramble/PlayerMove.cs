using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]

public class PlayerMove : MonoBehaviour
{
// Movement settings
    [SerializeField] private MovementSettings _settings;
    [SerializeField] private InputActionReference _moveAction;
    [SerializeField] private Transform _camera;
    [SerializeField] private ControlScrambler _scrambler;

    private const float GroundedPush = -2f;

    private CharacterController _controller;
    //horizontal velocity based on player input and speed setting
    private Vector3 _horizontalVelocity;
    //gravity and vertical velocity
    private float _verticalVelocity;
    //is the player jumping or falling
    private bool _isLaunched;

    public MovementSettings Settings => _settings;
    //is the player grounded
    public bool IsGrounded => _controller.isGrounded;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        if (_camera == null) _camera = Camera.main.transform;
        if (_scrambler == null) _scrambler = GetComponent<ControlScrambler>();
    }

    private void OnEnable() => _moveAction.action.Enable();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isLaunched) UpdateWalking();
        UpdateGravity();
        UpdateFacing();

        _controller.Move((_horizontalVelocity + Vector3.up * _verticalVelocity) * Time.deltaTime);

        if (_isLaunched && _controller.isGrounded) _isLaunched = false;
         
    }

// Update the horizontal velocity based on player input and movement settings
    private void UpdateWalking()
    {
        Vector2 input = _moveAction.action.ReadValue<Vector2>();
        if (_scrambler != null) input = _scrambler.ApplyScramble(input);
        Vector3 targetVelocity = CameraRelative(input) * _settings.MoveSpeed;

        _horizontalVelocity = Vector3.MoveTowards(_horizontalVelocity, targetVelocity, _settings.Acceleration * Time.deltaTime);
    }

    private void UpdateGravity()
    {
        if (_controller.isGrounded && _verticalVelocity < 0f)
        {
            // A small constant push keeps isGrounded reliable on stairs and slopes
            _verticalVelocity = GroundedPush;
            return;
        }

        _verticalVelocity = Mathf.Max(_verticalVelocity + _settings.Gravity * Time.deltaTime, _settings.TerminalVelocity);
    }

    private void UpdateFacing()
    {
        if (_horizontalVelocity.sqrMagnitude < 0.01f) return;

        Quaternion target = Quaternion.LookRotation(_horizontalVelocity);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, target, _settings.TurnSpeed * Time.deltaTime);
    }

    private Vector3 CameraRelative(Vector2 input)
    {
        Vector3 forward = Vector3.ProjectOnPlane(_camera.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(_camera.right, Vector3.up).normalized;
        return forward * input.y + right * input.x;
    }

        public void Launch(Vector3 horizontalVelocity, float verticalVelocity)
    {
        _horizontalVelocity = horizontalVelocity;
        _verticalVelocity = verticalVelocity;
        _isLaunched = true;
    }



}
