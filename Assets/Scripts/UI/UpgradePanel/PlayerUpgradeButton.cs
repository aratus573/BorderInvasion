using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public class PlayerUpgradeButton : MonoBehaviour
{
    [SerializeField] int UpgradeManagerListIndex;
    [SerializeField] Text btnText;
    [SerializeField] Text descriptionText;
    [SerializeField] Text priceText;
    [SerializeField] UpgradePanelProgressArea progress;
    [SerializeField] UpgradeButton upgradeButton;
    PlayerUpgrade upgrade;


    // Start is called before the first frame update
    void Start()
    {
        upgrade=UpgradeManager.Instance.PlayerUpgrades[UpgradeManagerListIndex];
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
        // priceText.text="Price: "+upgrade.price; // this is original code(without .ToString)
        priceText.text="Price: "+upgrade.price.ToString(); // edited by chamcham
        progress.SetCurrTime(upgrade.level);
        progress.SetMaxTime(upgrade.maxLevel);
        progress.RefreshProgressArea();
    }
}
