using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AudioVisualiser : MonoBehaviour
{
    [SerializeField]
    AudioSource microphoneAudio;
    [SerializeField]
    GameObject[] bars;

    private const int sampleSize = 512;
    private float[] spectrumData = new float[sampleSize];
    private const float heightMultiplier = 5000f;

    void Update()
    {
        analyseData();
        updateBars();
    }

    void analyseData()
    {
        microphoneAudio.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);
    }

    void updateBars()
    {
        int segmentSize = sampleSize / bars.Length;

        for(int i = 0; i < bars.Length; i++)
        {
            float average = 0f;
            int start = i*segmentSize;
            int end = start+segmentSize;
            GameObject bar = bars[i];

            for(int j = start; j < end; j++)
            {
                average += spectrumData[j];
            }
            average /= segmentSize;

            Vector3 newScale = bar.transform.localScale;
            newScale.y = 1 + average * heightMultiplier;
            bar.transform.localScale = newScale;
        }
    }
}
