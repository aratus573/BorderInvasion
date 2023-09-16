using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyStats", menuName = "EnemyStats")]
public class EnemyData : CharacterData
{
    public bool isMeleeAttack;
    public int attack;
    public float attackCD;
    public float targetDetectRange;
    public float attackRange;
    public float resourceDrop;
    public float acceleration;
    public float armor;
    public bool isStunned=false;
    public float currentStunDuration;
    public float currentArmorBreakPercentage;
    public float criticalHitMultiplier;
    public enum EnemyType{Land, Flying}
    public EnemyType enemyType;
}
