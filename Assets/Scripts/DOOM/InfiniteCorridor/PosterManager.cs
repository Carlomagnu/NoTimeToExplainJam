using UnityEngine;

public class PosterManager : MonoBehaviour
{
    public static PosterManager Instance;

    public PosterState referencePoster;
    public PosterState[] otherPosters;

    public GameObject exitDoor;

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

        if (exitDoor)
            exitDoor.SetActive(true);
    }
}