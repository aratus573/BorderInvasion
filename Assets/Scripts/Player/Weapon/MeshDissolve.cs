using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDissolve : MonoBehaviour
{
    protected enum DissolveState
    {
        Dissolve,
        Assemble,
        Stop
    }
    protected DissolveState State = DissolveState.Stop;

    protected Renderer meshRenderer;
    public float DissolveSpeed = 2f;
    public float AssembleSpeed = 2f;
    protected float t;
    protected void Awake()
    {
        meshRenderer = this.GetComponent<Renderer>();
    }
    private void Update()
    {
        //cutoff 0 : solid, 1 : transparent
        if (State == DissolveState.Dissolve)
        {
            if (meshRenderer.materials[1].GetFloat("_Cutoff") < 1)
            {
                meshRenderer.materials[1].SetFloat("_Cutoff", t);
                t += DissolveSpeed * Time.deltaTime;
            }
            else
            {
                State = DissolveState.Stop;
                meshRenderer.enabled = false;
            }
            
        }
        else if(State == DissolveState.Assemble)
        {
            if (meshRenderer.materials[1].GetFloat("_Cutoff") > 0)
            {
                meshRenderer.materials[1].SetFloat("_Cutoff", t);
                t -= AssembleSpeed * Time.deltaTime;
            }
            else
            {
                meshRenderer.materials[0].SetColor("_Color", Color.white);
                //disable the dissolve material
                meshRenderer.materials[1].SetFloat("_Cutoff", 1);
                State = DissolveState.Stop;
            }
        }
    }
    public virtual void StartDissolve()
    {
        meshRenderer.materials[0].SetColor("_Color", Color.clear);
        meshRenderer.materials[1].SetFloat("_Cutoff", 0);
        State = DissolveState.Dissolve;
    }
    public virtual void StartAssemble()
    {
        meshRenderer.materials[1].SetFloat("_Cutoff", 1);
        State = DissolveState.Assemble;
        meshRenderer.enabled = true;
    }
}
