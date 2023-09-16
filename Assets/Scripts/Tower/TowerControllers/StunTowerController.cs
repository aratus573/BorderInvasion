using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunTowerController : TowerController
{

    private TowerRotation towerRotation;
    private EnemyStats targetEnemyStats;
    private float stunChance = 0.1f;
    private float stunDuration = 1f;
    private bool canStunEnemy
    {
        get { return (Random.Range(0f,1f) <= stunChance ) ;}
        set {canStunEnemy = value;}
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
            towerRotation.SetTarget(mostDangerousTarget.transform);
        }
    }

    protected override void ActOrShoot()
    {
        base.Shoot();
    }

    public override void TowerActAfterEnemyGotShot()
    {
        if (canStunEnemy)
        {
            StunTargetEnemy();
            canStunEnemy = false;
        }
    }

    private void StunTargetEnemy()
    {
        while(canStunEnemy)
        {
            print("stun enemy");
            targetEnemyStats.ApplyStun(stunDuration);
        }
    }

}
