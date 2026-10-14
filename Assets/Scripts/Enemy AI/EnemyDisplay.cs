using UnityEngine;

public class EnemyDisplay : MonoBehaviour
{
    public GameObject North;
    public GameObject East;
    public GameObject South;
    public GameObject West;

    private GameObject Player;

    void Start()
    {
        Player = GameObject.FindGameObjectsWithTag("Player")[0];
    }

    void Update()
    {
        North.SetActive(true);
        East.SetActive(true);
        South.SetActive(true);
        West.SetActive(true);
        Vector3 vectorToPlayer = Player.transform.position - transform.position;
        vectorToPlayer.y = 0;
        float angleToPlayer = Vector3.SignedAngle(transform.forward, vectorToPlayer, Vector3.up);

        if(angleToPlayer <= 45 && angleToPlayer >= -45)
        {
            East.SetActive(false);
            South.SetActive(false);
            West.SetActive(false);
        }
        else if(angleToPlayer > 45 && angleToPlayer < 135)
        {
            North.SetActive(false);
            South.SetActive(false);
            West.SetActive(false);
        }
        else if(angleToPlayer < -45 && angleToPlayer > -135)
        {
            North.SetActive(false);
            East.SetActive(false);
            South.SetActive(false);
        }
        else
        {
            North.SetActive(false);
            East.SetActive(false);
            West.SetActive(false);
        }

        Debug.Log("North: " + North.activeSelf + ", East: " + East.activeSelf + ", South: " + South.activeSelf + ", West: " + West.activeSelf);
    }
}
