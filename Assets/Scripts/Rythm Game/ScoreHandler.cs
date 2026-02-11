using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreHandler : MonoBehaviour
{
    [SerializeField]
    MusicProgressCalculator musicProgressCalculator;

    public enum NoteResult
    {
        Bad,
        Great,
        Perfect
    }

    private int score;

    public int getScore() => score;

    public void addScore(NoteResult result)
    {
        switch (result)
        {
            case NoteResult.Bad:
                score += 50;
                break;
            case NoteResult.Great:
                score += 100;
                break;
            case NoteResult.Perfect:
                score += 300;
                break;
        }
    }

    public NoteResult calculateResult()
    {
        double delay = musicProgressCalculator.getBarProgress() - Math.Floor(musicProgressCalculator.getBarProgress());
        if(delay < 0.1d || delay > 0.9d)
        {
            return NoteResult.Perfect;
        } else if(delay < 0.3f || delay > 0.7f)
        {
            return NoteResult.Great;
        }
        else
        {
            return NoteResult.Bad;
        }
    }
}
