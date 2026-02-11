using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreLabel : MonoBehaviour
{
    private float timeElapsed = 0f;
    private const float animationTime = 0.1f;

    // Update is called once per frame
    void Update()
    {
        timeElapsed += Time.deltaTime;
        float ratio = timeElapsed / animationTime;

        this.transform.localScale = Vector3.one * Math.Min(1, ratio);

        if(timeElapsed > animationTime + 0.5f)
        {
            Destroy(this.gameObject);
        }
    }
}
