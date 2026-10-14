using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public BoxCollider attackCollider;
    GameObject Player;
    
    void Start()
    {
        Player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    void Update()
    {
        Player.GetComponent<PlayerHealth>().TakeDamage(1);
    }
}
