using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class PrintWorldLocal : MonoBehaviour
{
    void Update()
    {
        Debug.Log($"{name}  local:{transform.localPosition}  world:{transform.position}  parent:{(transform.parent?transform.parent.name:"none")}");
        enabled = false; // print once
    }
}
