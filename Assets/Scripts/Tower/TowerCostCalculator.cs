using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCostCalculator : MonoBehaviour
{
    public static TowerCostCalculator Instance;
    private float multiplier = 0.2f;
    private float refundRate = 0.6f;
    
    private void Awake() {
        Instance = this;
    }

    public void SetCostByNumOfTower(string towerName)
    {
        // shold be called before SubtractTowerCount / AddTowerCount
        TowerAttributes currAtt =  TowerAttributeManager.Instance.FindCurrAttributesByName(towerName);
        TowerAttributes oriAtt =  TowerAttributeManager.Instance.FindOriginalAttributesByName(towerName);
        TowerManager manager = TowerManager.Instance;
        float totalMultiplier = 1f;
        switch(towerName)
        {
            case ("Basic Tower"):
                totalMultiplier = ( 1 + (manager.towerCount[0]) * multiplier);
                break;
            case ("Snipe Tower"):
                totalMultiplier = ( 1 + (manager.towerCount[1]) * multiplier);
                break;
            case ("Wall"):
                totalMultiplier = ( 1 + (manager.towerCount[2]) * multiplier);
                break;
            case ("Freeze Tower"):
                totalMultiplier = ( 1 + (manager.towerCount[3]) * multiplier);
                break;
            default:
                totalMultiplier = 1;
                break;
        }
        currAtt.resourceCost = Mathf.RoundToInt(oriAtt.resourceCost * totalMultiplier);
        SetRefundByNumOfTower(towerName);
        manager.SetTowerAttributeByType(towerName);

    }

    public void SetRefundByNumOfTower(string towerName)
    {
        TowerAttributes currAtt =  TowerAttributeManager.Instance.FindCurrAttributesByName(towerName);
        currAtt.resourceRefund = Mathf.RoundToInt(currAtt.resourceCost * refundRate);
    }


}
