using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using StarterAssets;

public class Boost : MonoBehaviour
{
    public bool OverHeat = false;
    public enum BoostState
    {
        Stop,
        Accelerate,
        DashAccel,
        DashDecel
    }
    [SerializeField] private BoostState boostState;
    [SerializeField] private List<ParticleSystem> BackThrusters = new List<ParticleSystem>();
    [SerializeField] private List<ParticleSystem> LimbThrusters = new List<ParticleSystem>();
    [SerializeField] private ParticleSystem overHeatSmoke;
    public Color BoostColor;
    public Color FlyColor;
    public float BackThrusterBoostSpeed;
    public float BackThrusterFlySpeed;
    public float BackThrusterBoostSize;
    public float BackThrusterFlySize;
    public float LimbThrusterBoostSpeed;
    public float LimbThrusterFlySpeed;
    public float LimbThrusterBoostSize;
    public float LimbThrusterFlySize;

    private float BoostTargetSpeed;
    private float BoostTargetAccel;
    [Header("Audio")]
    [SerializeField] private AudioClip soundDashStart;
    [SerializeField] private AudioClip soundBoostStart;
    [SerializeField] private AudioSource AudioSourceStart;
    [SerializeField] private AudioSource AudioSourceLoop;
    private bool canPlayDashClip = true;
    private bool canPlayStartClip = true;

    [Header("UI")]
    [SerializeField] private UIManager UI;
    public Color FuelUIColor;
    public Color FuelUIOverHeatColor;
    private StarterAssetsInputs Input;
    private Animator animator;
    private PlayerStats playerStats;

    private ThirdPersonController thirdPersonController;

    private void Awake()
    {
        Input = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        boostState = BoostState.Stop;
    }

    void Start()
    {
        UI = UIManager.instance;
    }
    void Update()
    {
        DecideBoostState();
        HandleBoostState();

        if (playerStats.GodMode)
        {
            UI.UpdateFuelBarFill(playerStats.CurrentFuel / playerStats.MaxFuel, "GodMode");
        }
        else if(!OverHeat)
        {
            UI.UpdateFuelBarFill(playerStats.CurrentFuel / playerStats.MaxFuel, "Normal");
        }
        else
        {
            UI.UpdateFuelBarFill(playerStats.CurrentFuel / playerStats.MaxFuel, "OverHeat");
        }
    }

    private void LateUpdate()
    {
        FuelCost();
        UpdateThrusters();
        HandleAudio();
    }

    private void DecideBoostState()
    {
        if(GameStateManager.Instance.IsPlayingCutscene)
        {
            return;
        }
        if (playerStats.PlayerDead)
        {
            SetBoostState(BoostState.Stop);
            return;
        }
        //Boost State for Dash Attack is decided by animation event
        if(boostState == BoostState.DashAccel || boostState == BoostState.DashDecel)
        {
            return;
        }
        if (GetComponent<BuildModeController>().GetBuildMode() || (animator.GetBool("Attacking") &&!animator.GetBool("Boost")))
        {
            SetBoostState(BoostState.Stop);
            return;
        }

        if (Input.sprint && !OverHeat)
        {
            SetBoostState(BoostState.Accelerate);
        }
        else
        {
            SetBoostState(BoostState.Stop);
        }
    }
    private void HandleBoostState()
    {
        switch (boostState)
        {
            case BoostState.Accelerate:
                BoostAccelerate();
                break;

            case BoostState.DashAccel:
                DashAccelerate();
                break;

            case BoostState.DashDecel:
                DashDecelerate();
                break;

            case BoostState.Stop:
                BoostStop();
                break;

            default:
                BoostStop();
                break;
        }
    }
    public BoostState GetBoostState()
    {
        return boostState;
    }
    public void SetBoostState(BoostState State)
    {
        boostState = State;
    }

    public void SetCutsceneBoostState(bool boost)
    {
        if (boost)
        {
            boostState = BoostState.Accelerate;
        }
        else
        {
            boostState = BoostState.Stop;
        }
    }

    private void BoostAccelerate()
    {
        BoostTargetSpeed = playerStats.BoostSpeed;
        BoostTargetAccel = playerStats.BoostAcceleration;
        animator.SetBool("Boost", true);
    }

    private void DashAccelerate()
    {
        BoostTargetSpeed = playerStats.DashAttackSpeed;
        BoostTargetAccel = playerStats.BoostAcceleration;
        animator.SetBool("Boost", true);
    }

    private void DashDecelerate()
    {
        BoostTargetSpeed = 0;
        BoostTargetAccel = 5f;
        animator.SetBool("Boost", true);
    }

