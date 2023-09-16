using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Tower", menuName = "Tower Attribute")]
public class TowerAttributes : ScriptableObject
{
    public new string name;
    public string description;
    public int resourceCost;
    public int resourceRefund;

    public int towerDamage;
    public int healthPoint;
    // attack interval = 1 / fireRate
    public float fireRate;
    public float detectRange;
    // detect interval = 1 / fireRate
    public float detectRate;
    public float rotateSpeed;

    [System.Serializable]
    public class Abilities
    {
        public bool canAttack;
        public bool hasBullet;
        public bool canSlow;
        public bool toLandEnemies;
        public bool toFlyingEnemies;
    }
    public Abilities abilities = new Abilities();

    [Space]
    public GameObject bulletPrefab;

}
