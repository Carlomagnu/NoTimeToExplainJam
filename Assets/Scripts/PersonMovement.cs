using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMovement : MonoBehaviour
{
    TheaterStateMachine theaterStateMachine;
    Animator animator;

    void Start()
    {
        theaterStateMachine = GameObject.FindObjectOfType<TheaterStateMachine>();
        animator = GetComponent<Animator>();
        animator.SetFloat("Speed", Random.Range(0.8f, 1.2f));
    }

    void Update()
    {
        bool activate = theaterStateMachine.currentState == TheaterStateMachine.State.Audience && theaterStateMachine.currentJoke != TheaterStateMachine.Joke.Third;
        animator.SetBool("Move", activate);
    }
}
