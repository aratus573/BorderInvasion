using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class PlayerStats : CharacterStats
{
    public PlayerData templateData;
    //playerData don't need to be assigned. This is Public for external access.
    public PlayerData playerData;
    [Header("UI")]
    [SerializeField] private UIManager UI;
    public Color HealthColor;
    public Color GodModeHealthColor;
    [Header("Health Regeneration")]
    [SerializeField] private float RegenTimer;
    [Header("Death")]
    [Tooltip("Player don't take damage and boosting don't cost fuel in GodMode")]
    public bool GodMode = false;
    public bool PlayerDead = false;
    private bool GodModeTimerSet = false;
    public float GodModeTime = 10;
    public float ReviveTimer; // edited by chamcham
    [SerializeField] private float GodModeTimer;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip soundDeath;
    [SerializeField] private GameObject GodModeBarrier;
    [SerializeField] private ParticleSystem PlayerHurtEffect;
    [SerializeField] private ParticleSystem DeathExplosionParticle;
    private Animator animator;
    private ThirdPersonController thirdPersonController;
    private ShooterController shooterController;
    private BuildModeController buildModeController;

    private void Awake()
    {
        if (templateData != null)
        {
            characterData = Instantiate(templateData);
        }
        playerData = (PlayerData)characterData;
        for(int i = 0; i < templateData.WeaponList.Count; ++i)
        {
            playerData.WeaponList[i] = Instantiate(templateData.WeaponList[i]);
        }
        animator = GetComponent<Animator>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        shooterController = GetComponent<ShooterController>();
        buildModeController = GetComponent<BuildModeController>();
    }
    private void Start()
    {
        UI = UIManager.instance;
    }

    private void Update()
    {
        if (GodModeTimer > 0)
        {
            GodModeTimer -= Time.deltaTime;
        }
        if(GodModeTimerSet && GodModeTimer <= 0)
        {
            GodMode = false;
            GodModeTimerSet = false;
            ToggleBarrier();
        }

        if (GodMode)
        {
            UI.UpdateHealthBarFill(CurrentHealth / MaxHealth, "GodMode");
        }
        else
        {
            UI.UpdateHealthBarFill(CurrentHealth / MaxHealth, "Normal");
        }

        if (GameStateManager.Instance.IsPlayingCutscene)
        {
            return;
        }
        //Handle Player revive timer
        if (PlayerDead)
        {
            ReviveTimer -= Time.deltaTime;
            if(ReviveTimer <= 0)
            {
                PlayerRevive();
            }
        }
        //Handle Player health regeneration
        else
        {
            if(RegenTimer > 0)
            {
                RegenTimer -= Time.deltaTime;
            }
            else if(CurrentHealth < MaxHealth)
            {
                CurrentHealth = Mathf.Min(CurrentHealth + HealthRegenRate * Time.deltaTime, MaxHealth);
            }
        }


    }

    public override void TakeDamage(float damage)
    {
        if (GodMode)
        {
            return;
        }
        PlayerHurtEffect.Play();
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        if(CurrentHealth == 0 && PlayerDead == false)
        {
            PlayerDeath();
        }
        RegenTimer = HealthRegenTime;
    }

    private void PlayerDeath()
    {
        gameObject.layer = LayerMask.NameToLayer("Dead");
        animator.SetBool("Dead", true);
        PlayerDead = true;
        ReviveTimer = ReviveTime;
        shooterController.TargetLockToggle(false);
        Instantiate(DeathExplosionParticle, transform.position + Vector3.up, Quaternion.identity, transform);
        audioSource.PlayOneShot(soundDeath);
    }

    private void PlayerRevive()
    {
        // teleport player out of the map before setting gameObject layer back.
        //otherwise enemy will try to attack player for 1 frame.
        transform.position = new Vector3(804, 100, -53);

        animator.SetBool("Dead", false);
        shooterController.AssembleCharacter();
        //buildModeController.SetBuildMode(false);
        CurrentHealth = MaxHealth;
        CurrentFuel = MaxFuel;
        PlayerDead = false;
        GameStateManager.Instance.PlayReviveCutscene();
        GodMode = true;
        gameObject.layer = LayerMask.NameToLayer("Player");
        thirdPersonController.ResetCamera();
    }

    public void playerReady()
    {
        RegenTimer = 0;
        GodModeTimerSet = true;
        GodModeTimer = GodModeTime;
    }

    public void ToggleBarrier()
    {
        GodModeBarrier.SetActive(!GodModeBarrier.activeSelf);
    }

    public void PlayerStop()
    {
        thirdPersonController.FlyMode = false;
        //buildModeController.SetBuildMode(false);
        shooterController.TargetLockToggle(false);
    }

    #region Read from playerData
    public float ReviveTime
    {
        get { if (playerData != null) return playerData.ReviveTime; else return 0; }
        set { playerData.ReviveTime = value; }
    }
    public float HealthRegenTime
    {
        get { if (playerData != null) return playerData.HealthRegenTime; else return 0; }
        set { playerData.HealthRegenTime = value; }
    }
    public float HealthRegenRate
    {
        get { if (playerData != null) return playerData.HealthRegenRate; else return 0; }
        set { playerData.HealthRegenRate = value; }
    }
    public float WalkAcceleration
    {
        get { if (playerData != null) return playerData.WalkAcceleration; else return 0; }
        set { playerData.WalkAcceleration = value; }
    }

    public float VerticalFlightSpeed
    {
        get { if (playerData != null) return playerData.VerticalFlightSpeed; else return 0; }
        set { playerData.VerticalFlightSpeed = value; }
    }

    public float VerticalAcceleration
    {
        get { if (playerData != null) return playerData.VerticalAcceleration; else return 0; }
        set { playerData.VerticalAcceleration = value; }
    }
    public float BoostSpeed
    {
        get { if (playerData != null) return playerData.BoostSpeed; else return 0; }
        set { playerData.BoostSpeed = value; }
    }
    public float BoostAcceleration
    {
        get { if (playerData != null) return playerData.BoostAcceleration; else return 0; }
        set { playerData.BoostAcceleration = value; }
    }
    public float DashAttackSpeed
    {
        get { if (playerData != null) return playerData.DashAttackSpeed; else return 0; }
        set { playerData.DashAttackSpeed = value; }
    }
    public float MaxFuel
    {
        get { if (playerData != null) return playerData.MaxFuel; else return 0; }
        set { playerData.MaxFuel = value; }
    }
    public float CurrentFuel
    {
        get { if (playerData != null) return playerData.CurrentFuel; else return 0; }
        set { playerData.CurrentFuel = value; }
    }
    public float FuelUsage
    {
        get { if (playerData != null) return playerData.FuelUsage; else return 0; }
        set { playerData.FuelUsage = value; }
    }
    public float FlyFuelUsage
    {
        get { if (playerData != null) return playerData.FlyFuelUsage; else return 0; }
        set { playerData.FlyFuelUsage = value; }
    }
    public float DashAttackFuelUsage
    {
        get { if (playerData != null) return playerData.DashAttackFuelUsage; else return 0; }
        set { playerData.DashAttackFuelUsage = value; }
    }
    public float FuelRegen
    {
        get { if (playerData != null) return playerData.FuelRegen; else return 0; }
        set { playerData.FuelRegen = value; }
    }
    public float NormalSensitivity
    {
        get { if (playerData != null) return playerData.NormalSensitivity; else return 0; }
        set { playerData.NormalSensitivity = value; }
    }
    public float AimSensitivity
    {
        get { if (playerData != null) return playerData.AimSensitivity; else return 0; }
        set { playerData.AimSensitivity = value; }
    }
    #endregion
}
