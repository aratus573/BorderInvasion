using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public class UpgradeButton : MonoBehaviour
{
    public Object chosenUpgrade;
    [SerializeField] Text priceText;
    [SerializeField] UpgradePanelProgressArea progress;
    [SerializeField] GameObject playerBase;

    private void Start() {
    }
    
    public void Upgrade(){
        if(chosenUpgrade==null){
            Debug.Log("No Upgrade Chosen Yet");
            return;
        }
        switch(chosenUpgrade.GetType().ToString()){
            case "PlayerUpgrade":
                PlayerUpgrade playerUpgrade = (PlayerUpgrade)chosenUpgrade;
                UpgradeManager.Instance.ApplyPlayerUpgrade((PlayerUpgrade)chosenUpgrade);
                UpdateProgress(playerUpgrade.level,playerUpgrade.maxLevel,playerUpgrade.price);
                //
                //PlayerStats playerStats=GameObject.Find("PlayerArmature").GetComponent<PlayerStats>();
                //Debug.Log(playerStats.MaxHealth);
                //
                break;
            case "WeaponUpgrade":
                WeaponUpgrade weaponUpgrade = (WeaponUpgrade)chosenUpgrade;
                UpgradeManager.Instance.ApplyWeaponUpgrade((WeaponUpgrade)chosenUpgrade);
                UpdateProgress(weaponUpgrade.level,weaponUpgrade.maxLevel,weaponUpgrade.price);
                break;
            case "TowerUpgrade":
                TowerUpgrade towerUpgrade = (TowerUpgrade)chosenUpgrade;
                UpgradeManager.Instance.ApplyTowerUpgrade((TowerUpgrade)chosenUpgrade);
                UpdateProgress(towerUpgrade.level,towerUpgrade.maxLevel,towerUpgrade.price);
                break;
            case "PlayerBase":
                if(UpgradeManager.Instance.playerResource<20){break;}
                playerBase.GetComponent<PlayerBase>().Repair();
                UpgradeManager.Instance.playerResource-=20;
                break;
            default:
                break;
        }
        Debug.Log("upgrade button upgraded "+chosenUpgrade.name);
    }
    void UpdateProgress(int level,int maxLevel,int newPrice){
        priceText.text="Price: "+newPrice;
        progress.SetCurrTime(level);
        progress.SetMaxTime(maxLevel);
        progress.RefreshProgressArea();
    }
}
