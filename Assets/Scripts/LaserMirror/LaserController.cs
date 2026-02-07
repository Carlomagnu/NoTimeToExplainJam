using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRenderer;

    [Header("Laser Settings")]
    public float maxDistance = 50f;
    public int maxBounces = 10;
    public LayerMask laserMask = ~0;

    // Receivers we hit last frame vs this frame
    private readonly HashSet<ILaserReceiver> _receiversLastFrame = new HashSet<ILaserReceiver>();
    private readonly HashSet<ILaserReceiver> _receiversThisFrame = new HashSet<ILaserReceiver>();

    void Update()
    {
        if (lineRenderer == null) return;

        _receiversThisFrame.Clear();

        Vector3 origin = transform.position;
        Vector3 dir = transform.forward;

        List<Vector3> points = new List<Vector3> { origin };

        for (int bounce = 0; bounce < maxBounces; bounce++)
        {
            if (Physics.Raycast(origin, dir, out RaycastHit hit, maxDistance, laserMask))
            {
                points.Add(hit.point);

                // ---- G6 CORE: detect receivers on the hit object ----
                ILaserReceiver receiver = hit.collider.GetComponentInParent<ILaserReceiver>();
                if (receiver != null)
                {
                    _receiversThisFrame.Add(receiver);
                }
                // -----------------------------------------------------

                // Reflect if mirror
                if (hit.collider.GetComponentInParent<MirrorReflector>() != null)
                {
                    dir = Vector3.Reflect(dir, hit.normal);
                    origin = hit.point + dir * 0.02f; // nudge avoids self-hit flicker
                    continue;
                }

                // Stop on non-mirror object
                break;
            }
            else
            {
                points.Add(origin + dir * maxDistance);
                break;
            }
        }

        // Draw the beam
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());

        // ---- G6 CORE: turn OFF receivers no longer hit ----
        foreach (var r in _receiversLastFrame)
        {
            if (!_receiversThisFrame.Contains(r))
                r.SetLaserActive(false);
        }

        // ---- G6 CORE: turn ON receivers currently hit ----
        foreach (var r in _receiversThisFrame)
        {
            // You can call true every frame; it's fine.
            // But we can avoid spam by only calling when newly hit:
            if (!_receiversLastFrame.Contains(r))
                r.SetLaserActive(true);
            else
                r.SetLaserActive(true);
        }

        // Prepare for next frame
        _receiversLastFrame.Clear();
        foreach (var r in _receiversThisFrame)
            _receiversLastFrame.Add(r);
    }
}

