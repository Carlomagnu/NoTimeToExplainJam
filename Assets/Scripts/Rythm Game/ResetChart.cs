using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetChart : MonoBehaviour
{
    [SerializeField]
    MusicProgressCalculator musicProgressCalculator;
    [SerializeField]
    GameObject chartInputs;

    private float resetCounter = 4f;

    void Update()
    {
        if(musicProgressCalculator.getBarProgress() - 1 > resetCounter)
        {
            for(int i = 0; i < chartInputs.transform.childCount; i++)
            {
                Destroy(chartInputs.transform.GetChild(i).gameObject);
            }
            resetCounter += 4f;
        }
    }
}
