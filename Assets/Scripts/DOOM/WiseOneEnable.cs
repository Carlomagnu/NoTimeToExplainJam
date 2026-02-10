using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class WiseOneEnable : MonoBehaviour
{

    [SerializeField] GameObject wiseOne;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(enableBro());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator enableBro()
    {
        yield return new WaitForSeconds(0.2f);
        wiseOne.SetActive(true);
    }
}
