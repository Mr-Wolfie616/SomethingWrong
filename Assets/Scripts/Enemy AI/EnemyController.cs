using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    private EnemyStates CurrentState = EnemyStates.Wait;
    private float StateCountdown = 0f;
    private List<GameObject> enemyNodes;
    private List<GameObject> tiredEnemyNodes;
    private float SPEED = 7f;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Find State & Enemy Nodes
        StateCountdown -= Time.deltaTime;
        Debug.Log(StateCountdown);
        if(StateCountdown <= 0)
        {
            StateCountdown = Random.Range(5f, 8f);
            if(CurrentState == EnemyStates.Wait) CurrentState = EnemyStates.Patrol;
            else if(CurrentState == EnemyStates.Patrol) CurrentState = EnemyStates.Wait;
            enemyNodes = new List<GameObject>(GameObject.FindGameObjectsWithTag("EnemyNode"));
            tiredEnemyNodes = new List<GameObject>();
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

        // Move towards
        if(CurrentState == EnemyStates.Patrol && nearestNode != null)
        {
            if(nearestNodeDistance < 1f)
            {
                //CurrentState = EnemyStates.Wait;
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
        else if(CurrentState == EnemyStates.Chase && nearestNode != null)
        {
            // Chase player
        }
    }

    void Movement(Transform target)
    {
        Ray CollisionRay = new Ray(transform.position, transform.forward);
        if(Physics.Raycast(CollisionRay, out RaycastHit hit, 1f))
        {
            float xConst = 1;
            float zConst = 1;
            if(target.position.x < transform.position.x) xConst = -1;
            if(target.position.z < transform.position.z) zConst = -1;
            new Vector3(hit.normal.z * xConst, 0, hit.normal.x * zConst);
            transform.position += transform.forward * SPEED * Time.deltaTime;
        }
        else
        {
            // Move towards target
            transform.position += transform.forward * SPEED * Time.deltaTime;
            float targetAngle = Vector3.Angle(target.position - transform.position, transform.forward); 
            transform.rotation = Quaternion.Euler
            (
                transform.rotation.x, 
                Mathf.Lerp(transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.y +targetAngle, 0.1f), 
                transform.rotation.z
            );
        }
    }
}
