using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    #region Fields
    // ------------------------------------------------------------------------------------------------------------------------
    public TowerAttributes TemplateStats; // Reference in inspector
    public TowerAttributes CurrentStats;
    protected TowerEmpoweringHandler damageHandler;
    protected Transform targetEnemyTransform;
    [HideInInspector]public Vector3 targetColliderCenter;
    public bool IsPlaced = false;
    public float FireCountdown = 0f;
    protected int layerOfTower;

    #region About enemies
    // --------------------------------------------------------------------------------
        // [HideInInspector] 
        public Collider[] EnemiesColliderWithinRange;
        protected int layerOfEnemy;
    // --------------------------------------------------------------------------------
    #endregion About enemies

    #region Tower's attributes from TowerStats
    // --------------------------------------------------------------------------------
        [Space]
        [Header("Tower attributes")]
        public string _Name;
        public int TowerDamage;
        public int ResourceCost;
        public int ResourceRefund;
        public int HealthPoint;
        public float DetectRange;
        protected float DetectRate;
        protected float RotateSpeed;

        protected TowerAttributes.Abilities abilities = new TowerAttributes.Abilities();
        protected float FireRate;
        protected GameObject BulletPrefab;
    // --------------------------------------------------------------------------------
    #endregion Tower's attributes from TowerStats

    #region Inspector setup fields
    // --------------------------------------------------------------------------------
        [Space]
        [Header("Unity Setup Fields")]
        public GameObject TowerBase;
        public Transform FirePosition;
        public MeshRenderer RangeIndicatorRenderer;
        public List<GameObject> LayerMaskSettingTarget = new List<GameObject>();
        [SerializeField] private TowerAudioManager audioManager;
    // --------------------------------------------------------------------------------
    #endregion Inspector setup fields
    
    private ObjectPooler objectPooler;
    private BuildModeController buildModeController;
    private TowerManager towerManager;
    // ------------------------------------------------------------------------------------------------------------------------
    #endregion Fields

    #region Methods
    // ------------------------------------------------------------------------------------------------------------------------
    #region Unity methods: Awake(), Start(), Update()
    // --------------------------------------------------------------------------------
    void Awake()
    {
        InitializeTowerWhenAwakened();
        // if (abilities.canAttack && abilities.hasBullet)
        // {
        //     towerRotation = GetComponent<TowerRotation>();
        //     towerRotation.SetFirePosition(FirePosition);
        //     towerRotation.SetRotateSpeed(RotateSpeed);
        // }
        // else if (abilities.canAttack && !abilities.hasBullet)
        // {
        //     towerHeadup = GetComponent<TowerHeadup>();
        //     towerHeadup.SetTiltingSpeed(RotateSpeed);
        // }
        // if (abilities.canSlow)
        // {
        //     slowDownEnemies = GetComponent<SlowDownEnemies>();
        // }
    }

    protected virtual void InitializeTowerWhenAwakened()
    {
        layerOfEnemy = (1 << LayerMask.NameToLayer("Enemy"));
        damageHandler = GetComponent<TowerEmpoweringHandler>();
        damageHandler.SetController(this);
        damageHandler.ResetOriDamage();
        layerOfTower = (1 << LayerMask.NameToLayer("Ignore Raycast"));
        layerOfTower |= (1 << LayerMask.NameToLayer("Tower"));

    }

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        towerManager = TowerManager.Instance;
        buildModeController = GameObject.FindGameObjectWithTag("Player").GetComponent<BuildModeController>();
        towerManager.SetLayerMask(this, towerManager.LayerIgnoreRaycast);
    }

    void Update()
    {
        WithinUpdate();
        if (!IsPlaced)
        {
            return;
        }

        if (!targetEnemyTransform)
        {
            // if tower is placed but doesn't have a target
            IdleAndRotate();
        }
        else
        {
            // if tower is placed && tower has a target
            TowerBehave();
        }
    }

    protected virtual void WithinUpdate()
    {
        
    }

    // --------------------------------------------------------------------------------
    #endregion Unity methods

    #region Utilities: UpdateTarget(), SetTowerAttributes(), SetLayerMask();
    // --------------------------------------------------------------------------------
    protected virtual void UpdateTarget()
    {
        // EnemiesColliderWithinRange = Physics.OverlapSphere(TowerBase.transform.position, DetectRange, layerOfEnemy);
     
        // if (EnemiesColliderWithinRange.Length == 0)
        // {
        //     targetEnemy = null;
        //     return;
        // }

        // Collider mostDangerousTarget = SearchForTarget();
        
        // if (mostDangerousTarget)
        // {
        //     targetColliderCenter = mostDangerousTarget.bounds.center;
        //     targetEnemy = mostDangerousTarget.transform;
        //     // if (abilities.hasBullet)
        //     // {
        //         // towerRotation.SetTarget(mostDangerousTarget.transform);
        //     // }
        // }
    }

    public virtual void SetTowerAttributes(TowerAttributes att)
    {
        CurrentStats = att;
        abilities = att.abilities;

        _Name = att.name;
        TowerDamage = att.towerDamage;
        ResourceCost = att.resourceCost;
        ResourceRefund = att.resourceRefund;
        HealthPoint = att.healthPoint;
        DetectRange = att.detectRange;
        DetectRate = att.detectRate;

        // if (!abilities.canAttack)
        // {
        //     return;
        // }
        // if (abilities.hasBullet)
        // {
        //     RotateSpeed = att.rotateSpeed;
        //     FireRate = att.fireRate;
        //     BulletPrefab = att.bulletPrefab;
        // }
        // else
        // {
        //     RotateSpeed = att.rotateSpeed;
        // }
        // // CancelInvoke();
        // // InvokeRepeating(nameof(UpdateTarget), 0f, 1/DetectRate);
        
        // FireCountdown = 1f/FireRate;
    }
    
    protected Collider SearchForTarget()
    {
        float shortestPathRemain = Mathf.Infinity;
        Collider tempCollider = null;
        foreach (Collider enemyCol in EnemiesColliderWithinRange)
        {
            if (!enemyCol.gameObject)
            {
                continue;
            }

            EnemyMovement enemyMovement = enemyCol.GetComponent<EnemyMovement>();
            float pathRemain = enemyMovement.GetPathRemainingDistance();

            if (pathRemain < shortestPathRemain)
            {
                shortestPathRemain = pathRemain;
                tempCollider = enemyCol;
            }
        }
        return tempCollider;
    }
    // --------------------------------------------------------------------------------
    #endregion Utilities

    #region Tower's acts: RotateAndAct(), Shoot(), AttackWithoutBullet(), TakeDamage()
    // --------------------------------------------------------------------------------
    protected virtual void IdleAndRotate()
    {

    }
    // private void IdleAndRotate()
    // {
    //         if (!abilities.canAttack)
    //         {
    //             // wall tower can not attack
    //             return;
    //         }

    //         if(abilities.hasBullet && !towerRotation.startRotate)
    //         {
    //             // basic tower can attack and has bullet
    //             towerRotation.StartRotating();
    //         }
    //         else if (abilities.canAttack && !abilities.hasBullet
    //                 && !towerHeadup.startToTilt)
    //         {
    //             // freeze towers CAN attack and DO NOT have bullet
    //             towerHeadup.StartTilting();
    //             towerHeadup.TiltDown();
    //         }

    // }
    private void TowerBehave()
    {
        FireCountdown -= Time.deltaTime;
        if (FireCountdown > 0f || !targetEnemyTransform) {return;}
        ActOrShoot();
        audioManager.Play();
        FireCountdown = 1f / FireRate;
    }
    protected virtual void ActOrShoot()
    {

    }

    // !!!
    public virtual void OnPlace()
    {
        Collider[] towersNearby = Physics.OverlapSphere(TowerBase.transform.position, 20f, layerOfTower);
        if (towersNearby.Length < 0) {return ;}
        foreach (Collider col in towersNearby)
        {
            if ( col.gameObject.tag != "Tower") {continue ;}
            EmpowerTowerController controllerNearby;
            if (col.TryGetComponent<EmpowerTowerController>(out controllerNearby))
            {
                controllerNearby.ResetTowersNearby();
                damageHandler.SetPowerPercentage(controllerNearby.empowerDamagePercentage);
                damageHandler.AddEmpowerStatck();
            }
        }
    }
    public virtual void OnSold()
    {

    }
    // private void ActOrShoot()
    // {
    //     FireCountdown -= Time.deltaTime;
    //     if (FireCountdown <= 0f && targetEnemy != null)
    //     {
    //         if (!abilities.canAttack)
    //         {
    //             // wall tower can not attack
    //             return;
    //         }

    //         if(abilities.hasBullet)
    //         {
    //             // basic tower can attack and has bullet
    //             Shoot();
    //         }
    //         else if (abilities.canAttack && !abilities.hasBullet)
    //         {
    //             // freeze tower can attack and doesn't have bullet
    //             towerHeadup.TiltUp();
    //             RangingAttack();
    //         }
            
    //         audioManager.Play();

    //         if(abilities.canSlow)
    //         {
    //             SlowEnemies();
    //         }
            
    //         FireCountdown = 1f / FireRate;
    //     }
    // }

    protected void Shoot()
    {
        GameObject towerBulletOBJ = objectPooler.SpawnFromPool(BulletPrefab.name, FirePosition.position, FirePosition.rotation);
        // _animator.SetBool("Shoot", true);
        TowerBullet towerBullet = towerBulletOBJ.GetComponent<TowerBullet>();
        towerBullet.Seek(targetEnemyTransform, targetColliderCenter);
        towerBullet.SetTowerController(this);
        towerBullet.damage = TowerDamage;
    }

    public virtual void TowerActAfterEnemyGotShot()
    {

    }
    
    protected void RangingAttack()
    {
        foreach (Collider enemy in EnemiesColliderWithinRange)
        {
            if (!enemy.gameObject)
            {
                return;
            }
            
            if (enemy.GetComponent<EnemyStats>() != null)
            {
                EnemyStats enemyStats = enemy.GetComponent<EnemyStats>();
                enemyStats.TakeDamage(TowerDamage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        HealthPoint =Mathf.Max(0, HealthPoint-amount);
        // if(healthPoint==0){
        //     Destroy(gameObject);
        // }
    }
    // --------------------------------------------------------------------------------
    #endregion Tower's acts
    // ------------------------------------------------------------------------------------------------------------------------
    #endregion Methods

}
