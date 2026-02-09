using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


public class DoNotDoStateManager : MonoBehaviour
{
    enum State
    {
        Duck,
        Journey,
        Bomb,
        Uranium,
        Button
    }

    [SerializeField]
    Animator pondAnimator;
    [SerializeField]
    Animator tableAnimator;
    [SerializeField]
    Animator buttonAnimator;
    [SerializeField]
    GameObject labLight;
    [SerializeField]
    GameObject bombLight;
    [SerializeField]
    AudioSource bombTick;
    [SerializeField]
    AudioSource buttonClick;
    [SerializeField]
    AudioSource explosion;

    private State state = State.Duck;

    public void nextState()
    {
        switch (state)
        {
            case State.Duck:
                pondAnimator.SetTrigger("Off");
                tableAnimator.SetTrigger("On");
                state = State.Bomb;
                StartCoroutine(blinkBombLight());
                break;
            case State.Bomb:
                state = State.Button;
                tableAnimator.SetTrigger("Off");
                buttonAnimator.SetTrigger("On");
                break;
            case State.Button:
                StartCoroutine(bombSequence());
                break;
        }
    }

    IEnumerator bombSequence()
    {
        buttonClick.Play();
        yield return new WaitForSeconds(1.5f);
        activateFloat();
        explosion.Play();

        CameraShake cameraShake = GameObject.FindFirstObjectByType<CameraShake>();
        cameraShake.Shake(1f, 1f);

        yield return null;
    }

    void activateFloat()
    {
        Rigidbody[] bodies = GameObject.FindObjectsByType<Rigidbody>(FindObjectsSortMode.None);
        Physics.gravity = new UnityEngine.Vector3(0f, 0f, 0f);
        foreach(Rigidbody body in bodies)
        {
            body.isKinematic = false;
            body.AddForce(randomDirection() * 0.5f);
        }
    }

    private UnityEngine.Vector3 randomDirection()
    {
        UnityEngine.Vector3 vec = new UnityEngine.Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f),Random.Range(0f, 1f));
        return vec.normalized;
    }

    private IEnumerator blinkBombLight()
    {
        yield return new WaitForSeconds(1f);
        while(state == State.Bomb)
        {
            labLight.SetActive(false);
            bombLight.SetActive(true);
            bombTick.Play();
            yield return new WaitForSeconds(0.3f);
            labLight.SetActive(true);
            bombLight.SetActive(false);
            yield return new WaitForSeconds(1.2f);
        }

        yield return null;
    }
}
