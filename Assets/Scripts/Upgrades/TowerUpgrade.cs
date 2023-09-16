using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower Upgrade", menuName = "TowerUpgrade")]
public class TowerUpgrade : ScriptableObject
{
    public int price;
    public int priceIncrement;
    public string Name;
    [TextArea]
    public string description;
    public int level;
    public int maxLevel;
    public string[] affectedTowerNames;
    public ModifiedAttribute modifiedAttribute;
    public float value;
    public enum ModifiedAttribute{towerDamage, healthPoint, fireRate, detectRange}
}