    private void BoostStop()
    {
        BoostTargetSpeed = 0;
        BoostTargetAccel = playerStats.BoostAcceleration;
        animator.SetBool("Boost", false);
    }

    public float GetBoostTargetSpeed()
    {
        return BoostTargetSpeed;
    }

    public float GetBoostTargetAccel()
    {
        return BoostTargetAccel;
    }

    private void FuelCost()
    {
        if(!playerStats.GodMode && boostState == BoostState.DashAccel)
        {
            playerStats.CurrentFuel = Mathf.Max(playerStats.CurrentFuel - playerStats.DashAttackFuelUsage * Time.deltaTime, 0f);
        }
        else if(!playerStats.GodMode && boostState == BoostState.Accelerate)
        {
            playerStats.CurrentFuel = Mathf.Max(playerStats.CurrentFuel - playerStats.FuelUsage * Time.deltaTime, 0f);
        }
        else if(!playerStats.GodMode && thirdPersonController.FlyMode)
        {
            playerStats.CurrentFuel = Mathf.Max(playerStats.CurrentFuel - playerStats.FlyFuelUsage * Time.deltaTime, 0f);
        }
        else if(playerStats.CurrentFuel < playerStats.MaxFuel)
        {
            playerStats.CurrentFuel = Mathf.Min(playerStats.CurrentFuel + playerStats.FuelRegen * Time.deltaTime, playerStats.MaxFuel);
        }

        if(playerStats.CurrentFuel == 0)
        {
            OverHeat = true;
            overHeatSmoke.Play(true);
        }
        if(OverHeat && playerStats.CurrentFuel == playerStats.MaxFuel)
        {
            OverHeat = false;
            overHeatSmoke.Stop();
        }

    }

    private void UpdateThrusters()
    {
        if (boostState != BoostState.Stop || thirdPersonController.FlyMode)
        {
            //start thruster VFX
            foreach (ParticleSystem thruster in BackThrusters)
            {
                var main = thruster.main;
                if (boostState != BoostState.Stop )
                {
                    SetThrusterValue(main, BoostColor, BackThrusterBoostSpeed, BackThrusterBoostSize);
                }
                else
                {
                    SetThrusterValue(main, FlyColor, BackThrusterFlySpeed, BackThrusterFlySize);
                }
                thruster.Play();
            }
            foreach (ParticleSystem thruster in LimbThrusters)
            {
                var main = thruster.main;
                if (boostState != BoostState.Stop)
                {
                    SetThrusterValue(main, BoostColor, LimbThrusterBoostSpeed, LimbThrusterBoostSize);
                }
                else
                {
                    SetThrusterValue(main, FlyColor, LimbThrusterFlySpeed, LimbThrusterFlySize);
                }
                thruster.Play();
            }
        }
        else
        {
            //stop
            foreach (ParticleSystem thruster in BackThrusters)
            {
                //var main = thruster.main;
                //SetThrusterValue(main, FlyColor, 0.8f, 0.2f);
                thruster.Stop();
            }
            foreach (ParticleSystem thruster in LimbThrusters)
            {
                //var main = thruster.main;
                //SetThrusterValue(main, FlyColor, 1.2f, 0.25f);
                thruster.Stop();
            }
        }

    }

    private void SetThrusterValue(ParticleSystem.MainModule thruster, Color color, float speed, float size)
    {
        thruster.startColor = color;
        thruster.startSpeedMultiplier = speed;
        thruster.startSizeMultiplier = size;
    }


    private void HandleAudio()
    {
        if(boostState != BoostState.Stop)
        {
            if (!AudioSourceLoop.isPlaying)
            {
                AudioSourceLoop.PlayDelayed(0.3f);
            }
        }
        else if(GetComponent<ThirdPersonController>().FlyMode)
        {
            if (!AudioSourceLoop.isPlaying)
            {
                AudioSourceLoop.Play();
            }
        }
        else
        {
            AudioSourceLoop.Stop();
        }

        if (boostState == BoostState.Stop)
        {
            canPlayDashClip = true;
            canPlayStartClip = true;
        }
        else if (boostState == BoostState.DashAccel && canPlayDashClip)
        {
            canPlayDashClip = false;
            AudioSourceStart.Stop();
            AudioSourceStart.clip = soundDashStart;
            AudioSourceStart.Play();
        }
        else if (boostState == BoostState.Accelerate && canPlayStartClip)
        {
            canPlayStartClip = false;
            AudioSourceStart.Stop();
            AudioSourceStart.clip = soundBoostStart;
            AudioSourceStart.Play();
        }
    }
}
