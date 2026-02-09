using UnityEngine;

public class PosterState : MonoBehaviour, IPoster
{
    [Header("Poster Materials")]
    public Material[] states;

    [Header("Settings")]
    public bool isReferencePoster = false;

    private int currentIndex;
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        ApplyState();
    }

    // Called when hallway loops
    public void RandomizeState()
    {
        if (isReferencePoster) return;

        currentIndex = Random.Range(0, states.Length);
        ApplyState();
    }

    // Called when shots
    public void OnShot(RaycastHit hit)
    {
        CycleState();
    }


    private void CycleState()
    {
        if (isReferencePoster) return;

        currentIndex++;
        if (currentIndex >= states.Length)
            currentIndex = 0;

        ApplyState();
    }

    public int GetStateIndex()
    {
        return currentIndex;
    }

    void ApplyState()
    {
        if (rend && states.Length > 0)
            rend.material = states[currentIndex];
    }
}
