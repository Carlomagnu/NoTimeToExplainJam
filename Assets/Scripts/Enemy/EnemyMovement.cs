using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] private SceneTransition transition;

    public Transform Target;
    public float UpdateSpeed = 0.1f;

    [Header("Line of Sight Settings")]
    [SerializeField] private LayerMask obstacleMask; // Layers that block line of sight
    [SerializeField] private float sightRange = 20f; // Maximum sight distance
    [SerializeField] private Transform eyePosition; // Where the enemy "sees" from (optional)

    [Header("Catch Settings")]
    [SerializeField] private float catchDistance = 2f; // Distance at which player is caught

    [Header("Scene Transition")]
    [SerializeField] private float timeBeforeSceneChange = 60f; // p seconds
    [SerializeField] private string nextSceneName = "NextScene"; // Name of the scene to load

    private FogControl fogControl;
    private NavMeshAgent Agent;
    private Vector3 lastKnownPosition;
    private bool hasLastKnownPosition = false;
    private bool playerCaught = false;

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

        // If no eye position is set, use the enemy's position
        if (eyePosition == null)
        {
            eyePosition = transform;
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
            // Always update last known position if we have line of sight (regardless of fog state)
            if (Target != null)
            {
                bool hasLineOfSight = CheckLineOfSight();

                if (hasLineOfSight)
                {
                    lastKnownPosition = Target.position;
                    hasLastKnownPosition = true;
                    Debug.Log($"[EnemyMovement] Last known position updated: {lastKnownPosition}", this);
                }
            }

            // Check if fog is active before moving
            if (fogControl != null && fogControl.isFogActive)
            {
                if (Target == null)
                {
                    Debug.LogWarning("[EnemyMovement] Target is NULL. Cannot set destination.", this);
                }
                else
                {
                    // Check if player is caught
                    if (!playerCaught)
                    {
                        float distanceToTarget = Vector3.Distance(transform.position, Target.position);
                        if (distanceToTarget <= catchDistance)
                        {
                            OnPlayerCaught();
                            yield break; // Exit the coroutine
                        }
                    }

                    // Check line of sight to target
                    bool hasLineOfSight = CheckLineOfSight();

                    if (hasLineOfSight)
                    {
                        // Can see the player - move to their current position
                        Debug.Log(
                            $"[EnemyMovement] Line of sight CLEAR. Moving to Target position: {Target.position}",
                            this
                        );
                        Agent.SetDestination(Target.position);
                    }
                    else
                    {
                        // Cannot see the player - move to last known position
                        if (hasLastKnownPosition)
                        {
                            Debug.Log(
                                $"[EnemyMovement] Line of sight BLOCKED. Moving to last known position: {lastKnownPosition}",
                                this
                            );
                            Agent.SetDestination(lastKnownPosition);
                        }
                        else
                        {
                            Debug.Log("[EnemyMovement] Line of sight BLOCKED and no last known position.", this);
                            Agent.ResetPath();
                        }
                    }
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

    /// <summary>
    /// Called when the player is caught by the enemy
    /// </summary>
    private void OnPlayerCaught()
    {
        playerCaught = true;
        Debug.Log("[EnemyMovement] Player caught! Loading scene: " + nextSceneName, this);

        // Stop the agent
        Agent.ResetPath();

        // Load the next scene using Transition
        StartCoroutine(LoadSceneAfterDelay());
    }

    /// <summary>
    /// Loads the next scene after a delay using Transition
    /// </summary>
    private IEnumerator LoadSceneAfterDelay()
    {
        yield return new WaitForSeconds(timeBeforeSceneChange);
        transition.changeScene(nextSceneName);
    }

    /// <summary>
    /// Checks if there's a clear line of sight between the enemy and the target.
    /// Returns true if the target is visible, false if blocked by obstacles.
    /// </summary>
    private bool CheckLineOfSight()
    {
        if (Target == null) return false;

        Vector3 directionToTarget = Target.position - eyePosition.position;
        float distanceToTarget = directionToTarget.magnitude;

        // Check if target is within sight range
        if (distanceToTarget > sightRange)
        {
            Debug.Log("[EnemyMovement] Target is out of sight range.", this);
            return false;
        }

        // Raycast to check for obstacles
        RaycastHit hit;
        if (Physics.Raycast(eyePosition.position, directionToTarget.normalized, out hit, distanceToTarget, obstacleMask))
        {
            // Something is blocking the view
            Debug.Log($"[EnemyMovement] Line of sight blocked by: {hit.collider.gameObject.name}", this);
            return false;
        }

        // Clear line of sight
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize sight range
        if (eyePosition != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(eyePosition.position, sightRange);
        }

        // Visualize catch distance
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, catchDistance);

        // Visualize line of sight to target
        if (Target != null && eyePosition != null)
        {
            bool canSee = CheckLineOfSight();
            Gizmos.color = canSee ? Color.green : Color.red;
            Gizmos.DrawLine(eyePosition.position, Target.position);
        }

        // Visualize last known position
        if (hasLastKnownPosition)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(lastKnownPosition, 0.5f);
        }
    }
}