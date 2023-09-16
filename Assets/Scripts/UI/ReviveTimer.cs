using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;

public class ReviveTimer : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;
    [SerializeField] private Text timerText;


    // Update is called once per frame
    void Update()
    {
        if (playerStats.PlayerDead)
        {
            timerText.enabled = true;
            timerText.text = Mathf.FloorToInt(playerStats.ReviveTimer).ToString();
            // timerText.text = Mathf.FloorToInt(playerStats.ReviveTime).ToString();
        }
        else
        {
            timerText.enabled = false;
        }
    }
}
