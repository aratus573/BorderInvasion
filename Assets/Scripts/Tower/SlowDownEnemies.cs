using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownEnemies : MonoBehaviour
{
    [SerializeField] private TowerController towerController;
    void Awake()
    {
        towerController = GetComponent<TowerController>();
    }

    public void SlowEnemies(Collider[] enemiesWithinRange)
    {
        foreach (Collider collider in enemiesWithinRange)
        {
            EnemySpeedManager speedManager = collider.GetComponent<EnemySpeedManager>();
            speedManager.SetSpeedState(EnemySpeedManager.State.Slowed);
            speedManager.SetSlowStartTime();
        }
    }
}
