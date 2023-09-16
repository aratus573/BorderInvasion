using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextWaveCountdownUI : MonoBehaviour
{
    #region Fields
    public static NextWaveCountdownUI instance;

    private Material _material;
    private float newFill;
    public float NewFill
    {
        get {return newFill;}
        set 
        {
            if (value >= 1f)
            {
                newFill = 1f;
            }
            else if (value <= 0f)
            {
                newFill = 0f;
            }
            else
            {
                newFill = value;
            }
        }
    }
    private float fillTarget;
    private float delta;
    private UIManager UI;
    [SerializeField] private float dampening = 1;
    // Start is called before the first frame update

    public enum ComingState
    {
        Normal, Flying, Heavy
    }
    [SerializeField] private ComingState nextWave;
    private List<Material> barList = new List<Material>();

    #endregion
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        UI = UIManager.instance;
        barList = UI.waveCountdownMaterials;
    }


    void Update()
    {
        SetMaterialByState(nextWave);

        delta -= fillTarget - NewFill;
        fillTarget = NewFill;

        if (Mathf.Abs(delta) > 0.001f)
        {
            delta = Mathf.Lerp(delta, 0, Time.deltaTime * dampening);
        }
        else
        {
            delta = 0f;
        }

        _material.SetFloat("_Delta", delta);
        _material.SetFloat("_Fill", fillTarget);
    }

    public void SetNewFill(float fillPercentage)
    {
        NewFill = fillPercentage;
    }

    public void SetMaterialByState(ComingState state)
    {
        Image img = GetComponent<Image>();
        switch(state)
        {
            case (ComingState.Normal):
                _material = new Material(barList[0]);
                img.material = _material;
                break;
            case (ComingState.Flying):
                _material = new Material(barList[1]);
                img.material = _material;
                break;
            case (ComingState.Heavy):
                _material = new Material(barList[2]);
                img.material = _material;
                break;
            default:
                _material = new Material(barList[0]);
                img.material = _material;
                break;
        }
    }

    public void SetNextWaveStateByString(string stateString)
    {
        switch(stateString)
        {
            case ("Land"):
                nextWave = ComingState.Normal;
                break;
            case ("Flying"):
                nextWave = ComingState.Flying;
                break;
            case ("Heavy"):
                nextWave = ComingState.Heavy;
                break;
        }
    }
}
