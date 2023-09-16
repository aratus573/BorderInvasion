using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FreezeTowerController : TowerController
{
    private SlowDownEnemies slowDownEnemies;
    private TowerHeadup towerHeadup;

    protected override void InitializeTowerWhenAwakened()
    {
        base.InitializeTowerWhenAwakened();
        slowDownEnemies = GetComponent<SlowDownEnemies>();
        towerHeadup = GetComponent<TowerHeadup>();
        towerHeadup.SetTiltingSpeed(RotateSpeed);
    }
    public override void SetTowerAttributes(TowerAttributes att)
    {
        base.SetTowerAttributes(att);
        RotateSpeed = att.rotateSpeed;
        CancelInvoke();
        InvokeRepeating(nameof(UpdateTarget), 0f, 1/DetectRate);

    }
    protected override void IdleAndRotate()
    {
        if (!towerHeadup.startToTilt)
        {
            towerHeadup.StartTilting();
            towerHeadup.TiltDown();
        }
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
        }
    }
    protected override void ActOrShoot()
    {
        towerHeadup.TiltUp();
        base.RangingAttack();
        slowDownEnemies.SlowEnemies(EnemiesColliderWithinRange);
    }
}
