using UnityEngine;

public class EnemyDisplay : MonoBehaviour
{
    private GameObject Player;
    private GameObject North;
    private GameObject East;
    private GameObject South;
    private GameObject West;
    void Start()
    {
        Player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    void Update()
    {
        Vector3 vectorToPlayer = Player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(vectorToPlayer, transform.forward);
        if(Vector3.Cross(vectorToPlayer, transform.forward).y < 0) angleToPlayer = -angleToPlayer;

        if(angleToPlayer >= 45 || angleToPlayer <= -45)
        {
            North.SetActive(true);
        }
        else if(angleToPlayer < 45 && angleToPlayer > 135)
        {
            East.SetActive(true);
        }
        else if(angleToPlayer < -45 && angleToPlayer > -135)
        {
            South.SetActive(true);
        }
        else
        {
            West.SetActive(true);
        }
    }
}
