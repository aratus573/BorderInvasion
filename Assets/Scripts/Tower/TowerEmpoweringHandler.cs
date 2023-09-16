using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerEmpoweringHandler : MonoBehaviour
{
    bool isEmpowered
    {
        get {return towerEmpowerStacks >= 1 || focusEmpowerStacks >= 1;}
    }
    int oriDamage;
    int towerEmpowerStacks = 0;
    int focusEmpowerStacks = 0;
    int focusStacksMaxLimit = 5;
    [SerializeField] TowerController controller;
    float towerEmpowerPercentage;
    float focusEmpowerPercentage;
    float totalFocusEmpowerAmplifier;
    float totalTowerEmpowerAmplifier;

    private void Awake() {
        ResetOriDamage();
    }
    public void ResetOriDamage()
    {
        oriDamage = controller.CurrentStats.towerDamage;
    }
    public void AddEmpowerStatck()
    {
        towerEmpowerStacks += 1;
        ApplyDamage();
    }
    public void TryAddingFocusStack()
    {
        focusEmpowerStacks = Mathf.Clamp(focusEmpowerStacks + 1, 0, focusStacksMaxLimit);
        ApplyDamage();
    }

    public void ResetFocusStatck()
    {
        focusEmpowerStacks = 0;
        ApplyDamage();
    }
    public void SubtractEmpowerStatck()
    {
        towerEmpowerStacks = Mathf.Clamp(towerEmpowerStacks - 1, 0, towerEmpowerStacks - 1);
        ApplyDamage();
    }
    public void SetPowerPercentage(float _empowerPercentage)
    {
        if (_empowerPercentage == towerEmpowerPercentage) {return; }
        towerEmpowerPercentage = _empowerPercentage;
        ApplyDamage();
    }
    public void SetFocusPowerPercentage(float _focusEmpowerPercentage)
    {
        if (_focusEmpowerPercentage == focusEmpowerPercentage) {return; }
        focusEmpowerPercentage = _focusEmpowerPercentage;
        ApplyDamage();
    }
    public void SetController(TowerController _controller)
    {
        if (_controller == controller) {return; }
        controller = _controller;
    }

    private void ApplyDamage()
    {
        controller.TowerDamage = Mathf.RoundToInt(MultipliedEmpoweredDamage());
    }
    private float MultipliedEmpoweredDamage()
    {
        if (!isEmpowered) 
        {
            return oriDamage;
        }
        totalTowerEmpowerAmplifier = 1f + towerEmpowerPercentage * towerEmpowerStacks;
        totalFocusEmpowerAmplifier = 1f + focusEmpowerPercentage * focusEmpowerStacks;
        return oriDamage * totalTowerEmpowerAmplifier * totalFocusEmpowerAmplifier;
    }

}
