using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class UIManager : MonoBehaviour
{
    [Header("Build Mode")]
    [SerializeField] private GameObject buildModePanel;
    [SerializeField] private GameObject sellModePanel;
    [SerializeField] private GameObject sellModePanel_cancelButton;
    [SerializeField] private GameObject checkPanel;

    [Header("Player Status")]
    // [SerializeField] Slider FuelSlider;
    [SerializeField] BarFillingShaderController fuelBarFiller;
    // [SerializeField] Slider HealthSlider;
    [SerializeField] BarFillingShaderController healthBarFiller;

    [Header("Game Status")]
    [SerializeField] BarFillingShaderController baseBarFiller;
    [SerializeField] private Text baseAlert;
    [SerializeField] Text waveText,waveTimerText,resourceText;
    [SerializeField] EnemyManager enemyManager;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject gameWinPanel;
    [SerializeField] GameObject pausePanel;

    [Header("Player Weapon")]
    [SerializeField] private GameObject WeaponPanel;
    [SerializeField] private List<GameObject> weaponPanelChildList = new List<GameObject>();
    [SerializeField] private GameObject AssaultRifleCrosshair;
    [SerializeField] private GameObject SwordCrosshair;
    [SerializeField] private GameObject RocketCrosshair;
    [SerializeField] private GameObject SniperRifleCrosshair;
    [SerializeField] private GameObject SniperScope;
    [SerializeField] private GameObject TargetLockIndicator;

    [Header("Other")]
    [SerializeField] GameObject upgradePanel;
    public bool isUpgradePanelOpen=false;
    public List<Material> barMaterials = new List<Material>();
    public List<Material> waveCountdownMaterials = new List<Material>();
    public static UIManager instance;
    private void Awake() {
        instance = this;
        for (int i = 1; i < WeaponPanel.transform.childCount; i++)
        {
            weaponPanelChildList.Add(WeaponPanel.transform.GetChild(i).gameObject);
        }    
    }
    private void Update() {
        UpdatePlayerUI();
    }
    public void SetWeaponPanel(bool state)
    {
        WeaponPanel.SetActive(state);
    }

    public void SetBuildModePanel(bool state)
    {
        buildModePanel.SetActive(state);
    }

    public void SetUpgradePanel(bool state)
    {
        upgradePanel.SetActive(state);
    }

    // added by chamcham 
    public void SetSellPanelAndButton(bool state)
    {
        sellModePanel.SetActive(state);
        sellModePanel_cancelButton.SetActive(state);

    }
    public void SetGameWinPanel(bool state){
        gameWinPanel.SetActive(state);
    }
    public void SetGameOverPanel(bool state){
        gameOverPanel.SetActive(state);
    }
    public void SetPausePanel(bool state){
        pausePanel.SetActive(state);
    }
    // added by chamcham 
    public void SetCheckPanel(bool state)
    {
        checkPanel.SetActive(state);
    }

    #region Bar filling
    public void UpdateHealthBarFill(float HealthPercentage, string barState)
    {
        // HealthSlider.value = HealthPercentage;
        // HealthSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = color;

        healthBarFiller.SetNewFill(HealthPercentage);
        healthBarFiller.SetCurrStateTo(barState);
    }
    public void UpdateFuelBarFill(float FuelPercentage, string barState) // revised by chamcham
    {
        // FuelSlider.value = FuelPercentage;
        // FuelSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = color;

        fuelBarFiller.SetNewFill(FuelPercentage);
        fuelBarFiller.SetCurrStateTo(barState);
    }
    public void UpdateBaseBarFill(float HealthPercentage)
    {
        baseBarFiller.SetNewFill(HealthPercentage);
    }
    #endregion

    public void UpdatePlayerUI(){
        waveText.text="Wave "+enemyManager.GetCurrentWave();
        //waveTimerText.text=Mathf.RoundToInt(enemyManager.timeToNextWave)+"";
        resourceText.text="Resource: "+UpgradeManager.Instance.playerResource;
    }
    public void WeaponPanelSetTransparent(int weaponID) // added by chamcham
    {
        int target = weaponID - 1;
        weaponPanelChildList.Clear();
        weaponPanelChildList = new List<GameObject>();

        for (int i = 1; i < WeaponPanel.transform.childCount; i++)
        {
            weaponPanelChildList.Add(WeaponPanel.transform.GetChild(i).gameObject);
        }

        foreach(GameObject child in weaponPanelChildList)
        {
            if (weaponPanelChildList.IndexOf(child) != target)
            {
                WeaponUIShower shower = child.GetComponent<WeaponUIShower>();
                shower.Alpha = 0.3f;
            }
            else
            {
                WeaponUIShower targetShower = child.GetComponent<WeaponUIShower>();
                targetShower.Alpha = 0.8f;
            }
        }
    }

    public void SetCrosshair(bool setCrosshair, bool setSniperScope, int WeaponID)
    {
        AssaultRifleCrosshair.SetActive(false);
        SwordCrosshair.SetActive(false);
        RocketCrosshair.SetActive(false);
        SniperRifleCrosshair.SetActive(false);
        SniperScope.SetActive(false);
        switch (WeaponID)
        {
            case 1:
                AssaultRifleCrosshair.SetActive(setCrosshair);
                break;
            case 2:
                SwordCrosshair.SetActive(setCrosshair);
                break;
            case 3:
                RocketCrosshair.SetActive(setCrosshair);
                break;
            case 4:
                SniperRifleCrosshair.SetActive(setCrosshair);
                SniperScope.SetActive(setSniperScope);
                break;
            default:
                break;
        }
    }

    public void SetTargetLockIndicator(bool state)
    {
        TargetLockIndicator.SetActive(state);
        TargetLockIndicator.GetComponent<Animation>().Play();
    }

    public void SetbaseAlertText(bool state)
    {
        baseAlert.enabled = state;
    }

}
