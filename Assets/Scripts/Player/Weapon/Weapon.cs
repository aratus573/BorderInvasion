using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public enum AimType
    {
        None,
        Scope,
        TargetLock
    }

    public string Name;
    [TextArea]
    public string Description;
    public bool Melee;
    public AimType aimType;
    public float Damage;
    public float BlastDamageMultiplier;
    public float FireCD;
    public AudioClip ShotSound;
    public GameObject BulletPrefab;

}
