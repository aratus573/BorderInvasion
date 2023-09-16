using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerAttributeManager : MonoBehaviour
{
    public static TowerAttributeManager Instance;
    public List<TowerAttributes> oriTowerAttributes;
    public List<TowerAttributes> currTowerAttributeList;
    public string[] currTowerAttributeNames;

    void Awake()
    {
        Instance = this;
        for (int i = 0; i<oriTowerAttributes.Count; i++)
        {
            currTowerAttributeList.Add(Instantiate(oriTowerAttributes[i]));
        }

        currTowerAttributeNames = new string[currTowerAttributeList.Count];
        for (int i = 0; i < currTowerAttributeList.Count; i++)
        {
            currTowerAttributeNames[i] = currTowerAttributeList[i].name;
        }
    }
    void Start()
    {

    }


    public TowerAttributes FindCurrAttributesByName(string towerName)
    {
        int tempIndex = -1;

        tempIndex = Array.IndexOf(currTowerAttributeNames, towerName);

        if (tempIndex == -1)
        {
            Debug.Log($"Can't find any attribute by {towerName} in currTowerAttributeList.");
            return null;
        }
        else
        {
            return currTowerAttributeList[tempIndex];
        }
    }
    
    public TowerAttributes FindOriginalAttributesByName(string towerName)
    {
        int tempIndex = -1;

        tempIndex = Array.IndexOf(currTowerAttributeNames, towerName);

        if (tempIndex == -1)
        {
            Debug.Log($"Can't find any attribute by {towerName} in oriTowerAttributes. ");
            return null;
        }
        else
        {
            return oriTowerAttributes[tempIndex];
        }
    }


    // public void ApplyUpgrade(TowerUpgrade upgrade){
    //     if(upgrade.level<upgrade.maxLevel){
    //         upgrade.level++;
    //         // foreach(GameObject tower in upgrade.affectedTowers){
    //         //     TowerAttributes
    //         //     switch(upgrade.modifiedAttribute){
    //         //         case TowerUpgrade.ModifiedAttribute.towerDamage:
    //         //             tower.towerDamage=Mathf.RoundToInt(tower.towerDamage*upgrade.value);
    //         //             break;
    //         //         case TowerUpgrade.ModifiedAttribute.healthPoint:
    //         //             tower.healthPoint=Mathf.RoundToInt(tower.healthPoint*upgrade.value);
    //         //             break;
    //         //         case TowerUpgrade.ModifiedAttribute.fireRate:
    //         //             tower.fireRate=Mathf.RoundToInt(tower.fireRate*upgrade.value);
    //         //             break;
    //         //         case TowerUpgrade.ModifiedAttribute.detectRange:
    //         //             tower.detectRange=Mathf.RoundToInt(tower.detectRange*upgrade.value);
    //         //             break;
    //         //     }
    //         // }
    //         Debug.Log("Upgraded "+ upgrade.description);
    //     }
    //     else{
    //         Debug.Log("Already Max Level");
    //     }
    // }

}