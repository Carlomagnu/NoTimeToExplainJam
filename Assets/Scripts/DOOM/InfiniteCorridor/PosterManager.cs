using UnityEngine;

public class PosterManager : MonoBehaviour
{
    public static PosterManager Instance;

    public PosterState referencePoster;
    public PosterState[] otherPosters;

    //Sounds
    [SerializeField] AudioSource source;
    [SerializeField] AudioClip victory;

    //Lift
    [SerializeField] ElevatorController elevator;


    private void Awake()
    {
        Instance = this;
    }

    public void CheckSolved()
    {
        int targetIndex = referencePoster.GetStateIndex();
        Debug.Log("Reference: " + targetIndex);

        foreach (var poster in otherPosters)
        {
            if (poster.GetStateIndex() != targetIndex)
                return;
        }

        Solve();
    }

    void Solve()
    {
        Debug.Log("Posters matched!");
        victorySound();
        elevator.CallElevator();
    }

    private void victorySound()
    {
        source.PlayOneShot(victory);
    }
}