using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 16;
    [SerializeField] AudioSource deathSound;
    public int currentHealth;
    private ControlScrambler _scrambler;
    private MainMenuManager _menuManager;
    private void Awake()                              
    {                                                 
        _scrambler = GetComponent<ControlScrambler>();   
        _menuManager = FindFirstObjectByType<MainMenuManager>();
    } 
    
    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (currentHealth == 0)
        {
            Die();
        }
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
        deathSound.Play();
        _menuManager.Death();
    }
}
