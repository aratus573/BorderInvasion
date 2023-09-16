using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private float Damage;
    public float Speed;
    public float DestroyTimer;
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
        switch(col.gameObject.tag){
            case "Player":
                col.gameObject.GetComponent<PlayerStats>().TakeDamage(Damage);
                //Debug.Log("Enemy hit player");
                Destroy(gameObject);
                break;
            case "PlayerBase":
                col.gameObject.GetComponent<PlayerBase>().TakeDamage(Damage);
                //Debug.Log("Enemy hit base");
                Destroy(gameObject);
                break;
            default: 
                break;
        }
    }

    public void SetDamage(int NewDamage)
    {
        Damage = NewDamage;
    }
}
