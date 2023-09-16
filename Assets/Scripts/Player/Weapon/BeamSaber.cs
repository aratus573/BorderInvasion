using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamSaber : MonoBehaviour
{
    [Header("Weapon")]
    public float WeaponDamage;
    public bool CriticalHit;
    public float CriticalDamageMultiplier = 2f;
    [SerializeField] private List<GameObject> TriggeredList = new List<GameObject>();
    [Header("BeamSaber")]
    [Tooltip("A list of lightsaber blade / blade game objects, each of them can have a light as child. The light's intensity depends on the blade length.")]
    public List<GameObject> bladeGameObjects;

    [Tooltip("Beam extend speed in seconds.")]
    public float bladeExtendSpeed = 0.3f;

    [Tooltip("Whether the saber (and all of its blades) is active initially or not.")]
    public bool saberActive = false;

    [Tooltip("The blade and light color. Alpha should be set, the higher alpha is, the bigger the glow effect. If alpha is 0, then there's no glow effect.")]
    public Color bladeColor;

    public GameObject EnemyHitEffect;

    [SerializeField] private AudioClip soundOn;
    [SerializeField] private AudioClip soundOff;
    [SerializeField] private AudioClip soundHit;
    [SerializeField] private AudioClip soundSwing;
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioSource AudioSourceSwing;

    private class Blade
    {
        // the blade itself
        public GameObject gameObject;

        // the light attached to the blade
        public Light light;

        // minimum blade length
        private float scaleMin;

        // maximum blade length; initialized with length from scene
        private float scaleMax;

        // current scale, lerped between min and max scale
        private float scaleCurrent;

        public bool active = false;

        // the delta is a lerp value within 1 second. it will be initialized depending on the extend speed
        private float extendDelta;

        private float localScaleX;
        private float localScaleZ;

        public Blade(GameObject gameObject, float extendSpeed, bool active)
        {

            this.gameObject = gameObject;
            this.light = gameObject.GetComponentInChildren<Light>();
            this.active = active;

            // consistency check
            if (light == null)
            {
                Debug.Log("No light found. Blade should have a light as child");
            }

            // remember initial scale values (non extending part of the blade)
            this.localScaleX = gameObject.transform.localScale.x;
            this.localScaleZ = gameObject.transform.localScale.z;

            // remember initial scale values (extending part of the blade)
            this.scaleMin = 0f;
            this.scaleMax = gameObject.transform.localScale.y;

            // initialize variables
            // the delta is a lerp value within 1 second. depending on the extend speed we have to size it accordingly
            extendDelta = this.scaleMax / extendSpeed;

            if (active)
            {
                // set blade size to maximum
                scaleCurrent = scaleMax;
                extendDelta *= 1;
            }
            else
            {
                // set blade size to minimum
                scaleCurrent = scaleMin;
                extendDelta *= -1;
            }

        }

        public void SetActive(bool active)
        {
            // whether to scale in positive or negative direction
            extendDelta = active ? Mathf.Abs(extendDelta) : -Mathf.Abs(extendDelta);

        }

        public void SetColor(Color color)
        {
            if (light != null)
            {
                light.color = color;
            }

            // TODO: make fail-safe. accessing index 0 of materials and the fixed constant _MKGlowColor is risky
            gameObject.GetComponent<MeshRenderer>().materials[0].SetColor("_MKGlowColor", color);

        }

        public void updateLight()
        {
            if (this.light == null)
                return;

            // light intensity depending on blade size
            this.light.intensity = this.scaleCurrent;
        }

        public void updateSize()
        {

            // consider delta time with blade extension
            scaleCurrent += extendDelta * Time.deltaTime;

            // clamp blade size
            scaleCurrent = Mathf.Clamp(scaleCurrent, scaleMin, scaleMax);

            // scale in z direction
            gameObject.transform.localScale = new Vector3(this.localScaleX, scaleCurrent, this.localScaleZ);

            // whether the blade is active or not
            active = scaleCurrent > 0;

            // show / hide the gameobject depending on the blade active state
            if (active && !gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
            else if (!active && gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }

        }
    }
   
    private List<Blade> blades;
    
    void Awake()
    {

        // consistency check
        if (bladeGameObjects.Count == 0)
        {
            Debug.LogError("No blades found. Must have at least 1 blade!");
        }

        // store initial attributes
        blades = new List<Blade>();
        foreach (GameObject bladeGameObject in bladeGameObjects)
        {
            blades.Add(new Blade(bladeGameObject, bladeExtendSpeed, false));
        }


        // light and blade color
        InitializeBladeColor();

        // initially update blade length, so that it isn't set to what we have in unity's visual editor
        UpdateBlades();


    }
    private void Start()
    {
        /*
        foreach (Blade blade in blades)
        {
            blade.SetActive(false);
        }
        */
    }

    void Update()
    {
        UpdateBlades();
    }

    // set the color of the light and the blade
    void InitializeBladeColor()
    {

        // check if alpha is set; if it isn't set, then there'll be no glow effect
        if (bladeColor.a == 0)
        {
            // Debug.Log("Beam color alpha is 0.");
            // bladeColor.a = 1;
        }

        // update blade color, light color and glow color
        foreach (Blade blade in blades)
        {
            blade.SetColor(bladeColor);
        }
    }


    public void ToggleBeamSaber()
    {
        if (saberActive)
        {
            BeamSaberOn();
        }
        else
        {
            BeamSaberOff();
        }
    }
    public void BeamSaberOn()
    {
        foreach (Blade blade in blades)
        {
            blade.SetActive(true);
        }
        if (AudioSource.clip != soundOn)
        {
            AudioSource.clip = soundOn;
            AudioSource.Play();
        }
        
    }

    public void BeamSaberOff()
    {
        foreach (Blade blade in blades)
        {
            blade.SetActive(false);
        }
        if (AudioSource.clip != soundOff)
        {
            AudioSource.clip = soundOff;
            //AudioSource.Play();
        }
    }

    public void PlaySwingAudio()
    {
        if(!AudioSourceSwing.isPlaying)
        {
            AudioSourceSwing.clip = soundSwing;
            AudioSourceSwing.Play();
        }

    }

    private void UpdateBlades()
    {
        foreach (Blade blade in blades)
        {

            blade.updateLight();
            blade.updateSize();

        }
        // saber is active if any of the blades is active
        bool active = false;
        foreach (Blade blade in blades)
        {
            if (blade.active)
            {
                active = true;
                break;
            }
        }
        this.saberActive = active;
    }

    void OnCollisionEnter(Collision col)
    {
        
        if (!TriggeredList.Contains(col.gameObject) && col.gameObject.CompareTag("Enemy"))
        {
            AudioSource.PlayOneShot(soundHit);
            //Debug.Log("Hit");
            TriggeredList.Add(col.gameObject);
            if (CriticalHit)
            {
                col.gameObject.GetComponent<CharacterStats>().TakeDamage(WeaponDamage * CriticalDamageMultiplier);
            }
            else
            {
                col.gameObject.GetComponent<CharacterStats>().TakeDamage(WeaponDamage);
            }
            Instantiate(EnemyHitEffect, col.GetContact(0).point, Quaternion.identity);
        }
        else if (col.gameObject.CompareTag("Enemy"))
        {
            //Debug.Log("Multi Hit");
        }
    }

    void OnCollisionExit(Collision col)
    {
        //TriggeredList.Remove(col.gameObject);
    }

    public void Clear()
    {
        TriggeredList.Clear();
    }
}
