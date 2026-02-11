using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChartCurrentPosition : MonoBehaviour
{
    [SerializeField]
    MusicProgressCalculator musicProgressCalculator;
    [SerializeField]
    float startPosition;
    [SerializeField]
    float endPosition;

    void Update()
    {
        this.GetComponent<RectTransform>().localPosition = new Vector3(0,startPosition + ((endPosition - startPosition) * calculateProgress()), 0);
    }


    float calculateProgress()
    {
        float barProgress = (musicProgressCalculator.getBarProgress() - 1) % 4;
        return barProgress / 4;
    }
}
