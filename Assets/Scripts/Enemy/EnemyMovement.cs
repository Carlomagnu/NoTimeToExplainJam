using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    public Transform Target;
    public float UpdateSpeed = 0.1f;
    private FogControl fogControl;
    private NavMeshAgent Agent;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Debug.Log("[EnemyMovement] Awake called. NavMeshAgent acquired.", this);

        // Find the FogControl component in the scene
        fogControl = FindObjectOfType<FogControl>();
        if (fogControl == null)
        {
            Debug.LogError("[EnemyMovement] FogControl not found in scene!", this);
        }
        else
        {
            Debug.Log("[EnemyMovement] FogControl found and assigned.", this);
        }
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
            // Check if fog is active before moving
            if (fogControl != null && fogControl.isFogActive)
            {
                if (Target == null)
                {
                    Debug.LogWarning("[EnemyMovement] Target is NULL. Cannot set destination.", this);
                }
                else
                {
                    Debug.Log(
                        $"[EnemyMovement] Fog is active. Setting destination to Target position: {Target.position}",
                        this
                    );
                    Agent.SetDestination(Target.transform.position);
                }
            }
            else
            {
                // Fog is not active, stop the agent
                if (fogControl == null)
                {
                    Debug.LogWarning("[EnemyMovement] FogControl is NULL. Cannot check fog state.", this);
                }
                else
                {
                    Debug.Log("[EnemyMovement] Fog is NOT active. Enemy stopped.", this);
                }

                // Stop the NavMeshAgent
                Agent.ResetPath();
            }

            yield return Wait;
        }

        Debug.Log("[EnemyMovement] FollowTarget coroutine stopped (component disabled).", this);
    }
}