using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;

public class ShooterController : MonoBehaviour
{
    [Header("Camera")]
    public CinemachineVirtualCamera AimCamera;
    public CinemachineVirtualCamera LockCamera;

    [Header("PlayerDeath")]
    [SerializeField] private List<MeshDissolve> bodyPartsList;

    [Header("Weapon")]
    [SerializeField] private Weapon CurrentWeapon;
    private float CurrentWeaponCD;
    [SerializeField] private int CurrentWeaponID = 1;
    [SerializeField] private Transform AssaultRifleMuzzle;
    [SerializeField] private Transform RocketMuzzle;
    [SerializeField] private Transform SniperRifleMuzzle;
    private Transform BulletSpawn;
    [SerializeField] private BeamSaber Saber;
    [SerializeField] private ParticleSystem AssaultRifleParticle;
    [SerializeField] private ParticleSystem RocketParticle;
    [SerializeField] private ParticleSystem SniperRifleParticle;
    [System.Serializable]
    private class WeaponParts
    {
        public List<MeshDissolve> weaponParts;
    }
    [SerializeField] private List<WeaponParts> WeaponPartsList = new List<WeaponParts>();
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip DissolveSound;
    [SerializeField] private AudioClip TargeLockSound;
    [SerializeField] private AudioClip ScopeSound;

    [Header("Aiming")]
    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private LayerMask lockColliderLayerMask = new LayerMask();
    private bool isAiming;
    public bool TargetLockMode = false;
    [SerializeField] private float TargetLockRadius;
    [SerializeField] private float TargetLockAngle;
    [SerializeField] private float TargetLockInputCD = 0.1f;
    private float currentLockInputCD = 0f;
    public Transform LockTarget;
    public Transform AimTarget;
    [SerializeField] private Transform LookBase;
    [SerializeField] private Rig BodyRig;
    [SerializeField] private Rig RightHandRig;
    [SerializeField] private Rig LeftHandRifleRig;
    [SerializeField] private Rig LeftHandRocketRig;
    public Transform UpperBody;

    private ThirdPersonController thirdPersonController;
    private StarterAssetsInputs Input;
    private Animator animator;
    private PlayerStats playerStats;
    [SerializeField] private UIManager UI; // added by chamcham

    private void Awake()
    {
        thirdPersonController = GetComponent<ThirdPersonController>();
        Input = GetComponent<StarterAssetsInputs>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>();
    }

    private void Start()
    {
        CurrentWeapon = playerStats.playerData.WeaponList[0];
        BulletSpawn = AssaultRifleMuzzle;
        UI.WeaponPanelSetTransparent(CurrentWeaponID);
    }

