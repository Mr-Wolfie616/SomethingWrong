using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;   

public class Attack : MonoBehaviour
{
    [Header("Reference")]
    public InputActionReference input;
    public float Damage = 15f;
    public float Range = 1.5f;     
    public float HitRadius = 1f;   

    private bool canAttack = true;  

    private void Awake()
    {
        input.action.Enable();   
    }

    private void Update()
    {
        Input();
    }

    private void Input()
    {
        if (canAttack && input.action.WasPressedThisFrame())   
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
        canAttack = false;  
        Hit();
        yield return new WaitForSeconds(1);
        canAttack = true;    
    }

    private void Hit()
    {
        
        Vector3 hitPoint = transform.position + Vector3.up + transform.forward * Range;
        Collider[] hits = Physics.OverlapSphere(hitPoint, HitRadius);

        foreach (Collider hit in hits)
        {
            Enemy_Damage enemy = hit.GetComponent<Enemy_Damage>();
            if (enemy != null) enemy.TakeDamage(Damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up + transform.forward * Range, HitRadius);
    }
}