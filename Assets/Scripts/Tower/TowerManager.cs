using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public static TowerManager Instance;
    public List<GameObject> AvailableTypesOfTowerPrefab;
    public GameObject slowedEFX;
    public string selectedEFXName = "SelectedEFX" ;
    public BuildModeController buildModeController {get; private set;}
    public GameObject selectedTower;
    public List<TowerController> controllers = new List<TowerController>();
    public int LayerIgnoreRaycast;
    public int LayerTower;
    public List<int> towerCount = new List<int>() {0, 0, 0, 0, 0, 0, 0, 0};

    void Awake() 
    {    
        LayerIgnoreRaycast = LayerMask.NameToLayer("Ignore Raycast");
        LayerTower = LayerMask.NameToLayer("Tower");
        Instance = this;
    }

    void Start() 
    {
        buildModeController = GameObject.FindGameObjectWithTag("Player").GetComponent<BuildModeController>();
    }
    void Update()
    {
        SetLayerMaskOnBuildMode();
    }

    public void ShowAllIndicatorsIfTrue(bool state)
    {
        foreach (TowerController tower in controllers)
        {
            if (tower.RangeIndicatorRenderer)
            {
                tower.RangeIndicatorRenderer.enabled = state;
            }
        }
    }

    public void SetTowerAttributeByType(string towerName)
    {
        TowerAttributeManager towerAttributeManager = TowerAttributeManager.Instance;
        TowerAttributes att = towerAttributeManager.FindCurrAttributesByName(towerName);
        foreach (TowerController controller in controllers)
        {
            if (controller._Name == towerName)
            {
                controller.SetTowerAttributes(att);
            }
        }
    }

    public void SetLayerMask(TowerController tower, int _layerMask)
    {
        tower.gameObject.layer = _layerMask;
        foreach (GameObject target in tower.LayerMaskSettingTarget)
        {
            target.layer = _layerMask;
        }
    }

    public void EnableMainPartColliders()
    {
        foreach (TowerController controller in controllers)
        {
            if (!controller)
            {
                continue;
            }
            controller.GetComponent<BoxCollider>().enabled = true;
        }
    }

    public void DisableMainPartColliders()
    {
        foreach (TowerController controller in controllers)
        {
            if (!controller)
            {
                continue;
            }
            controller.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void AddTowerCountByItsName(string towerName)
    {
        switch(towerName)
        {
            case ("Basic Tower"):
                towerCount[0] += 1;
                break;
            case ("Snipe Tower"):
                towerCount[1] += 1;
                break;
            case ("Wall"):
                towerCount[2] += 1;
                break;
            case ("Freeze Tower"):
                towerCount[3] += 1;
                break;
            case ("Armor Breaker"):
                towerCount[4] += 1;
                break;
            case ("Stun Tower"):
                towerCount[5] += 1;
                break;
            case ("Empower Tower"):
                towerCount[6] += 1;
                break;
            case ("Focus Tower"):
                towerCount[7] += 1;
                break;    
        }

    }
    
    public void SubtractTowerCount(string towerName)
    {
        switch(towerName)
        {
            case ("Basic Tower"):
                towerCount[0] -= 1;
                break;
            case ("Snipe Tower"):
                towerCount[1] -= 1;
                break;
            case ("Wall"):
                towerCount[2] -= 1;
                break;
            case ("Freeze Tower"):
                towerCount[3] -= 1;
                break;
            case ("Armor Breaker"):
                towerCount[4] -= 1;
                break;
            case ("Stun Tower"):
                towerCount[5] -= 1;
                break;
            case ("Empower Tower"):
                towerCount[6] -= 1;
                break;
            case ("Focus Tower"):
                towerCount[7] -= 1;
                break;
        }

    }

    private void SetLayerMaskOnBuildMode()
    {
        foreach (TowerController tower in controllers)
        {
            if (!tower)
            {
                continue;
            }
            else
            {
                if (buildModeController.GetBuildMode() && tower.gameObject.layer != LayerIgnoreRaycast)
                {
                    SetLayerMask(tower, LayerIgnoreRaycast);
                }
                else if (!buildModeController.GetBuildMode() && tower.gameObject.layer != LayerTower)
                {
                    SetLayerMask(tower, LayerTower);
                }
            }
        }

    }

}

