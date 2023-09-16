using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public class WeaponUpgradeButton : MonoBehaviour
{
    [SerializeField] int UpgradeManagerListIndex;
    [SerializeField] Text btnText;
    [SerializeField] Text descriptionText;
    [SerializeField] Text priceText;
    [SerializeField] UpgradePanelProgressArea progress;
    [SerializeField] UpgradeButton upgradeButton;
    WeaponUpgrade upgrade;


    // Start is called before the first frame update
    void Start()
    {
        upgrade=UpgradeManager.Instance.WeaponUpgrades[UpgradeManagerListIndex];
        btnText.text=upgrade.Name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateDescription(){
        //update description and upgrade progress on upgrade button click
        upgradeButton.chosenUpgrade=upgrade;

        descriptionText.text=upgrade.description;
        priceText.text="Price: "+upgrade.price;
        progress.SetCurrTime(upgrade.level);
        progress.SetMaxTime(upgrade.maxLevel);
        progress.RefreshProgressArea();
    }
}
