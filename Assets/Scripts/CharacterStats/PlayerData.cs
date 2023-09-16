using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerStats", menuName = "PlayerStats")]
public class PlayerData : CharacterData
{
    [Header("Camera")]
    public float NormalSensitivity;
    public float AimSensitivity;
    [Header("Health")]
    public float ReviveTime = 20f;
    public float HealthRegenTime = 5f;
    public float HealthRegenRate = 10f;
    [Header("Speed")]
    public float WalkAcceleration = 10.0f;
    public float VerticalFlightSpeed = 6f;
    public float VerticalAcceleration = 10f;
    public float BoostSpeed = 15f;
    public float BoostAcceleration = 20f;
    public float DashAttackSpeed = 30f;
    public float MaxFuel = 100f;
    public float CurrentFuel = 100f;
    public float FlyFuelUsage = 5f;
    public float FuelUsage = 10f;
    public float DashAttackFuelUsage = 30f;
    public float FuelRegen = 10f;

    public List<Weapon> WeaponList = new List<Weapon>();

}
