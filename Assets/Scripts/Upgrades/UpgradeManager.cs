using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;
    public float playerResource;
    [SerializeField] List<PlayerUpgrade> TemplatePlayerUpgrades = new List<PlayerUpgrade>();
    [SerializeField] List<WeaponUpgrade> TemplateWeaponUpgrades = new List<WeaponUpgrade>();
    [SerializeField] List<TowerUpgrade> TemplateTowerUpgrades = new List<TowerUpgrade>();
    [SerializeField] GameObject player;
    PlayerStats playerStats;
    public List<PlayerUpgrade> PlayerUpgrades = new List<PlayerUpgrade>();
    public List<WeaponUpgrade> WeaponUpgrades = new List<WeaponUpgrade>();
    public List<TowerUpgrade> TowerUpgrades = new List<TowerUpgrade>();

    private void Awake() {
        Instance = this;
        playerStats=player.GetComponent<PlayerStats>();
        for(int i = 0; i < TemplatePlayerUpgrades.Count; ++i)
        {
            PlayerUpgrades.Add(Instantiate(TemplatePlayerUpgrades[i]));
        }
        for(int i = 0; i < TemplateWeaponUpgrades.Count; ++i)
        {
            WeaponUpgrades.Add(Instantiate(TemplateWeaponUpgrades[i]));
        }
        for(int i = 0; i < TemplateTowerUpgrades.Count; ++i)
        {
            TowerUpgrades.Add(Instantiate(TemplateTowerUpgrades[i]));
        }
        //playerStats.MaxFuel=playerStats.MaxFuel;
        // Debug.Log(playerStats.MaxHealth);
        // ApplyPlayerUpgrade(PlayerUpgrades[0]);
        // Debug.Log(playerStats.MaxHealth);
        // Debug.Log(playerStats.playerData.WeaponList[0].Damage);
        // ApplyWeaponUpgrade(WeaponUpgrades[0]);
        // Debug.Log(playerStats.playerData.WeaponList[0].Damage);
        // Debug.Log(TowerAttributeManager.Instance.FindAttributesByName("Basic Tower").towerDamage);
        // ApplyTowerUpgrade(TowerUpgrades[0]);
        // Debug.Log(TowerAttributeManager.Instance.FindAttributesByName("Basic Tower").towerDamage);
    }

    public void ApplyPlayerUpgrade(PlayerUpgrade upgrade){
        if(playerResource<upgrade.price){
            Debug.Log("No enough resources");
            return;
        }
        if(upgrade.level<upgrade.maxLevel){
            SubtractResource(upgrade.price);
            upgrade.level++;
            upgrade.price+=upgrade.priceIncrement;
            foreach (PlayerUpgrade.ModifiedAttribute attribute in upgrade.modifiedAttribute)
            {
                ApplyAttribute(attribute, upgrade.level);
            }
            Debug.Log("Upgraded " + upgrade.name);

        }
        else{
            Debug.Log("Already Max Level");
        }
    }
    public void ApplyWeaponUpgrade(WeaponUpgrade upgrade){
        if(playerResource<upgrade.price){
            Debug.Log("No enough resources");
            return;
        }
        if(upgrade.level<upgrade.maxLevel){
            SubtractResource(upgrade.price);
            upgrade.level++;
            upgrade.price+=upgrade.priceIncrement;
            switch(upgrade.modifiedAttribute){
                case WeaponUpgrade.ModifiedAttribute.damage:
                    playerStats.playerData.WeaponList[upgrade.affectedWeaponIndex].Damage
                    =playerStats.templateData.WeaponList[upgrade.affectedWeaponIndex].Damage*(1+upgrade.value*upgrade.level);
                    break;
                case WeaponUpgrade.ModifiedAttribute.attackCD:
                    playerStats.playerData.WeaponList[upgrade.affectedWeaponIndex].FireCD
                    =playerStats.templateData.WeaponList[upgrade.affectedWeaponIndex].FireCD*(1-upgrade.value*upgrade.level);
                    break;
                case WeaponUpgrade.ModifiedAttribute.blastDamageMultiplier:
                    playerStats.playerData.WeaponList[upgrade.affectedWeaponIndex].BlastDamageMultiplier
                    = playerStats.templateData.WeaponList[upgrade.affectedWeaponIndex].BlastDamageMultiplier * (1 + upgrade.value * upgrade.level);
                    break;
            }
            Debug.Log("Upgraded "+ upgrade.name);
        }
        else{
            Debug.Log("Already Max Level");
        }
    }
    public void ApplyTowerUpgrade(TowerUpgrade upgrade){
        if(playerResource<upgrade.price){
            Debug.Log("No enough resources");
            return;
        }
        if(upgrade.level<upgrade.maxLevel){
            SubtractResource(upgrade.price);
            upgrade.level++;
            upgrade.price+=upgrade.priceIncrement;
            foreach(string towerName in upgrade.affectedTowerNames){
                TowerAttributes newTowerAtt=TowerAttributeManager.Instance.FindCurrAttributesByName(towerName);
                TowerAttributes originalTowerAtt=TowerAttributeManager.Instance.FindOriginalAttributesByName(towerName);
                switch(upgrade.modifiedAttribute){
                    case TowerUpgrade.ModifiedAttribute.towerDamage:
                        newTowerAtt.towerDamage=Mathf.RoundToInt(originalTowerAtt.towerDamage*(1+upgrade.value*upgrade.level));
                        break;
                    case TowerUpgrade.ModifiedAttribute.healthPoint:
                        newTowerAtt.healthPoint=Mathf.RoundToInt(originalTowerAtt.healthPoint*(1+upgrade.value*upgrade.level));
                        break;
                    case TowerUpgrade.ModifiedAttribute.fireRate:
                        newTowerAtt.fireRate=Mathf.RoundToInt(originalTowerAtt.fireRate*(1+upgrade.value*upgrade.level));
                        break;
                    case TowerUpgrade.ModifiedAttribute.detectRange:
                        newTowerAtt.detectRange=Mathf.RoundToInt(originalTowerAtt.detectRange*(1+upgrade.value*upgrade.level));
                        break;
                }
                TowerManager.Instance.SetTowerAttributeByType(towerName);
            }
            Debug.Log("Upgraded "+ upgrade.name);
        }
        else{
            Debug.Log("Already Max Level");
        }
    }

    public void ApplyAttribute(PlayerUpgrade.ModifiedAttribute attribute, int level)
    {
        switch (attribute.attributeType)
        {
            case PlayerUpgrade.AttributeType.maxHealth:
                playerStats.MaxHealth = playerStats.templateData.MaxHealth * (1 + attribute.value * level);
                break;
            case PlayerUpgrade.AttributeType.moveSpeed:
                playerStats.MoveSpeed = playerStats.templateData.MoveSpeed * (1 + attribute.value * level);
                break;
            case PlayerUpgrade.AttributeType.boostSpeed:
                playerStats.BoostSpeed = playerStats.templateData.BoostSpeed * (1 + attribute.value * level);
                break;
            case PlayerUpgrade.AttributeType.maxFuel:
                playerStats.MaxFuel = playerStats.templateData.MaxFuel * (1 + attribute.value * level);
                break;
            case PlayerUpgrade.AttributeType.fuelUsage:
                playerStats.FuelUsage = playerStats.templateData.FuelUsage * (1 + attribute.value * level);
                break;
            case PlayerUpgrade.AttributeType.fuelRegen:
                playerStats.FuelRegen = playerStats.templateData.FuelRegen * (1 + attribute.value * level);
                break;
            case PlayerUpgrade.AttributeType.dashSpeed:
                playerStats.DashAttackSpeed = playerStats.templateData.DashAttackSpeed * (1 + attribute.value * level);
                break;
            case PlayerUpgrade.AttributeType.dashFuelUsage:
                playerStats.DashAttackFuelUsage = playerStats.templateData.DashAttackFuelUsage * (1 + attribute.value * level);
                break;
        }
    }

    public void AddResource(int num)
    {
        playerResource += num;
    }
    public void SubtractResource(int num)
    {
        playerResource -= num;
    }
}
