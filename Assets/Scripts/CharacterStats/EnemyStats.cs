using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class EnemyStats : CharacterStats
{
    public EnemyData templateData;
    public EnemyData enemyData;
    [SerializeField] private GameObject minimapIcon;
    EnemyAnimation enemyAnimation;
    EnemyMovement enemyMovement;
    NavMeshAgent navMeshAgent;
    private void Update() {
        HandleStun();
    }
    private void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
        enemyData = (EnemyData)characterData;
        enemyAnimation=GetComponent<EnemyAnimation>();
        enemyMovement=GetComponent<EnemyMovement>();
        navMeshAgent=GetComponent<NavMeshAgent>();
    }
    public override void TakeDamage(float damage)
    {
        damage=damage/(damage+enemyData.armor);
        //for entities that cant critical hit
        characterData.CurrentHealth = Mathf.Max(0, characterData.CurrentHealth - damage);
        enemyAnimation.PlayHurtAnimation();

        if (TryGetComponent(out FloatingHealthBar floatingHealthBar))
        {
            floatingHealthBar.ShowFloatingText(damage.ToString(), Color.white);
            floatingHealthBar.UpdateHealthBar(characterData.CurrentHealth, characterData.MaxHealth);
        }

        if (characterData.CurrentHealth==0){
            enemyMovement.enabled=false;
            navMeshAgent.enabled=false;
            EnemyDeath();
        }
    }
    public void TakeDamage(float damage,bool isCriticalHit)
    {
        if(isCriticalHit){
            damage*=enemyData.criticalHitMultiplier;
        }
        //armor damage reduction calc
        damage=damage/(damage+enemyData.armor);

        characterData.CurrentHealth = Mathf.Max(0, characterData.CurrentHealth - damage);
        enemyAnimation.PlayHurtAnimation();

        if (TryGetComponent(out FloatingHealthBar floatingHealthBar))
        {
            floatingHealthBar.ShowFloatingText(damage.ToString(), Color.white);
            floatingHealthBar.UpdateHealthBar(characterData.CurrentHealth, characterData.MaxHealth);
        }

        if (characterData.CurrentHealth==0){
            enemyMovement.enabled=false;
            navMeshAgent.enabled=false;
            EnemyDeath();
        }
    }

    public void ApplyStun(float stunDuration){
        if(enemyData.currentStunDuration<stunDuration){
            enemyData.currentStunDuration=stunDuration;
        }
    }

    public void ApplyArmorBreak(float armorBreakPercentage){
        if(enemyData.currentArmorBreakPercentage<armorBreakPercentage){
            enemyData.currentArmorBreakPercentage=armorBreakPercentage;
            enemyData.armor=templateData.armor*(1-enemyData.currentArmorBreakPercentage);
        }
    }

    void HandleStun(){
        if(enemyData.currentStunDuration>0){
            enemyData.isStunned=true;
            enemyData.MoveSpeed=0;
            enemyData.currentStunDuration-=Time.deltaTime;
            return;
        }
        enemyData.isStunned=false;
        enemyData.MoveSpeed=templateData.MoveSpeed;
    }

    #region Method Called by Other Methods
    public void EnemyDeath(){
        gameObject.tag="DeadEnemy";
        gameObject.layer=11;
        minimapIcon.SetActive(false);
        enemyAnimation.PlayDeathAnimation();
        Destroy(gameObject,3f);
        UpgradeManager.Instance.playerResource+=enemyData.resourceDrop+(EnemyManager.Instance.GetCurrentWave() / 8);
        EnemyManager.Instance.EnemyDies(enemyMovement);
    }
    #endregion


    #region Read from enemyData
    public bool IsMeleeAttack
    {
        get { if (enemyData != null) return enemyData.isMeleeAttack; else return false; }
        set { enemyData.isMeleeAttack = value; }
    }
    public int Attack
    {
        get { if (enemyData != null) return enemyData.attack; else return 0; }
        set { enemyData.attack = value; }
    }
    public float AttackCD
    {
        get { if (enemyData != null) return enemyData.attackCD; else return 0; }
        set { enemyData.attackCD = value; }
    }
    public float AttackRange
    {
        get { if (enemyData != null) return enemyData.attackRange; else return 0; }
        set { enemyData.attackRange = value; }
    }
    public float TargetDetectRange
    {
        get { if (enemyData != null) return enemyData.targetDetectRange; else return 0; }
        set { enemyData.targetDetectRange = value; }
    }
    public float ResourceDrop
    {
        get { if (enemyData != null) return enemyData.resourceDrop; else return 0; }
        set { enemyData.resourceDrop = value; }
    }
    public float Acceleration
    {
        get { if (enemyData != null) return enemyData.acceleration; else return 0; }
        set { enemyData.acceleration = value; }
    }
    public EnemyData.EnemyType EnemyType
    {
        get { if (enemyData != null) return enemyData.enemyType; else return 0; }
        set { enemyData.enemyType = value; }
    }
    #endregion
}
