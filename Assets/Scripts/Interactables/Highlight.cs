using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    public Renderer rend;
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
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    public void ToggleHighlight(bool val)
    {
        rend.GetPropertyBlock(mpb);
        if (val)
        {
            mpb.SetFloat(HighlightID, 1f);
        }
        else
        {
            mpb.SetFloat(HighlightID, 0f);
        }
        rend.SetPropertyBlock(mpb);
    }
}
