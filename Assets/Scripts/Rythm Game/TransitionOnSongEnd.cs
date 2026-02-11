using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionOnSongEnd : MonoBehaviour
{
    [SerializeField]
    AudioSource music;
    [SerializeField]
    SceneTransition sceneTransition;

    void Start()
    {
        StartCoroutine(waitForSong());
    }

    IEnumerator waitForSong()
    {
        while (music.isPlaying)
        {
            yield return null;
        }
        sceneTransition.changeScene("BREAKOUT");
        yield return null;
    }
}
