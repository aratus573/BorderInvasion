using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorBreakTowerController : TowerController
{
    // float recoveringTimer = 0f;
    public float ArmorBreakPercentage
    {
        get { return armorBreakPercentage; }
        set { armorBreakPercentage = Mathf.Clamp(value, 0, 1); }
    }
    private float armorBreakPercentage = 0.3f; // if armorBreakPercentage == 0.3, armor = armor * (1 - 0.3)
    private TowerRotation towerRotation;
    private EnemyStats targetEnemyStats;
    private float oriEnemyArmor;
    private bool targetEnemyIsBroken 
    {
        get {return targetEnemyStats.enemyData.armor < oriEnemyArmor;}
    }

    protected override void InitializeTowerWhenAwakened()
    {
        base.InitializeTowerWhenAwakened();
        towerRotation = GetComponent<TowerRotation>();
        towerRotation.SetFirePosition(FirePosition);
        towerRotation.SetRotateSpeed(RotateSpeed);
    }

    public override void SetTowerAttributes(TowerAttributes att)
    {
        base.SetTowerAttributes(att);
        RotateSpeed = att.rotateSpeed;
        FireRate = att.fireRate;
        BulletPrefab = att.bulletPrefab;
        CancelInvoke();
        InvokeRepeating(nameof(UpdateTarget), 0f, 1/DetectRate);
    }
    protected override void ActOrShoot()
    {
        base.Shoot();
        if (!targetEnemyIsBroken) { BreakTargetEnemysArmor(); }
            
    }
    protected override void IdleAndRotate()
    {
        towerRotation.StartRotating();
    }
    protected override void UpdateTarget()
    {
        EnemiesColliderWithinRange = Physics.OverlapSphere(TowerBase.transform.position, DetectRange, layerOfEnemy);
     
        if (EnemiesColliderWithinRange.Length == 0)
        {
            targetEnemyTransform = null;
            return;
        }

        Collider mostDangerousTarget = SearchForTarget();
        
        if (mostDangerousTarget)
        {
            targetColliderCenter = mostDangerousTarget.bounds.center;
            targetEnemyTransform = mostDangerousTarget.transform;
            targetEnemyStats = mostDangerousTarget.GetComponent<EnemyStats>();
            oriEnemyArmor = targetEnemyStats.templateData.armor;
            towerRotation.SetTarget(mostDangerousTarget.transform);
            
        }
    }

    // !!!
    private void BreakTargetEnemysArmor()
    {
        targetEnemyStats.ApplyArmorBreak(armorBreakPercentage);
        print ($"armor of {targetEnemyTransform.gameObject.name} is broken");
    }

    public override void TowerActAfterEnemyGotShot()
    {
        BreakTargetEnemysArmor();
    }
}
