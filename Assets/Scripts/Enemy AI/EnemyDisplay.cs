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
        Debug.Log("test0");
        Vector3 vectorToPlayer = Player.transform.position - transform.position;
        float angleToPlayer = Vector3.Angle(vectorToPlayer, transform.forward);
        if(Vector3.Cross(vectorToPlayer, transform.forward).y < 0) angleToPlayer = -angleToPlayer;
        Debug.Log("test1");

        if(angleToPlayer >= 45 || angleToPlayer <= -45)
        {
            North.SetActive(true);

            East.SetActive(false);
            South.SetActive(false);
            West.SetActive(false);
        }
        else if(angleToPlayer < 45 && angleToPlayer > 135)
        {
            East.SetActive(true);

            North.SetActive(false);
            South.SetActive(false);
            West.SetActive(false);
        }
        else if(angleToPlayer < -45 && angleToPlayer > -135)
        {
            West.SetActive(true);

            North.SetActive(false);
            East.SetActive(false);
            West.SetActive(false);
        }
        else
        {
            South.SetActive(true);

            North.SetActive(false);
            East.SetActive(false);
            South.SetActive(false);
        }

        Debug.Log("test2");
        Debug.Log("North: " + North.activeSelf + ", East: " + East.activeSelf + ", South: " + South.activeSelf + ", West: " + West.activeSelf);
    }
}
