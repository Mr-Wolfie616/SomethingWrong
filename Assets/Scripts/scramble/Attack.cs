using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Header("Reference")]
    public InputActionReference input;
    public Camera playerCam;
    public float Damage = 15f;
    
    public RaycastHit rayHit;

    private void Awake()
    {
        input = FindAnyObjectByType<InputActionReference>();
    }

    private void Update()
    {
        Input();
    }

    private void Input()
    {
        if(input.Attack)
        {
            TryHit();
        }
    }

    public void TryHit()
    {
       StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        Hit();
        yield return new WaitForSeconds(1);
        yield return new WaitForSeconds(3);
    }

    private void Hit()
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out rayHit, 5f))
        {
            rayHit.collider.TryGetComponent<IDamageable>(out var damageable);
            damageable?.TakeDamage(Damage);
        }
    }
}