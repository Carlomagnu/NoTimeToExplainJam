using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorA : MonoBehaviour
{
    public float activeSeconds = 6f;
    private float _timer;

    public bool IsActive => _timer > 0f;

    public void Activate()
    {
        _timer = activeSeconds;
        Debug.Log("Generator A ACTIVE");
    }

    void Update()
    {
        if (_timer > 0f)
            _timer -= Time.deltaTime;
    }
}

