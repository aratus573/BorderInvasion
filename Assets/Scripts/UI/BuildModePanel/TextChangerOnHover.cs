using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class TextChangerOnHover : MonoBehaviour
{
    [SerializeField] private Image btnImage;
    [SerializeField] private Text nameText;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject descriptionPanel;
    public enum TowerCategory
    {
        Basic, Snipe, Wall, Freeze, ArmorBreaker, Stun, Empower, Focus
    }
    public TowerCategory myCategory;

    private void Start() 
    {
        SetText();
    }
    private void SetText()
    {
        TowerAttributeManager attributeManager = TowerAttributeManager.Instance;
        TowerAttributes attribute = attributeManager.currTowerAttributeList[(int)myCategory];
        descriptionText.text = attribute.description + "\n" + "cost: " + attribute.resourceCost;
    }

    public void MouseEnter()
    {
        SetText();
        btnImage.enabled = false;
        nameText.enabled = false;
        descriptionText.enabled = true;
        descriptionPanel.SetActive(true);
    }

    public void MouseExit()
    {
        btnImage.enabled = true;
        nameText.enabled = true;
        descriptionText.enabled = false;
        descriptionPanel.SetActive(false);

    }
}
