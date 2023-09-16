using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab;
    public GameObject floatingTextPrefab;
    public Transform barPosition;

    public bool alwaysVisible;
    public float visibleTime;
    private float timeLeft;

    Transform UIbar;
    Transform cam;

    [SerializeField] BarFillingShaderController barFillingController;

    private void Awake()
    {
        cam = Camera.main.transform;

        foreach (var canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.name == "HealthBar Canvas")
            {
                UIbar = Instantiate(healthBarPrefab, canvas.transform).transform;
                UIbar.gameObject.SetActive(alwaysVisible);
            }
        }

        barFillingController = UIbar.GetChild(0).GetComponent<BarFillingShaderController>();
    }


    private void LateUpdate()
    {
        if (UIbar != null)
        {
            UIbar.position = barPosition.position;
            UIbar.LookAt(cam.transform);

            if (timeLeft <= 0 && !alwaysVisible)
                UIbar.gameObject.SetActive(false);
            else
                timeLeft -= Time.deltaTime;
        }
    }

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {

        if (UIbar == null)
            return;

        if (currentHealth <= 0)
            Destroy(UIbar.gameObject);

        UIbar.gameObject.SetActive(true);
        timeLeft = visibleTime;

        barFillingController.SetNewFill(currentHealth / maxHealth);
    }


    public void ShowFloatingText(string text, Color color, bool longer = false)
    {

        GameObject go = Instantiate(floatingTextPrefab, this.gameObject.transform.position, Quaternion.identity, transform);
        //Debug.Log(UIbar.position + "show text" + go.transform.position);
        go.GetComponent<FloatingText>().ChangeText(text, color);
        if (longer)
            go.GetComponent<Animator>().SetBool("Long", true);
    }

}