    void Update()
    {
        if (playerStats.PlayerDead || GameStateManager.Instance.IsPlayingCutscene)
        {
            //Input.aim = false;
            Input.fire1 = false;
            Input.weapon = CurrentWeaponID;
            animator.ResetTrigger("Attack");
            animator.SetBool("Attacking", false);
            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0, 5f * Time.deltaTime));
        }
        else
        {
            animator.SetLayerWeight(1, 1);
        }

        if (CurrentWeaponCD > 0)
        {
            CurrentWeaponCD -= Time.deltaTime;
        }
        if(currentLockInputCD > 0)
        {
            currentLockInputCD -= Time.deltaTime;
        }
        if (GetComponent<BuildModeController>().GetBuildMode())
        {
            return;
        }

        SwitchWeapon(Input.weapon);
        Aim();
        BodyRotation();
        HandleAttack();
    }

    private void SwitchWeapon(float NewWeaponID)
    {
        if (animator.GetBool("Attacking") || animator.GetBool("Switching")
            || (int)NewWeaponID == 0 || (int)NewWeaponID == CurrentWeaponID)
        {
            return;
        }
        int OldWeaponID = CurrentWeaponID;
        CurrentWeaponID = (int)NewWeaponID;

        //start switching weapon

        //setup animation and stats
        CurrentWeapon = playerStats.playerData.WeaponList[CurrentWeaponID - 1];
        CurrentWeaponCD = 2f;
        animator.SetBool("Switching", true);
        animator.SetInteger("Weapon", CurrentWeaponID);
        UI.WeaponPanelSetTransparent((int)NewWeaponID);// added by chamcham

        //swap out beam saber
        if (OldWeaponID == 2)
        {
            Saber.Clear();
            Saber.BeamSaberOff();
        }
        if(CurrentWeaponID == 1)
        {
            BulletSpawn = AssaultRifleMuzzle;
        }
        else if(CurrentWeaponID == 3)
        {
            BulletSpawn = RocketMuzzle;
        }
        else if(CurrentWeaponID == 4)
        {
            BulletSpawn = SniperRifleMuzzle;
        }

        //dissolve current weapon
        foreach (MeshDissolve WeaponParts in WeaponPartsList[OldWeaponID - 1].weaponParts)
        {
            WeaponParts.StartDissolve();
        }
        //new weapon appear
        foreach (MeshDissolve WeaponParts in WeaponPartsList[CurrentWeaponID - 1].weaponParts)
        {
            WeaponParts.StartAssemble();
        }
        audioSource.PlayOneShot(DissolveSound);

    }

    private void Aim()
    {
        //Set up camera and UI
        if(CurrentWeapon.aimType == Weapon.AimType.Scope && Input.aim && !playerStats.PlayerDead)
        {
            if (!isAiming)
            {
                isAiming = true;
                thirdPersonController.SetSensitivity(playerStats.AimSensitivity);
                AimCamera.gameObject.SetActive(true);
                audioSource.PlayOneShot(ScopeSound);
            }
            UI.SetCrosshair(false, true, CurrentWeaponID);
        }
        else
        {
            if (isAiming)
            {
                isAiming = false;
                audioSource.PlayOneShot(ScopeSound);
                thirdPersonController.SetSensitivity(playerStats.NormalSensitivity);
                AimCamera.gameObject.SetActive(false);
            }
            UI.SetCrosshair(true, false, CurrentWeaponID);
        }

        if(CurrentWeapon.aimType == Weapon.AimType.TargetLock && Input.aim &&  !playerStats.PlayerDead && currentLockInputCD <= 0)
        {
            TargetLockToggle(!TargetLockMode);
        }
        else if(CurrentWeapon.aimType != Weapon.AimType.TargetLock)
        {
            TargetLockToggle(false);
        }

        //Setup aiming raycast

        if (TargetLockMode)
        {
            if (LockTarget != null && LockTarget.GetComponent<EnemyStats>().CurrentHealth != 0)
            {
                AimTarget.position = LockTarget.GetComponent<Collider>().bounds.center;
                return;
            }
            else
            {
                Debug.Log("Lock On Target disappeared");
                TargetLockToggle(false);
            }
        }

        Vector2 ScreenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(ScreenCenter);
        Debug.DrawRay(ray.origin, ray.direction);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, 100f, aimColliderLayerMask))
        {
            //Aiming too close causes wierd animation and bullet
            if (Vector3.Distance(raycastHit.point, BulletSpawn.position) >= 4f && !CurrentWeapon.Melee)
            {
                AimTarget.position = raycastHit.point;
            }
            //too close
            else if (!CurrentWeapon.Melee)
            {
                AimTarget.position = raycastHit.point + ray.direction * 4f;
            }
            else
            {
                AimTarget.position = ray.GetPoint(20);
            }
        }
        else //Aiming at the air
        {
            AimTarget.position = ray.GetPoint(50);
        }

    }

    private void BodyRotation()
    {
        if (playerStats.PlayerDead || GameStateManager.Instance.IsPlayingCutscene)
        {
            BodyRig.weight = 0f;
            RightHandRig.weight = 0f;
            LeftHandRifleRig.weight = 0f;
            LeftHandRocketRig.weight = 0f;
            return;
        }
        if (animator.GetBool("Switching"))
        {
            BodyRig.weight = 0.5f;
            RightHandRig.weight = 0f;
            LeftHandRifleRig.weight = 0f;
            LeftHandRocketRig.weight = 0f;
            return;
        }

        if (!CurrentWeapon.Melee)
        {
            BodyRig.weight = 1f;
            RightHandRig.weight = 1f;
            if(CurrentWeaponID == 1 || CurrentWeaponID == 4)
            {
                LeftHandRifleRig.weight = 1f;
                LeftHandRocketRig.weight = 0f;
            }
            else if(CurrentWeaponID == 3)
            {
                LeftHandRifleRig.weight = 0f;
                LeftHandRocketRig.weight = 1f;
            }
            return;
        }

        else
        {
            RightHandRig.weight = 0f;
            LeftHandRifleRig.weight = 0f;
            LeftHandRocketRig.weight = 0f;
        }
        if(!animator.GetBool("Attacking"))
        {
            BodyRig.weight = 0.5f;
        }
        else
        {
            BodyRig.weight = 0f;
        }
    }

    private void HandleAttack()
    {
        if (!Input.fire1 || CurrentWeaponCD > 0)
        {
            return;
        }
        CurrentWeaponCD = CurrentWeapon.FireCD;
        if (!CurrentWeapon.Melee)
        {
            FireGun(AimTarget.position);
        }
        else if (CurrentWeapon.Melee)
        {
            animator.SetBool("Attacking", true);
            animator.SetTrigger("Attack");
            Saber.PlaySwingAudio();
        }

    }

    private void FireGun(Vector3 Target)
    {
        if(CurrentWeaponID == 1)
        {
            AssaultRifleParticle.Play();
        }
        else if(CurrentWeaponID == 3)
        {
            RocketParticle.Play();
        }
        else if(CurrentWeaponID == 4)
        {
            SniperRifleParticle.Play();
        }
        if(CurrentWeapon.ShotSound != null)
        {
            audioSource.PlayOneShot(CurrentWeapon.ShotSound);
        }
        GameObject bullet = Instantiate(CurrentWeapon.BulletPrefab, BulletSpawn.position, Quaternion.identity);
        if (isAiming)
        {
            //make sure sniper bullet alligns with sniper scope
            bullet.transform.position = Camera.main.transform.position + new Vector3(0, -0.35f, 0);
            bullet.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward);
        }
        else
        {
            bullet.transform.LookAt(Target);
        }
        bullet.GetComponent<Projectile>().SetDamage(CurrentWeapon.Damage, CurrentWeapon.BlastDamageMultiplier);
        animator.SetTrigger("Shoot");
    }

    private void MeleeAttackEnd()
    {
        animator.SetBool("Attacking", false);
        animator.ResetTrigger("Attack");
    }

    private void SwordHitStart()
    {
        Saber.GetComponent<Collider>().enabled = true;
        Saber.WeaponDamage = CurrentWeapon.Damage;
        Saber.gameObject.GetComponent<MeleeWeaponTrail>().Emit = true;
        if(GetComponent<Boost>().GetBoostState() == Boost.BoostState.DashAccel)
        {
            Saber.CriticalHit = true;
        }
    }
    private void SwordHitEnd()
    {
        Saber.GetComponent<Collider>().enabled = false;
        Saber.Clear();
        Saber.gameObject.GetComponent<MeleeWeaponTrail>().Emit = false;
        Saber.CriticalHit = false;
    }

    private void SwitchWeaponEnd()
    {
        CurrentWeaponCD = 0;
        animator.SetBool("Switching", false);
        if (CurrentWeapon.Melee)
        {
            Saber.BeamSaberOn();
        }
    }

    public void AssembleCharacter()
    {
        foreach (MeshDissolve WeaponParts in WeaponPartsList[CurrentWeaponID - 1].weaponParts)
        {
            WeaponParts.StartAssemble();
        }
        if(CurrentWeaponID == 2)
        {
            Saber.BeamSaberOn();
        }
        foreach (MeshDissolve BodyParts in bodyPartsList)
        {
            BodyParts.StartAssemble();
        }
    }

    public void DissolveCharacter()
    {
        foreach (MeshDissolve BodyParts in bodyPartsList)
        {
            BodyParts.StartDissolve();
        }
    }

    public void DissolveWeapon()
    {
        foreach (MeshDissolve WeaponParts in WeaponPartsList[CurrentWeaponID - 1].weaponParts)
        {
            WeaponParts.StartDissolve();
        }
        if (CurrentWeaponID == 2)
        {
            SwordHitEnd();
            Saber.BeamSaberOff();
        }
    }

    public void TargetLockToggle(bool newLockState)
    {
        currentLockInputCD = TargetLockInputCD;
        LockTarget = null;

        //Lock On
        if (newLockState)
        {
            //try to lock on something by overlap sphere
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, TargetLockRadius, lockColliderLayerMask);
            float lowestDistance = TargetLockRadius + 1;
            foreach (Collider col in hitColliders)
            {
                Vector3 direction = col.bounds.center - Camera.main.transform.position;
                //check if target is at the front of camera
                if(Vector3.Angle(Camera.main.transform.forward, col.bounds.center - Camera.main.transform.position) > TargetLockAngle)
                {
                    Debug.Log("too far from screen center");
                    continue;
                }
                //check if there is obstacle
                Physics.Raycast(Camera.main.transform.position, direction.normalized, out RaycastHit hit, direction.magnitude, aimColliderLayerMask);
                if (hit.collider != col)
                {
                    continue;
                }
                //the closest target has priority
                if(direction.magnitude < lowestDistance)
                {
                    LockTarget = col.transform;
                    lowestDistance = direction.magnitude;
                }
            }

            //target lock success
            if (LockTarget != null)
            {
                TargetLockMode = true;
                thirdPersonController.LockCameraPosition = true;
                LockCamera.gameObject.SetActive(true);
                UI.SetTargetLockIndicator(true);
                audioSource.PlayOneShot(TargeLockSound);
                return;
            }
            //no available target
            else
            {
                Debug.Log("Lock On Failed. No available target");
                TargetLockToggle(false);
                return;
            }
        }

        //Lock Off
        if (TargetLockMode)
        {
            TargetLockMode = false;
            LockCamera.gameObject.SetActive(false);
            UI.SetTargetLockIndicator(false);
        }
        thirdPersonController.LockCameraPosition = false;
    }

}
