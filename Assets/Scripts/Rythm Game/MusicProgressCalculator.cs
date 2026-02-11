using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicProgressCalculator : MonoBehaviour
{
    [SerializeField]
    AudioSource song;
    const float LENGTH_OF_BEAT = 60f/179f;
    const float HZ_OF_SONG = 44100f;
    const float timeSamplesPerBeat = HZ_OF_SONG * LENGTH_OF_BEAT;

    public float getBeatProgress()
    {
        return song.timeSamples / timeSamplesPerBeat;
    }

    public float getBarProgress()
    {
        return 1 + (song.timeSamples / (timeSamplesPerBeat * 4));
    }
}