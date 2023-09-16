using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterData : ScriptableObject
{
    [Header("Health")]
    public float MaxHealth;
    public float CurrentHealth;
    [Header("Speed")]
    public float MoveSpeed;
}
