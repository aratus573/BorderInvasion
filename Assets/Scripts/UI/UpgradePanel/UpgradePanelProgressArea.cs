using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class UpgradePanelProgressArea : MonoBehaviour
{
    [SerializeField] private Text text;
    [SerializeField] private Image bar;
    [SerializeField] private Material _material;

    private int currTime = 0;
    private int maxTime = 0;

    void Start()
    {
        _material = new Material(bar.material);
        bar.material = _material;
    }

    void Update()
    {
        
    }

    public void SetCurrTime(int _num)
    {
        currTime = _num;
    }

    public void SetMaxTime(int _num)
    {
        maxTime = _num;
    }

    public void RefreshProgressArea()
    {
        RefreshText();
        RefreshBar();
    }
    public void RefreshText()
    {
        text.text = currTime.ToString() + " / " + maxTime.ToString();
    }

    public void RefreshBar()
    {
        float percentage = (float)currTime / maxTime;
        Debug.Log(percentage);

    }

}
