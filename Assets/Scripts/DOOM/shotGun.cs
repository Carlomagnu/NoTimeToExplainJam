using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class shotGun : MonoBehaviour, IInteractable
{

    [Header("Shooting")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] private float range = 50f;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private int pelletCount = 8;
    [SerializeField] private float spreadAngle = 6f;
    [SerializeField] private float fireRate = 0.6f;

    [Header("Impact FX")]
    [SerializeField] private ParticleSystem impactParticles;
    [SerializeField] private float debugCircleRadius = 0.15f;
    private float nextFireTime;

    [Header("CameraShake")]
    [SerializeField] float shakeDuration = 0.2f;
    [SerializeField] float shakeMagnitude = 0.4f;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilKickBack = 0.15f;
    [SerializeField] private float recoilRotationUp = 15f;
    [SerializeField] private float recoilRotationSide = 5f;
    [SerializeField] private float recoilReturnSpeed = 8f;
    [SerializeField] private float recoilSnappiness = 12f;

    [Header("Audio")]
    [SerializeField] AudioSource speaker;
    [SerializeField] AudioClip shoot;

    // Recoil runtime
    private Vector3 targetPosition;
    private Vector3 currentPosition;

    private Vector3 targetRotation;
    private Vector3 currentRotation;

    //Blood
    private bool hasBled;
    [SerializeField] int maxBlood = 2;
    private int activeBlood;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Only do stuff if were holding
        if (transform.parent == null) return;

        if (Input.GetMouseButtonDown(0) &&
            Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
        HandleRecoil();
    }

    public void Interact(PlayerInteract player)
    {
        Debug.Log(this.name + " just got picked up by: " + player.name);
        player.PickUp(gameObject);

        //Assign main camera to shotgun
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
    }

    // MAKES IT NOT DRIFT AWAY WHEN PICKED UP
    void LateUpdate()
    {
        if (transform.parent != null)
        {
            transform.localPosition = currentPosition;
            transform.localRotation = Quaternion.Euler(currentRotation);
        }
    }


    void Fire()
    {
        hasBled = false;
        for (int i = 0; i < pelletCount; i++)
        {
            Vector3 spreadDir =
                Quaternion.Euler(
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle),
                    0f
                ) * cameraTransform.forward;

            Ray ray = new Ray(
                cameraTransform.position,
                spreadDir
            );

            if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers))
            {
                Debug.Log("Hit: " + hit.collider.name);

                SpawnImpact(hit);
                if (!hasBled)
                {
                    spawnBlood(hit);
                    hasBled = true;
                }
                HandlePosterHit(hit);

                DrawDebugCircle(hit.point, hit.normal);
            }
        }

        // Camera shake and immpact
        speaker.PlayOneShot(shoot);
        CameraShake.Instance?.Shake(shakeDuration, shakeMagnitude);
        ApplyRecoil();
    }


    void SpawnImpact(RaycastHit hit)
    {
        if (!impactParticles) return;

        ParticleSystem p =
            Instantiate(
                impactParticles,
                hit.point,
                Quaternion.LookRotation(hit.normal)
            );

        Destroy(p.gameObject, 2f);
    }

    //Circle to see where we are hitting
    void DrawDebugCircle(Vector3 pos, Vector3 normal)
    {
        int segments = 18;
        float angleStep = 360f / segments;

        Vector3 right =
            Vector3.Cross(normal, Vector3.up);

        if (right == Vector3.zero)
            right = Vector3.Cross(normal, Vector3.forward);

        Vector3 up =
            Vector3.Cross(normal, right);

        Vector3 prev =
            pos + right * debugCircleRadius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;

            Vector3 next =
                pos +
                (right * Mathf.Cos(angle) +
                 up * Mathf.Sin(angle)) *
                debugCircleRadius;

            Debug.DrawLine(prev, next, Color.red, 1f);
            prev = next;
        }
    }


    void ApplyRecoil()
    {
        // Kick backwards
        targetPosition -= new Vector3(0, 0, recoilKickBack);

        // Rotate up + random side
        float side = Random.Range(-recoilRotationSide, recoilRotationSide);

        targetRotation += new Vector3(
            -recoilRotationUp,
            side,
            side * 0.5f
        );
    }

    void HandleRecoil()
    {
        if (transform.parent == null) return;

        // Return targets to rest
        targetPosition = Vector3.Lerp(
            targetPosition,
            Vector3.zero,
            recoilReturnSpeed * Time.deltaTime
        );

        targetRotation = Vector3.Lerp(
            targetRotation,
            Vector3.zero,
            recoilReturnSpeed * Time.deltaTime
        );

        // Move current toward target
        currentPosition = Vector3.Lerp(
            currentPosition,
            targetPosition,
            recoilSnappiness * Time.deltaTime
        );

        currentRotation = Vector3.Lerp(
            currentRotation,
            targetRotation,
            recoilSnappiness * Time.deltaTime
        );
    }

    //Spawns blood if the object is bleedable
    void spawnBlood(RaycastHit hit)
    {
        if (activeBlood >= maxBlood)
        {
            return;
        }
        Ibleedable bleedable =
        hit.collider.GetComponentInParent<Ibleedable>();

        
        if (bleedable != null)
        {
            bleedable.bleed(hit);
            activeBlood++;
            StartCoroutine(ReleaseBlood());
        }
    }

    IEnumerator ReleaseBlood()
    {
        yield return new WaitForSeconds(7f);
        activeBlood--;
    }

    // For posters
    void HandlePosterHit(RaycastHit hit)
    {
        IPoster poster =
            hit.collider.GetComponentInParent<IPoster>();

        if (poster != null)
        {
            poster.OnShot(hit);
        }
    }
}
