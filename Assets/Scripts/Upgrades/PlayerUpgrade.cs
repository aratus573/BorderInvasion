using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Upgrade", menuName = "PlayerUpgrade")]
public class PlayerUpgrade : ScriptableObject
{
    public int price;
    public int priceIncrement;
    public string Name;
    [TextArea]
    public string description;
    public int level;
    public int maxLevel;

    public List<ModifiedAttribute> modifiedAttribute;
    [System.Serializable]
    public struct ModifiedAttribute
    {
        public AttributeType attributeType;
        public float value;
    }
    public enum AttributeType { maxHealth, moveSpeed, boostSpeed, maxFuel, fuelUsage, fuelRegen, dashSpeed, dashFuelUsage }
}
