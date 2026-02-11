using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowScore : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textMeshPro;
    [SerializeField]
    ScoreHandler scoreHandler;

    void Update()
    {
        textMeshPro.text = scoreHandler.getScore().ToString();
    }
}
