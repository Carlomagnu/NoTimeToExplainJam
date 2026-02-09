using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class TheaterStateMachine : MonoBehaviour
{
    [SerializeField]
    AudioSource audienceAudio;
    [SerializeField]
    AudioClip lightLaughter;
    [SerializeField]
    AudioClip heavyLaughter;
    [SerializeField]
    GameObject audienceLights;
    [SerializeField]
    AudioClip[] narratorLines;
    [SerializeField]
    AudioSource narratorAudio;
    [SerializeField]
    GameObject audioVisualiser;
    [SerializeField]
    AudioClip lightActivateSound;
    [SerializeField]
    GameObject applauseLight;

    public enum Joke
    {
        First,
        Second,
        Third,
    }

    public enum State
    {
        Audience,
        Narrator,
        Player,
        Finished
    }

    public Joke currentJoke = Joke.First;
    public State currentState = State.Narrator;

    void Start()
    {
        StartCoroutine(stateLoop());
    }

    IEnumerator stateLoop()
    {
        while (true)
        {
            if(currentState == State.Audience)
            {
                audienceLights.SetActive(true);
                yield return StartCoroutine(playAudienceSound());
                yield return StartCoroutine(waitForLaughter());
                nextState();
            }
            else if(currentState == State.Narrator)
            {
                audienceLights.SetActive(false);
                yield return StartCoroutine(waitForNarrator());
                nextState();
            }
            yield return null;
        }
    }

    IEnumerator playAudienceSound()
    {
        audienceAudio.PlayOneShot(lightActivateSound);
        yield return new WaitForSeconds(1f);
        if(currentJoke == Joke.First)
        {
            audienceAudio.PlayOneShot(lightLaughter);
        }
        else if (currentJoke == Joke.Second)
        {
            audienceAudio.PlayOneShot(heavyLaughter);
        }
        else if(currentJoke == Joke.Third)
        {
            yield return new WaitForSeconds(5f);
            audienceAudio.PlayOneShot(lightActivateSound);
            applauseLight.SetActive(true);
        }
        yield return null;
    }

    IEnumerator waitForLaughter()
    {
        while (audienceAudio.isPlaying)
        {
            yield return null;
        }
    }

    IEnumerator waitForNarrator()
    {
        switch (currentJoke)
        {
            case Joke.First:
                narratorAudio.PlayOneShot(narratorLines[0]);
                break;
            case Joke.Second:
                narratorAudio.PlayOneShot(narratorLines[1]);
                break;
            case Joke.Third:
                narratorAudio.PlayOneShot(narratorLines[2]);
                break;
        }
        while (narratorAudio.isPlaying)
        {
            yield return null;
        }
        yield return null;
    }

    public void nextState()
    {
        switch (currentState)
        {
            case State.Narrator:
                currentState = State.Player;
                audioVisualiser.SetActive(true);
                break;
            case State.Player:
                currentState = State.Audience;
                audioVisualiser.SetActive(false);
                break;
            case State.Audience:
                currentState = State.Narrator;
                nextJoke();
                break;
        }
    }

    public void nextStateBtn()
    {
        if(currentState == State.Player)
        {
            currentState = State.Audience;
            audioVisualiser.SetActive(false);
        }
    }

    void nextJoke()
    {
        switch (currentJoke)
        {
            case Joke.First:
                currentJoke = Joke.Second;
                break;
            case Joke.Second:
                currentJoke = Joke.Third;
                break;
            case Joke.Third:
                currentState = State.Finished;
                break;
        }        
    }
}