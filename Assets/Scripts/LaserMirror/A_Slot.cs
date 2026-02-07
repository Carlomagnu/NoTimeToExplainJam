using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class A_Slot : MonoBehaviour
{
    public GeneratorA generatorA;

    private bool _processing = false;
    private ChargeableCube _insertedCube;

    void OnTriggerEnter(Collider other)
    {
        if (_processing) return;

        var cube = other.GetComponentInParent<ChargeableCube>();
        if (cube == null) return;

        // Ignore if a cube is already inserted
        if (_insertedCube != null && cube != _insertedCube) return;

        _processing = true;

        // Attempt to consume charge (single source of truth)
        if (!cube.ConsumeCharge())
        {
            Debug.Log("Cube inserted but NOT charged.");
            _processing = false;
            return;
        }

        Debug.Log("Cube inserted and CHARGED. Activating Generator A.");

        _insertedCube = cube;

        // Freeze cube physics
        var rb = cube.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        // Snap cube into slot
        cube.transform.position = transform.position;
        cube.transform.rotation = transform.rotation;

        // Disable cube colliders to prevent trigger spam
        foreach (var col in cube.GetComponentsInChildren<Collider>())
            col.enabled = false;

        if (generatorA != null)
            generatorA.Activate();

        _processing = false;
    }
}