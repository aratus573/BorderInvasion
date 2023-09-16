using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;
    [SerializeField] private List<EnemyMovement> EnemyList;
    [SerializeField] GameObject landEnemyPrefab;
    [SerializeField] GameObject flyingEnemyPrefab;

    [SerializeField] GameObject landHeavyEnemyPrefab;
    [SerializeField] GameObject portalPrefab;
    public List <Transform> LandSpawnPosition;
    public List <Transform> FlyingSpawnPosition;
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerBase;
    [SerializeField] Transform playerBaseFlyingDestination;
    [SerializeField] int maxWaves;
    [SerializeField] private float difficultyHealthMultiplier=1;
    [SerializeField] private float spawnInterval;
    [SerializeField] private int currentWave;
    [SerializeField] private int portalPerWave;
    [SerializeField] private int waveTotalEnemy;
    private bool spawningComplete;

    private enum EnemyType
    {
        Land, Flying, LandHeavy
    }
    private void Awake()
    {
        Instance = this;
    }

    private void Start() {
        switch(DifficultyManager.Instance.difficulty){
            case(0):
                maxWaves=20;
                break;
            case(1):
                maxWaves=30;
                break;
            case(2):
                maxWaves=50;
                break;
            default:
                maxWaves=30;
                break;
        }
    }


    public void WaveSetup()
    {
        currentWave++;
        EnemyType nextWaveEnemyType;
        bool spawnOnLand;

        if (currentWave % 4 == 0)
        {
            portalPerWave++;
        }

        if (currentWave % 5 == 0)
        {
            nextWaveEnemyType = EnemyType.LandHeavy;
            spawnOnLand = true;
        }
        else if (currentWave % 5 == 4)
        {
            nextWaveEnemyType = EnemyType.Flying;
            spawnOnLand = false;
        }
        else
        {
            nextWaveEnemyType = EnemyType.Land;
            spawnOnLand = true;
        }
        
        difficultyHealthMultiplier = DifficultyManager.Instance.GetDifficultyHealthMultiplier(currentWave);
        waveTotalEnemy = DifficultyManager.Instance.GetDifficultyEnemyCount(currentWave);
        spawningComplete = false;
        if(spawnOnLand){
            StartCoroutine(SendWave(portalPerWave, waveTotalEnemy, nextWaveEnemyType, LandSpawnPosition));
        }
        else{
            StartCoroutine(SendWave(portalPerWave, waveTotalEnemy, nextWaveEnemyType, FlyingSpawnPosition));
        }
    }

    IEnumerator SendWave(int portalNumber,int totalEnemy,EnemyType enemyType, List<Transform> spawnPositions){

        foreach(Transform spawn in spawnPositions)
        {
            GameObject enemyPortal= Instantiate(portalPrefab,spawn.position,Quaternion.Euler(0,90,0));
            Destroy(enemyPortal,10f);
        }
        
        int numberOfEnemySpawned = 0;
        while(!spawningComplete)
        {
            foreach(Transform spawn in spawnPositions)
            {
                numberOfEnemySpawned++;
                if(numberOfEnemySpawned > totalEnemy)
                {
                    Debug.Log("spawn complete");
                    spawningComplete = true;
                    break;
                }
                SpawnEnemy(enemyType,spawn.position);
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnEnemy(EnemyType enemyType, Vector3 spawnPos)
    {
        Vector3 offset = new Vector3(UnityEngine.Random.Range(-5f, 5f), 0, 0);
        GameObject enemy, enemyPrefab;
        switch (enemyType)
        {
            case EnemyType.Land:
                enemyPrefab = landEnemyPrefab;
                break;
            case EnemyType.Flying:
                enemyPrefab = flyingEnemyPrefab;
                break;
            case EnemyType.LandHeavy:
                enemyPrefab = landHeavyEnemyPrefab;
                break;
            default:
                enemyPrefab = landEnemyPrefab;
                break;
        }
        enemy = Instantiate(enemyPrefab, spawnPos + offset, Quaternion.identity);
        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        enemyMovement.playerBase = playerBase;
        enemyMovement.player = player;
        enemyMovement.playerBaseFlyingDestination = playerBaseFlyingDestination;
        EnemyList.Add(enemyMovement);
    }

    public void StopAllEnemy()
    {
        StopAllCoroutines();
        foreach (EnemyMovement enemy in EnemyList)
        {
            enemy.Stop();
        }
    }
    public void KillAllEnemy()
    {
        foreach (EnemyMovement enemy in EnemyList)
        {
            enemy.GetComponent<EnemyStats>().EnemyDeath();
        }
    }
    public void EnemyCheer()
    {
        foreach (EnemyMovement enemy in EnemyList)
        {
            enemy.GetComponent<EnemyAnimation>().PlayCheerAnimation();
        }
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public float GetHealthMultiplier()
    {
        return difficultyHealthMultiplier;
    }

    public void EnemyDies(EnemyMovement enemy)
    {
        EnemyList.Remove(enemy);
        if(spawningComplete && EnemyList.Count == 0)
        {
            StartCoroutine(GameStateManager.Instance.WaveComplete());
            if (currentWave == maxWaves)
            {
                StartCoroutine(GameStateManager.Instance.GameWin());
            }
        }
    }

}
