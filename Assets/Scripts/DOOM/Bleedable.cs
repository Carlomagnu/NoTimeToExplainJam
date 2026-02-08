using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleedable : MonoBehaviour, Ibleedable
{
    [Header("Blood tweaking")]
    [SerializeField] ParticleSystem bloodSpurt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void bleed(RaycastHit hit)
    {
        if (!bloodSpurt) return;

        Vector3 source = hit.point;
        source = new Vector3(source.x, source.y + 3f, source.z);

        ParticleSystem blood =
            Instantiate(
                bloodSpurt,
                source,
                Quaternion.LookRotation(hit.normal)
            );
        blood.transform.SetParent(hit.collider.transform);
        Destroy(blood.gameObject, 7f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
