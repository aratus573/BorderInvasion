using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    [SerializeField] float baseMaxHP;
    [SerializeField] float baseCurrentHP;
    [SerializeField] UIManager UI;
    [SerializeField] GameObject damagedEffect;
    [SerializeField] GameObject heavyDamagedEffect;

    [Header("Alert")]
    //play alert sound when take damage
    [SerializeField] private AudioSource alertAudioSource;
    [SerializeField] private float AlertTime;
    [SerializeField] private float currentAlertTimer;
    private bool isAlert = false;
    private bool isGameEnded=false;

    private void Start()
    {
        baseCurrentHP = baseMaxHP;
        UI = UIManager.instance;
        UI.UpdateBaseBarFill(BaseCurrentHP / BaseMaxHP);
    }
    private void Update()
    {
        DamagedEffect();
        if (currentAlertTimer > 0)
        {
            currentAlertTimer -= Time.deltaTime;
        }
        else
        {
            isAlert = false;
            alertAudioSource.Stop();
            UI.SetbaseAlertText(false);
        }
    }

    private void DamagedEffect()
    {
        if (baseCurrentHP > baseMaxHP * 0.7)
        {
            damagedEffect.SetActive(false);
            heavyDamagedEffect.SetActive(false);
            return;
        }
        if (baseCurrentHP > baseMaxHP * 0.3)
        {
            damagedEffect.SetActive(true);
            heavyDamagedEffect.SetActive(false);
            return;
        }
        damagedEffect.SetActive(true);
        heavyDamagedEffect.SetActive(true);
    }

    public float BaseMaxHP
    {
        get { return baseMaxHP; }
        set
        {
            baseMaxHP = value;
        }
    }
    public float BaseCurrentHP
    {
        get { return baseCurrentHP; }
        set { baseCurrentHP = value; }
    }
    public void ChangeBaseMaxHp(float val)
    {
        baseMaxHP += val;
        baseCurrentHP += val;
    }
    public void TakeDamage(float damage)
    {
        baseCurrentHP = Mathf.Max(baseCurrentHP -= damage, 0);
        UI.UpdateBaseBarFill(BaseCurrentHP / BaseMaxHP);

        if (!isAlert)
        {
            isAlert = true;
            alertAudioSource.Play();
            UI.SetbaseAlertText(true);
        }
        currentAlertTimer = AlertTime;

        if (baseCurrentHP == 0&&!isGameEnded)
        {
            isGameEnded=true;
            GameStateManager.Instance.GameOver();
        }
    }
    public void Repair(){
        baseCurrentHP = Mathf.Min(baseCurrentHP += BaseMaxHP*0.2f, BaseMaxHP);
        UI.UpdateBaseBarFill(BaseCurrentHP / BaseMaxHP);
    }

}
