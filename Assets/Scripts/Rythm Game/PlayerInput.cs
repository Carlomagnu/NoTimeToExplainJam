using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerAniation : MonoBehaviour
{
    enum Direction
    {
        Up,Down,Left,Right,None
    }

    [SerializeField]
    Animator animator;
    [SerializeField]
    ScoreHandler scoreHandler;
    [SerializeField]
    MusicProgressCalculator musicProgressCalculator;
    [SerializeField]
    GameObject inputsParent;
    [SerializeField]
    GameObject currentChartPosition;
    [SerializeField]
    GameObject downPrefab;
    [SerializeField]
    GameObject upPrefab;
    [SerializeField]
    GameObject leftPrefab;
    [SerializeField]
    GameObject rightPrefab;
    [SerializeField]
    GameObject perfectPrefab;
    [SerializeField]
    GameObject greatPrefab;
    [SerializeField]
    GameObject badPrefab;
    [SerializeField]
    GameObject chart;

    private double lastBar = 1d;

    // Update is called once per frame
    void Update()
    {
        Direction direction = Direction.None;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Direction.Up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Direction.Down;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Direction.Left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Direction.Right;
        }

        if (direction != Direction.None)
        {
            double roundedBar = Math.Round(musicProgressCalculator.getBarProgress());

            if(roundedBar != lastBar)
            {
                lastBar = roundedBar;

                changeAnimation(direction);

                spawnArrow(direction);

                ScoreHandler.NoteResult result = scoreHandler.calculateResult();
                scoreHandler.addScore(result);

                spawnResult(result);
            }
        }
    }

    void changeAnimation(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                animator.SetTrigger("Up");
                break;
            case Direction.Down:
                animator.SetTrigger("Down");
                break;
            case Direction.Left:
                animator.SetTrigger("Left");
                break;
            case Direction.Right:
                animator.SetTrigger("Right");
                break;

        }
    }

    void spawnArrow(Direction direction)
    {
        GameObject instance = null;

        switch (direction)
        {
            case Direction.Up:
                instance = Instantiate(upPrefab, inputsParent.transform);
                break;
            case Direction.Down:
                instance = Instantiate(downPrefab, inputsParent.transform);
                break;
            case Direction.Left:
                instance = Instantiate(leftPrefab, inputsParent.transform);
                break;
            case Direction.Right:
                instance = Instantiate(rightPrefab, inputsParent.transform);
                break;
        }

        instance.transform.localPosition = currentChartPosition.transform.localPosition;
    }

    void spawnResult(ScoreHandler.NoteResult result)
    {
        GameObject label = null;
        switch (result)
        {
            case ScoreHandler.NoteResult.Perfect:
                label = perfectPrefab;
                break;
            case ScoreHandler.NoteResult.Great:
                label = greatPrefab;
                break;
            case ScoreHandler.NoteResult.Bad:
                label = badPrefab;
                break;
        }
        GameObject instance = Instantiate(label, chart.transform);
        instance.transform.localPosition = currentChartPosition.transform.localPosition + new Vector3(50f, 20f, 0);
    }
}
