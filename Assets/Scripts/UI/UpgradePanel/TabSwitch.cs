using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabSwitch : MonoBehaviour
{
    /*
    Player = 0
    Weapon = 1
    Tower  = 2
    Base   = 3
    */ 
    public enum Tab
    {
        Player, Weapon, Tower, Base
    }
    public Tab currTab = Tab.Player;
    [SerializeField] private Transform tabsParent;

    public void SwitchTab(int targetNum)
    {

        if (targetNum == (int)currTab)
        {
            return;
        }

        tabsParent.GetChild((int)currTab).gameObject.SetActive(false);  // hide current tab
        tabsParent.GetChild(targetNum).gameObject.SetActive(true);    // show new tab
        Tab newTab;
        switch (targetNum)
        {
            case (0):
                newTab = Tab.Player;
                break;
            case (1):
                newTab = Tab.Weapon;
                break;
            case (2):
                newTab = Tab.Tower;
                break;
            case (3):
                newTab = Tab.Base;
                break;
            default:
                newTab = Tab.Player;
                break;
        }
        currTab = newTab;

    }
}
