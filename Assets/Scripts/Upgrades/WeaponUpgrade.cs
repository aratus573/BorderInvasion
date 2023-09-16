using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Upgrade", menuName = "WeaponUpgrade")]
public class WeaponUpgrade : ScriptableObject
{
    public int price;
    public int priceIncrement;
    public string Name;
    [TextArea]
    public string description;
    public int level;
    public int maxLevel;
    public int affectedWeaponIndex;
    public ModifiedAttribute modifiedAttribute;
    public float value;
    public enum ModifiedAttribute{damage, attackCD, blastDamageMultiplier}
}

