using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TextMeshProUGUI;
public class ResourceText : MonoBehaviour
{
    [SerializeField] Text resourceText;

    // Update is called once per frame
    void Update()
    {
        resourceText.text="Resource: "+UpgradeManager.Instance.playerResource;
    }
}
