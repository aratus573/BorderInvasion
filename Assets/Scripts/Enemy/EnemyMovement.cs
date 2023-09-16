using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] EnemyType enemyType;
    [SerializeField] GameObject gun;
    Vector3 firePoint;
    NavMeshAgent navMeshAgent;
    enum EnemyType{Land, Flying}
    [SerializeField] GameObject bulletPrefab;
    public GameObject playerBase,player;
    public Transform playerBaseFlyingDestination;
    float attackTimer;
    public LayerMask DetectionLayerMask;
    public bool IsMoving;
    private Transform target;
    private GameObject meleeTarget;
    public AudioClip attackSound;
    EnemyAnimation enemyAnimation;
    EnemyStats enemyStats;
    EnemyManager enemyManager;
    private void Start(){
        enemyManager=EnemyManager.Instance;
        navMeshAgent=GetComponent<NavMeshAgent>();

        enemyStats=GetComponent<EnemyStats>();
        enemyStats.MaxHealth*=enemyManager.GetHealthMultiplier();
        enemyStats.CurrentHealth = enemyStats.MaxHealth;
        
        navMeshAgent.stoppingDistance=enemyStats.AttackRange;
        navMeshAgent.speed=enemyStats.MoveSpeed;
        navMeshAgent.acceleration=enemyStats.Acceleration;
        enemyAnimation=GetComponent<EnemyAnimation>();
    }
    private void Update() {
        ChooseTarget();
        MoveEnemy();
        if(TargetInAttackRange()){EnemyAttack();}
        attackTimer-=Time.deltaTime;
        SetUIIndicator();
    }
    private void MoveEnemy(){
        if(enemyType == EnemyType.Flying)
        {
            navMeshAgent.SetDestination(playerBaseFlyingDestination.position);
        }
        else
        {
            navMeshAgent.SetDestination(target.position);
        }

    }
    private bool PlayerDetected(){
        firePoint=gun.transform.position;
        //check if player in detected and in LoS
        Vector3 direction = ((player.transform.position+Vector3.up) - firePoint).normalized;
        if (Physics.Raycast(firePoint, direction, out RaycastHit hit, enemyStats.TargetDetectRange, DetectionLayerMask) && hit.transform.tag=="Player"){
            //Debug.Log("found player");
            return true;
        }
        return false;
    } 
    private void ChooseTarget(){
        //change enemy target
        if(enemyType == EnemyType.Flying)
        {
            target = playerBase.transform;
            return;
        }
        else if(PlayerDetected())
        {
            target = player.transform;
            return;
        }
        else
        {
            target = playerBase.transform;
        }
    }
    private bool TargetInAttackRange(){

        Vector3 direction;
        if(target == player.transform){
             direction = ((player.transform.position+Vector3.up) - firePoint).normalized;
        }
        else{
            direction = (target.position - firePoint).normalized;
        }
        //if melee attacking, always rotate, can't move
        if(enemyStats.enemyData.isMeleeAttack && attackTimer > 0){
            IsMoving=false;
            navMeshAgent.isStopped = true;
            Quaternion rotation =Quaternion.LookRotation(direction);
            transform.rotation=Quaternion.Lerp(transform.rotation,rotation,5*Time.deltaTime);
            return false;
        }
        navMeshAgent.isStopped = false;
        //check if target in attack range
        if(Physics.Raycast(firePoint, direction, out RaycastHit hit, enemyStats.AttackRange, DetectionLayerMask)){
            //Debug.Log("target in attack range");
            IsMoving=false;
            navMeshAgent.updateRotation=false;
            Quaternion rotation =Quaternion.LookRotation(direction);
            transform.rotation=Quaternion.Lerp(transform.rotation,rotation,5*Time.deltaTime);
            return true;
        }
        navMeshAgent.updateRotation=true;
        IsMoving=true;
        return false;
    }
    private void EnemyAttack(){
        //attack if target in range
        if(attackTimer>0){return;}
        if(enemyStats.enemyData.isStunned){return;}
        attackTimer=enemyStats.AttackCD;

        if(enemyStats.IsMeleeAttack)
        {
            meleeTarget = target.gameObject;
            enemyAnimation.PlayMeleeAnimation();    
            return;
        }

        AudioSource.PlayClipAtPoint(attackSound,transform.position);
        GameObject bullet = Instantiate(bulletPrefab, firePoint, Quaternion.identity);
        bullet.transform.LookAt(target.position);
        bullet.GetComponent<EnemyProjectile>().SetDamage(enemyStats.Attack);
        //Debug.Log("enemy attack target");
    }

    public void MeleeHit(){
        if(meleeTarget == null) return;
        switch(meleeTarget.tag){
            case "Player":
                meleeTarget.GetComponent<PlayerStats>().TakeDamage(enemyStats.Attack);
                meleeTarget = null;
                break;
            case "PlayerBase":
                meleeTarget.GetComponent<PlayerBase>().TakeDamage(enemyStats.Attack);
                meleeTarget = null;
                break;
            default: 
                Debug.Log("error, melee target can't take damage");
                meleeTarget = null;
                break;
        }
    }

    public float GetPathRemainingDistance(){
        //copied from stack overflow, correctly calculate remaining distance
        if (navMeshAgent.pathPending ||
            navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid ||
            navMeshAgent.path.corners.Length == 0)
            return -1f;

        float distance = 0.0f;
        for (int i = 0; i < navMeshAgent.path.corners.Length - 1; ++i)
        {
            distance += Vector3.Distance(navMeshAgent.path.corners[i], navMeshAgent.path.corners[i + 1]);
        }

        return distance;
    }

    private void SetUIIndicator()
    {
        if (!PlayerDetected())
        {
            return;
        }
        if (TargetInAttackRange())
        {
            gameObject.SetIndicator(Color.red);
        }
        else
        {
            gameObject.SetIndicator(Color.white);
        }
    }

    //called by EnemyManager on Game End
    //stops all movement

    public void Stop()
    {
        navMeshAgent.enabled = false;
        this.enabled = false;
    }
}
