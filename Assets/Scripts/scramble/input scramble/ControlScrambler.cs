using UnityEngine;
using UnityEngine.InputSystem;

public class ControlScrambler : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private float _damageThreshold = 30f;

    [Header("Scramble Settings")]
    [SerializeField] private bool _permanent = false;
    [SerializeField] private float _scrambleDuration = 8f;
    [SerializeField] private bool _chaosMode = false;

    private float _damageTaken;
    private bool _swapAxes;
    private bool _invertX;
    private bool _invertY;

    // chaos mode: direction for each key in the order W, D, S, A
    private Vector2[] _keyDirections = { Vector2.up, Vector2.right, Vector2.down, Vector2.left };

    public bool IsScrambled { get; private set; }

    // called by the health code when the player gets hit
    public void AddDamage(float amount)
    {
        _damageTaken += amount;

        if (_damageTaken >= _damageThreshold)
        {
            _damageTaken = 0f;
            Scramble();
        }
    }

    [ContextMenu("Scramble Now")]
    public void Scramble()
    {
        if (_chaosMode)
        {
            // shuffle the directions between the keys, again if it lands on normal controls
            do
            {
                for (int i = 0; i < _keyDirections.Length; i++)
                {
                    int r = Random.Range(i, _keyDirections.Length);
                    Vector2 temp = _keyDirections[i];
                    _keyDirections[i] = _keyDirections[r];
                    _keyDirections[r] = temp;
                }
            }
            while (_keyDirections[0] == Vector2.up && _keyDirections[1] == Vector2.right && _keyDirections[2] == Vector2.down);
        }
        else
        {
            // keep rolling until at least one thing changes, otherwise the controls would stay normal
            do
            {
                _swapAxes = Random.value > 0.5f;
                _invertX = Random.value > 0.5f;
                _invertY = Random.value > 0.5f;
            }
            while (!_swapAxes && !_invertX && !_invertY);
        }

        IsScrambled = true;
        Debug.Log($"Controls scrambled! swap: {_swapAxes}, invertX: {_invertX}, invertY: {_invertY}");

        CancelInvoke(nameof(ResetControls));
        if (!_permanent) Invoke(nameof(ResetControls), _scrambleDuration);
    }
    // TEMP: press T to test scrambling until health is in
private void Update()
{
    if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
    {
        Scramble();
    }
}
    public void ResetControls()
    {
        _swapAxes = false;
        _invertX = false;
        _invertY = false;
        _keyDirections = new Vector2[] { Vector2.up, Vector2.right, Vector2.down, Vector2.left };
        IsScrambled = false;
    }

    // PlayerMove passes its input through this before using it
    public Vector2 ApplyScramble(Vector2 input)
    {
        if (!IsScrambled) return input;

        if (_chaosMode)
        {
            // add up the new direction of each key being held
            Vector2 result = Vector2.zero;
            if (input.y > 0) result += _keyDirections[0] * input.y;   // W
            if (input.x > 0) result += _keyDirections[1] * input.x;   // D
            if (input.y < 0) result += _keyDirections[2] * -input.y;  // S
            if (input.x < 0) result += _keyDirections[3] * -input.x;  // A
            return result;
        }

        if (_swapAxes) input = new Vector2(input.y, input.x);
        if (_invertX) input.x = -input.x;
        if (_invertY) input.y = -input.y;

        return input;
    }
}
