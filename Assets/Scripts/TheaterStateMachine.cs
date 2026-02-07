using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;


public class TheaterStateMachine : MonoBehaviour
{
    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioClip lightLaughter;
    [SerializeField]
    AudioClip heavyLaughter;
    [SerializeField]
    GameObject audienceLights;

    enum Joke
    {
        First,
        Second,
        Third,
        Finished
    }

    enum State
    {
        Audience,
        Narrator,
        Player,
        Idle
    }

    private Joke currentJoke = Joke.First;
    private State currentState = State.Narrator;

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
                if(currentJoke == Joke.First)
                {
                    audioSource.PlayOneShot(lightLaughter);
                }
                else if (currentJoke == Joke.Second)
                {
                    audioSource.PlayOneShot(heavyLaughter);
                }
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

    IEnumerator waitForLaughter()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
    }

    IEnumerator waitForNarrator()
    {
        yield return null;
    }

    public void nextState()
    {
        switch (currentState)
        {
            case State.Narrator:
                currentState = State.Player;
                break;
            case State.Player:
                currentState = State.Audience;
                break;
            case State.Audience:
                currentState = State.Narrator;
                nextJoke();
                break;
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
                currentJoke = Joke.Finished;
                break;
        }        
    }
}