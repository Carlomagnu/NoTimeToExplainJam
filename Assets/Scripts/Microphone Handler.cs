using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneHandler : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;

    void Start()
    {
        if(Microphone.devices.Length > 0)
        {
            audioSource.clip = Microphone.Start(Microphone.devices[0].ToString(), true, 10, AudioSettings.outputSampleRate);
            audioSource.Play();
        }
    }
}
