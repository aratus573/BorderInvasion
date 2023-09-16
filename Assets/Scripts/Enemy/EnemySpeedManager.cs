/*
Slowed this.gameObject, called by tower, speed will recover by itself
*/




using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemySpeedManager : MonoBehaviour
{
    public float oriSpeed;
    public float currSpeed;
    private EnemyStats enemyStats;
    private NavMeshAgent navMeshAgent;
    private SlowDownEnemies slowDownEnemies;
    [SerializeField] float timer = 0f;
    [SerializeField] private float recoverTimer = Mathf.Infinity;
    [SerializeField] private float slowingTimer = Mathf.Infinity;
    private float targetSpeedMultiplier = 0.7f;
    private float slowedDuration = 1.5f;

    public enum State
    {
        Normal, Slowed, Recovering
    }
    public State currState = State.Normal;

    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        slowDownEnemies = GetComponent<SlowDownEnemies>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        oriSpeed = enemyStats.MoveSpeed;
        currSpeed = oriSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        timer = Time.time;
        BehaveByState(currState);
    }
    
    private void SetMoveSpeedTo(float targetSpeed)
    {
        enemyStats.MoveSpeed = targetSpeed;
        navMeshAgent.speed = enemyStats.MoveSpeed;
    }

    public void BehaveByState(State state)
    {
        switch(state)
        {
            case (State.Normal):
                if (currSpeed != oriSpeed)
                {
                    currSpeed = oriSpeed;
                    SetMoveSpeedTo(currSpeed);
                }
                break;

            case (State.Slowed):
                if (currSpeed != oriSpeed * targetSpeedMultiplier)
                {
                    currSpeed = oriSpeed * targetSpeedMultiplier;
                    SetMoveSpeedTo(currSpeed);
                }
                
                if (timer - slowingTimer >= slowedDuration)
                {
                    SetSpeedState(State.Recovering);
                    SetRecoverStartTime();
                }

                break;

            case (State.Recovering):
                if (timer - recoverTimer >= slowedDuration) // completly recover from being slowed after slowedDuration seconds
                {
                    currSpeed = oriSpeed;
                    SetMoveSpeedTo(oriSpeed);
                    currState = State.Normal;
                    break;
                }
                else
                {
                    // slowly recover from being slowed
                    currSpeed = Mathf.Lerp(oriSpeed * targetSpeedMultiplier, oriSpeed, timer - recoverTimer / slowedDuration);
                    SetMoveSpeedTo(currSpeed);
                    break;
                }
        }
    }

    public void SetSpeedState(State targetState)
    {
        currState = targetState;
    }

    public void SetSlowStartTime()
    {
        slowingTimer = timer;
    }

    public void SetRecoverStartTime()
    {
        recoverTimer = timer;
    }
}
