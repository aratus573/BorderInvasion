using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;
public class BaseRepair : MonoBehaviour
{
    [SerializeField] Text btnText;
    [SerializeField] Text descriptionText;
    [SerializeField] Text priceText;
    [SerializeField] UpgradePanelProgressArea progress;
    [SerializeField] UpgradeButton upgradeButton;
    PlayerBase upgrade;
    [SerializeField] GameObject playerBase;


    // Start is called before the first frame update
    void Start()
    {
        upgrade=playerBase.GetComponent<PlayerBase>();
        btnText.text="Repair Base";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void UpdateDescription(){
        //update description and upgrade progress on upgrade button click
        upgradeButton.chosenUpgrade=upgrade;

        descriptionText.text="Repair 20% of base HP";
        priceText.text="Price: "+20;
        progress.SetCurrTime(0);
        progress.SetMaxTime(99);
        progress.RefreshProgressArea();
    }
}
