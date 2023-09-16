using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpowerTowerController : TowerController
{
    [SerializeField] private float AffectedRadius;
    public float empowerDamagePercentage
    {
        get ;
        private set;
    }
    private Collider[] TowerCollidersNearby;
    
    

    protected override void InitializeTowerWhenAwakened()
    {
        base.InitializeTowerWhenAwakened();
        empowerDamagePercentage = 0.3f;
    }
    public override void SetTowerAttributes(TowerAttributes att)
    {
        base.SetTowerAttributes(att);
        AffectedRadius = DetectRange;
    }

    public void ResetTowersNearby()
    {
        TowerCollidersNearby = Physics.OverlapSphere(TowerBase.transform.position, AffectedRadius, layerOfTower);
    }

    public override void OnPlace()
    {
        ResetTowersNearby();
        foreach (Collider col in TowerCollidersNearby)
        {
            if (col.gameObject.tag != "Tower") {continue;}
            TowerEmpoweringHandler damageHandler = col.GetComponent<TowerEmpoweringHandler>();
            damageHandler.SetPowerPercentage(empowerDamagePercentage);
            damageHandler.AddEmpowerStatck();
        }
    }

    public override void OnSold()
    {
        foreach (Collider col in TowerCollidersNearby)
        {
            if (col.gameObject.tag != "Tower") {continue;}
            TowerEmpoweringHandler damageHandler = col.GetComponent<TowerEmpoweringHandler>();
            damageHandler.SubtractEmpowerStatck();
        }
    }








}
