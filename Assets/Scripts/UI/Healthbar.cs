using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [Header("Scripts")]
    [SerializeField] PlayerHealth playerHealth;
    [Header("UI Elements")]
    [SerializeField] Image healthbar;
    [SerializeField] Sprite[] healthbarSprites;
    int health;
    int index = 16;

    private void Update()
    {
        health = playerHealth.currentHealth;
        index = Mathf.FloorToInt(health);
        if (index > 16)
        {
            index = 16;
        }
        healthbar.sprite = healthbarSprites[index];
    }
}