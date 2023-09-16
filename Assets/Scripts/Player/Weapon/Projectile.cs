using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private float Damage;
    public float Speed;
    public float DestroyTimer;
    public AudioClip HitSound;
    public GameObject EnemyHitEffect;
    [Header("SpecialProperties")]
    public bool Explosive;
    public GameObject Explosion;
    public float ExplosionRadius;
    private float ExplosionDamage;
    [SerializeField] private List<Transform> DelayDestroyAfterHit;

    public bool Penetrate;
    // Homing is not implemented yet
    public bool Homing;

    void Start()
    {
        Destroy(gameObject, DestroyTimer);
    }

    void Update()
    {
        transform.Translate(Speed * Time.deltaTime * Vector3.forward, Space.Self);
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            return;
        }
        if (col.gameObject.CompareTag("Enemy") && col.gameObject.TryGetComponent<CharacterStats>(out CharacterStats targetStats))
        {
            targetStats.TakeDamage(Damage);
            if (EnemyHitEffect != null)
            {
                Instantiate(EnemyHitEffect, col.GetContact(0).point, Quaternion.identity);
            }
        }
        else if (col.gameObject.CompareTag("Enemy"))
        {
            Debug.LogError("Collider with Enemy tag doesn't have CharacterStats component");
        }
        if (Explosive)
        {
            Instantiate(Explosion, transform.position, Quaternion.identity);
            LayerMask EnemyMask = LayerMask.GetMask("Enemy");
            Collider[] explosionColliders = Physics.OverlapSphere(transform.position, ExplosionRadius, EnemyMask);
            foreach (Collider hitCollider in explosionColliders)
            {
                // avoid doing explosion damage to direct hit target.
                if(hitCollider.gameObject == col.gameObject)
                {
                    continue;
                }
                if (hitCollider.CompareTag("Enemy") && hitCollider.TryGetComponent<CharacterStats>(out CharacterStats tarStats))
                {
                    tarStats.TakeDamage(ExplosionDamage);
                }
                else if (hitCollider.CompareTag("Enemy"))
                {
                    Debug.LogError("Collider with Enemy tag doesn't have CharacterStats component");
                }
            }
        }
        if (HitSound != null)
        {
            AudioSource.PlayClipAtPoint(HitSound, transform.position);
        }
        for(int i = 0; i < DelayDestroyAfterHit.Count; ++i)
        {
            if(DelayDestroyAfterHit[i].TryGetComponent<ParticleSystem>(out ParticleSystem particleSystem))
            {
                particleSystem.Stop(true);
            }
            DelayDestroyAfterHit[i].SetParent(null);
            Destroy(DelayDestroyAfterHit[i].gameObject, 2f);
        }
        if (!Penetrate)
        {
            Destroy(gameObject);
        }
    }

    public void SetDamage(float NewDamage, float BlastDamageMultiplier = 0)
    {
        Damage = NewDamage;
        ExplosionDamage = NewDamage * BlastDamageMultiplier;
    }
}
