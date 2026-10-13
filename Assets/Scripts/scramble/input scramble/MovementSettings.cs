using UnityEngine;
[CreateAssetMenu(menuName = "Source Code/Movement Settings")]
public class MovementSettings : ScriptableObject
{

    [field: Header("Walking")]
    [field: SerializeField] public float MoveSpeed { get; private set; } = 6f;
    [field: SerializeField] public float Acceleration { get; private set; } = 60f;
    [field: SerializeField] public float TurnSpeed { get; private set; } = 720f;

    [field: Header("Falling")]
    [field: SerializeField] public float Gravity { get; private set; } = -30f;
    [field: SerializeField] public float TerminalVelocity { get; private set; } = -20f;

    [field: Header("Jump")]
    [field: SerializeField] public float JumpDistance { get; private set; } = 2.5f;
    [field: SerializeField] public float JumpHeight { get; private set; } = 0.6f;


    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
