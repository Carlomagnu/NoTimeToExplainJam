using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; 

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform Target; 
    public float UpdateSpeed = 0.1f; 
    private NavMeshAgent Agent; 

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Debug.Log("[EnemyMovement] Awake called. NavMeshAgent acquired.", this);
    }

    private void Start()
    {
        Debug.Log("[EnemyMovement] Start called. Starting FollowTarget coroutine.", this);
        StartCoroutine(FollowTarget());
    }

    private IEnumerator FollowTarget()
    {
        Debug.Log("[EnemyMovement] FollowTarget coroutine started.", this);

        WaitForSeconds Wait = new WaitForSeconds(UpdateSpeed);

        while (enabled)
        {
            if (Target == null)
            {
                Debug.LogWarning("[EnemyMovement] Target is NULL. Cannot set destination.", this);
            }
            else
            {
                Debug.Log(
                    $"[EnemyMovement] Setting destination to Target position: {Target.position}",
                    this
                );
                Agent.SetDestination(Target.transform.position);
            }

            yield return Wait;
        }

        Debug.Log("[EnemyMovement] FollowTarget coroutine stopped (component disabled).", this);
    }
}
