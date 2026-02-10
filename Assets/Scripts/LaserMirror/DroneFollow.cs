using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class DroneFollow : MonoBehaviour
{
    
    [Header("Target")]
    public Transform target;

    [Header("Offsets (local to target)")]
    public float distanceBehind = 1.0f;     // 1 meter away
    public float heightAbove = 0.5f;        // above head level (tune)
    public float sideOffset = 0.4f;         // optional: slightly to the side

    [Header("Follow Smoothing")]
    public float positionSmoothTime = 0.15f;
    public float rotationSmoothSpeed = 8f;

    [Header("Behavior")]
    public bool stayBehindTarget = true;    // if false, stays in world direction offset
    public bool faceTarget = false;         // drone looks at player

    private Vector3 _vel;

    void Start()
{
    if (target == null) return;

    // Snap to the desired position at start so you don't start 500m away.
    Vector3 behindDir = stayBehindTarget ? -target.forward : Vector3.back;
    Vector3 sideDir = stayBehindTarget ? target.right : Vector3.right;

    Vector3 desiredPos =
        target.position +
        behindDir * distanceBehind +
        Vector3.up * heightAbove +
        sideDir * sideOffset;

    transform.position = desiredPos;
}
    void Reset()
    {
        positionSmoothTime = 0.15f;
        rotationSmoothSpeed = 8f;
        distanceBehind = 1.0f;
        heightAbove = 0.5f;
        sideOffset = 0.4f;
        stayBehindTarget = true;
        faceTarget = false;
    }

    void LateUpdate()
    {
        
        if (target == null) return;

        // Compute desired offset relative to target
        Vector3 behindDir = stayBehindTarget ? -target.forward : Vector3.back;
        Vector3 sideDir = stayBehindTarget ? target.right : Vector3.right;

        Vector3 desiredPos =
            target.position +
            behindDir * distanceBehind +
            Vector3.up * heightAbove +
            sideDir * sideOffset;

        // Smooth position
        transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref _vel, positionSmoothTime);

        // Smooth rotation (optional)
        if (faceTarget)
        {
            Vector3 toTarget = (target.position + Vector3.up * heightAbove) - transform.position;
            if (toTarget.sqrMagnitude > 0.0001f)
            {
                Quaternion desiredRot = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * rotationSmoothSpeed);
            }
        }
        else
        {
            // Otherwise drift toward matching player's facing (optional “companion” feel)
            Quaternion desiredRot = Quaternion.LookRotation(target.forward, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * rotationSmoothSpeed);
        }
    }
}