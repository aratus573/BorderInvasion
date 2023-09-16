using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarFillingShaderController : MonoBehaviour
{
    // <summary>
    //      the higher dampening is, the faster the bar change.
    // </summary>
    private List<Material> barsList;
    // [SerializeField] Image img = null;
    private Material _material;
    [SerializeField] private float newFill;
    private float fillTarget = 1f;
    private float delta = 0f;

    [SerializeField] private float dampening = 5f;
    [SerializeField] private UIManager UI;

    private enum BarState
    {
        Normal, GodMode, OverHeat
    }
    private enum BarType
    {
        Fuel, Health, Base, Enemy
    }
    [SerializeField] private BarType barType;
    [SerializeField] private BarState currState = BarState.Normal;    
    private string[] barStateStrings = new string[3] {"Normal", "GodMode", "OverHeat"};
    void Awake()
    {
        Renderer rend = GetComponent<Renderer>();
        Image img = GetComponent<Image>();
        if (rend != null)
        {
            _material = new Material(rend.material);
            rend.material = _material;
        }
        else if (img != null) 
        {
            _material = new Material(img.material);
            img.material = _material;
            
        }
        else 
        {
            Debug.LogWarning("No Renderer or Image attached to " + name);
        }

    }

    void Start()
    {
        UI = UIManager.instance;
        barsList = UI.barMaterials;
    }

    void Update()
    {
        SetMaterialByState(currState);
        delta -= fillTarget - newFill;
        fillTarget = newFill;

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
        newFill = fillPercentage;
    }


    private void SetMaterialByState(BarState state)
    {
        Image img = GetComponent<Image>();
        
        // 0   , 1     , 2   , 3    , 4      , 5
        // Fuel, Health, Base, Enemy, GodMode, OverHeat
        // if (state != currState)
        {
            switch(state) // switch material by state
            {
                case (BarState.Normal):
                    switch(barType)
                    {
                        case (BarType.Fuel):
                            _material =  new Material(barsList[0]);
                            img.material = _material;
                            break;
                        case (BarType.Health):
                            _material =  new Material(barsList[1]);
                            img.material = _material;
                            break;
                        case (BarType.Base):
                            _material =  new Material(barsList[2]);
                            img.material = _material;
                            break;
                        case (BarType.Enemy):
                            _material =  new Material(barsList[3]);
                            img.material = _material;
                            break;
                    }
                    break;

                case (BarState.GodMode):
                    if (barType == BarType.Fuel || barType == BarType.Health)
                    {
                        // set GodMode material
                        _material =  new Material(barsList[4]);
                        img.material = _material;
                    }
                    break;

                case (BarState.OverHeat):
                    
                    if (barType == BarType.Fuel)
                    {
                        // set OverHeat material
                        _material =  new Material(barsList[5]);
                        img.material = _material;
                    }
                    break;
            }
        }
        
    }

    public void SetCurrStateTo(string stateString)
    {
        BarState tempValue = BarState.Normal;
        for (int i = 0; i < barStateStrings.Length; i++)
        {
            if (barStateStrings[i] == stateString)
            {
                tempValue = (BarState)(Enum.ToObject(typeof(BarState), i));
            }
        }

        currState = tempValue;
    }
    
}
