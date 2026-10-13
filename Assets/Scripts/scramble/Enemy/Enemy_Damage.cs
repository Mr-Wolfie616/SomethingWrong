using UnityEngine;

public class Enemy_Damage : MonoBehaviour, IDamageable
{
    [SerializeField] private float Mhealth = 25f;

    public bool IsDead;

    public void TakeDamage(float damage)
    {
        if(IsDead) return;

        Mhealth -= damage;

        if(Mhealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        IsDead = true;
    }
}