using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperRifleDissolve : MeshDissolve
{
    private Color emmisiveColor;
    private void Start()
    {
        emmisiveColor = meshRenderer.materials[1].color;
    }
    private void Update()
    {
        //cutoff 0 : solid, 1 : transparent
        if (State == DissolveState.Dissolve)
        {
            if (meshRenderer.materials[0].GetFloat("_Cutoff") < 1)
            {
                meshRenderer.materials[0].SetFloat("_Cutoff", t);
                t += DissolveSpeed * Time.deltaTime;
            }
            else
            {
                State = DissolveState.Stop;
                meshRenderer.enabled = false;
            }
        }
        else if (State == DissolveState.Assemble)
        {
            if (meshRenderer.materials[0].GetFloat("_Cutoff") > 0)
            {
                meshRenderer.materials[0].SetFloat("_Cutoff", t);
                t -= AssembleSpeed * Time.deltaTime;
            }
            else
            {
                //the blue glowing part
                meshRenderer.materials[1].SetColor("_Color", emmisiveColor);
                State = DissolveState.Stop;
            }
        }
    }
    public override void StartDissolve()
    {
        meshRenderer.materials[0].SetFloat("_Cutoff", 0);
        meshRenderer.materials[1].SetColor("_Color", Color.clear);
        State = DissolveState.Dissolve;
    }
    public override void StartAssemble()
    {
        meshRenderer.materials[0].SetFloat("_Cutoff", 1);
        State = DissolveState.Assemble;
        meshRenderer.enabled = true;
    }
}
