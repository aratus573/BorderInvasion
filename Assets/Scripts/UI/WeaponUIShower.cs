using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Text = TMPro.TextMeshProUGUI;

public class WeaponUIShower : MonoBehaviour
{
    [SerializeField] private float alpha = 0.4f;
    public float Alpha
    {
        get {return alpha;}
        set
        {
            if (value >= 0.5f)
            {
                alpha = 0.8f;
            }
            else
            {
                alpha = 0.2f;
            }
        }
    }
    public Image textImage;
    public Text text;
    public Image backgroundImg;
    public Image weapon;
    // Start is called before the first frame update
    void Awake()
    {
        Alpha = 0.3f;
    }
    void Start()
    {
        textImage = transform.GetChild(0).GetComponent<Image>();
        text = transform.GetChild(0).GetChild(0).GetComponent<Text>();
        backgroundImg = transform.GetChild(1).GetComponent<Image>();
        weapon = transform.GetChild(1).GetChild(0).GetComponent<Image>();
    }

    void Update()
    {
        ApplyAlpha();
    }
    public void ApplyAlpha()
    {
        textImage.color = new Color(textImage.color.r, textImage.color.g, textImage.color.b, Alpha);
        text.color = new Color(text.color.r, text.color.g, text.color.b, Alpha);
        if (Alpha == 0.8f) // when the weapon this image represent IS selecetd
        {
            backgroundImg.color = new Color(backgroundImg.color.r, backgroundImg.color.g, backgroundImg.color.b, 0.5f);
            weapon.color = new Color(weapon.color.r, weapon.color.g, weapon.color.b, 1f);
        }
        else // when the weapon this image represent IS NOT selecetd
        {
            backgroundImg.color = new Color(backgroundImg.color.r, backgroundImg.color.g, backgroundImg.color.b, Alpha);
            weapon.color = new Color(weapon.color.r, weapon.color.g, weapon.color.b, Alpha);
        }
    }
}
