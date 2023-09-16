using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerBullet : MonoBehaviour, IPooledObject
{
    public float currSpeed ;
    public float damage = 0f;

    private Vector3 direction;
    private Transform targetTransform;
    private Vector3 targetCenterPosition;
    private float distanceThisFrame = 0;

    private BuildModeController buildModeController;
    private float oriSpeed = 0f;

    private TowerController parentTowerController;

    void Update()
    {
        if (!targetTransform)
        {
            KillThis();
            return;
        }
        direction = (targetCenterPosition - transform.position);

        // if (buildModeController.BuildMode)
        // {
        //     currSpeed = 0f;
        // }
        // else
        // {
        //     currSpeed = oriSpeed;
        // }

        distanceThisFrame = currSpeed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        // almost hit the target
        {
            EnemyStats enemyStats =  targetTransform.gameObject.GetComponent<EnemyStats>();
            if (enemyStats != null)
            {
                enemyStats.TakeDamage(damage);
                parentTowerController.TowerActAfterEnemyGotShot();
            }
            
            KillThis();
        }

        // move this bullet
        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
        // rotate this bullet
        Quaternion rotation = Quaternion.LookRotation(direction.normalized);
        transform.localEulerAngles = new Vector3(rotation.eulerAngles.x + 90f, rotation.eulerAngles.y, rotation.eulerAngles.z);
    }

    private void KillThis()
    {
        this.gameObject.SetActive(false);
    }

    public void Seek (Transform _target, Vector3 realCenter)
    {
        targetTransform = _target;
        targetCenterPosition = realCenter;
    }

    public void SetTowerController(TowerController targetTower)
    {
        parentTowerController = targetTower;
    }

    public void OnObjectSpawn()
    {
        buildModeController = GameObject.FindGameObjectWithTag("Player").GetComponent<BuildModeController>();
        oriSpeed = currSpeed;
    }
}
