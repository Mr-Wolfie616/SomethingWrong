using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [SerializeField] AudioSource detectSound;
    private AudioManager audioManager;
    private EnemyStates CurrentState = EnemyStates.Wait;
    private float StateCountdown = 0f;
    private List<GameObject> enemyNodes;
    private List<GameObject> tiredEnemyNodes;
    private float SPEED = 7f;
    private GameObject Player;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyNodes = new List<GameObject>();
        tiredEnemyNodes = new List<GameObject>();
        Player = GameObject.FindGameObjectsWithTag("Player")[0];
        audioManager = FindFirstObjectByType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Find State & Enemy Nodes
        StateCountdown -= Time.deltaTime;
        if(StateCountdown <= 0)
        {
            StateCountdown = Random.Range(5f, 8f);
            if(CurrentState == EnemyStates.Wait) CurrentState = EnemyStates.Patrol;
            else if(CurrentState == EnemyStates.Patrol) CurrentState = EnemyStates.Wait;
            enemyNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyNode"));
        }

        // Find nearest node
        float nearestNodeDistance = Mathf.Infinity;
        GameObject nearestNode = null;
        foreach(GameObject node in enemyNodes)
        {
            float distance = (transform.position - node.transform.position).magnitude;
            if(distance < nearestNodeDistance && !tiredEnemyNodes.Contains(node))
            {
                nearestNodeDistance = distance;
                nearestNode = node;
            }
        }

        // Find player
        Ray PlayerRay = new Ray(transform.position, Player.transform.position - transform.position);
        Debug.DrawRay(PlayerRay.origin, PlayerRay.direction * 100f, Color.green);
        if(Physics.Raycast(PlayerRay, out RaycastHit hit, 100f))
        {
            if(hit.collider.gameObject == Player) CurrentState = EnemyStates.Chase;
            else if(CurrentState == EnemyStates.Chase) CurrentState = EnemyStates.Wait;
            audioManager.PlayerDetected(true);
            detectSound.Play();
        }

        // Move towards
        if(CurrentState == EnemyStates.Patrol && nearestNode != null)
        {
            if(nearestNodeDistance < 1f)
            {
                tiredEnemyNodes.Add(nearestNode);
                if(tiredEnemyNodes.Count > 3)
                {
                    tiredEnemyNodes.RemoveAt(0);
                }
            }
            else
            {
                Movement(nearestNode.transform);
            }
        }
        else if(CurrentState == EnemyStates.Chase)
        {
            Movement(Player.transform);
        }
    }

    void Movement(Transform target)
    {
        Ray CollisionRay = new Ray(transform.position, transform.forward);
        Debug.DrawRay(CollisionRay.origin, CollisionRay.direction * 10f, Color.red);
        if(Physics.Raycast(CollisionRay, out RaycastHit hit, 2f))
        {
            float xConst = 0;
            float zConst = 0;
            if((target.position - transform.position).x > 0) xConst = 1;
            else if((target.position - transform.position).x < 0) xConst = -1;
            if((target.position - transform.position).z > 0) zConst = 1;
            else if((target.position - transform.position).z < 0) zConst = -1;
            Vector3 movement = new Vector3(1 * xConst, 0, 1 * zConst);
            transform.position += movement * SPEED * Time.deltaTime;
        }
        else
        {
            // Move towards target
            transform.position += transform.forward * SPEED * Time.deltaTime;
            float targetAngle = Vector3.Angle(target.position - transform.position, transform.forward);
            Debug.Log(targetAngle);
            //if(targetAngle > 180) targetAngle = targetAngle - 180;
            if(transform.rotation.eulerAngles.y + targetAngle + targetAngle > transform.rotation.eulerAngles.y + targetAngle)
            targetAngle = -targetAngle;
            {
                transform.rotation = Quaternion.Euler
                (
                    transform.rotation.x, 
                    Mathf.Lerp(transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.y + targetAngle, 0.1f), 
                    transform.rotation.z
                );
            }
        }
    }
}