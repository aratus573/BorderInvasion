using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SnipeTowerController : TowerController
{
    TowerRotation towerRotation;
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
            towerRotation.SetTarget(mostDangerousTarget.transform);
        }
    }

    protected override void ActOrShoot()
    {
        base.Shoot();
    }

}
