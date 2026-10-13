using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private ControlScrambler _scrambler;

    private void Awake()                              
    {                                                 
        _scrambler = GetComponent<ControlScrambler>();   
    } 
    
    
    
    
    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (_scrambler != null) _scrambler.AddDamage(dmg);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // death / fail logic here
    }
}
