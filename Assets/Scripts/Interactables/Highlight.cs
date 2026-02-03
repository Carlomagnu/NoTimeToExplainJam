using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    private Renderer[] renderers;
    public MaterialPropertyBlock mpb;
    private static readonly int HighlightID = Shader.PropertyToID("_Highlight");

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        mpb = new MaterialPropertyBlock();
    }

    public void ToggleHighlight(bool val)
    {
        float glow;
        if (val == true)
        {
            glow = 1f;
        }
        else
        {
            glow = 0f;
        }


        foreach (Renderer rend in renderers)
        {
            rend.GetPropertyBlock(mpb);
            mpb.SetFloat(HighlightID, glow);
            rend.SetPropertyBlock(mpb);
        }
    }
}
