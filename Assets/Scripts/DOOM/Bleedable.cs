using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleedable : MonoBehaviour, Ibleedable
{
    [Header("Blood tweaking")]
    [SerializeField] ParticleSystem bloodSpurt;
    [SerializeField] int maxBleed = 3;
    private int bleeds;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void bleed(RaycastHit hit)
    {
        if (!bloodSpurt) return;
        if (bleeds >= maxBleed) return;

        Vector3 source = hit.point;
        source = new Vector3(source.x, source.y + 3f, source.z);

        ParticleSystem blood =
            Instantiate(
                bloodSpurt,
                source,
                Quaternion.LookRotation(hit.normal) 
            );
        bleeds += 1;
        Destroy(blood.gameObject, 7f);
        StartCoroutine(ReleaseBlood());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator ReleaseBlood()
    {
        yield return new WaitForSeconds(7f);
        bleeds--;
    }
}
